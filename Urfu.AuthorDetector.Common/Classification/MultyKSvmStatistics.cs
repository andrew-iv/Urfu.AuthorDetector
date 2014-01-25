using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{



    public class MSvmClassifier : IClassifier
    {
        private readonly IDictionary<Author, IEnumerable<string>> _authors;
        private readonly ICommonMetricProvider _commonProvider;
        private string[] _allPosts;
        private Author[] _keys;
        private KeyValuePair<int, double[]>[] _authorsMetrics;
        private KeyValuePair<int, double[]> _stdDevs; 

        private MultyKSvmStatistics _stat;

        public MSvmClassifier(IDictionary<Author, IEnumerable<string>> authors, ICommonMetricProvider commonProvider = null)
        {
            _authors = authors;
            _keys = authors.Keys.ToArray();


            _commonProvider = commonProvider;

            _allPosts = authors.SelectMany(x => x.Value).ToArray();
            if (_commonProvider != null)
                InitPostProvider();

        }

        private void InitPostProvider()
        {
            _authorsMetrics = _authors.Select(x=>new{x.Key, Value = _commonProvider.GetMetrics(x.Value)}) .SelectMany(aut =>
                {
                    var ind = Enumerable.Range(0, _keys.Length).First(i => _keys[i] == aut.Key);
                    return aut.Value.Select(x =>
                                            new KeyValuePair<int, double[]>(ind, x));
                }).ToArray();

            var kernel =  new Polynomial(4,1); //Laplacian.Estimate(_authorsMetrics.Select(x => x.Value).ToArray());
            _stat = new MultyKSvmStatistics(_commonProvider.Size, _authors.Count, kernel);
            _stat.Study(_authorsMetrics);

        }

        public IEnumerable<Author> Authors { get { return _keys; } }
        public Author ClassificatePosts(IEnumerable<string> posts)
        {
            return _keys[
                _commonProvider.GetMetrics(posts).Select(x => _stat.Classify(x)).ToArray().GroupBy(x => x)
                    .Select(x
                     => new { cnt = x.Count(), id = x.Key }
                    )
                    .OrderByDescending(x => x.cnt).First().id];
        }

        public string Description { get { return "SvmClassifier"; } }
        public string Name { get { return "SvmClassifier"; } }
    }

    public class MultyKSvmStatistics
    {
        private readonly int _dimension;
        private readonly IKernel _kernel;
        private readonly int _classes;
        private readonly MulticlassSupportVectorMachine _svm;
        private SelectionStrategy _strategy = SelectionStrategy.Sequential;
        private double _complexity = 1d;
        private double _tolerance = 0.25;

        public MultyKSvmStatistics(int dimension, int classes, IKernel kernel = null)
        {
            
            _dimension = dimension;
            _kernel = kernel;
            _classes = classes;
            _svm = _kernel == null
                       ? new MulticlassSupportVectorMachine(dimension, classes)
                       : new MulticlassSupportVectorMachine(dimension, kernel, classes);
        }

        public void Study(IEnumerable<KeyValuePair<int, double[]>> data)
        {
            var keyValuePairs = data as KeyValuePair<int, double[]>[] ?? data.ToArray();
            var input = keyValuePairs.Select(x => x.Value).ToArray();
            var output = keyValuePairs.Select(x => x.Key).ToArray();
            var ml = new MulticlassSupportVectorLearning(_svm, input, output)
                {
                    // Configure the learning algorithm
                    Algorithm = (svm, classInputs, classOutputs, i, j) =>

                                // Use Platt's Sequential Minimal Optimization algorithm
                                new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                                    {
                                        //Complexity = Complexity,
                                        Tolerance = Tolerance,
                                        PositiveWeight = 1d,
                                        NegativeWeight = 1d,
                                        UseClassProportions = true,
                                        //Strategy = Strategy,
                                        //Compact = (_kernel is Linear)

                                    }
                };
            ml.Run(true);
            new MulticlassSupportVectorLearning(_svm, input, output)
                {
                    Algorithm = (svm, classInputs, classOutputs, i, j) =>
                                new ProbabilisticOutputLearning(svm, classInputs, classOutputs)
                }.Run();

        }

        public int Classify(double[] vec,
                             out double res,
                             MulticlassComputeMethod method = MulticlassComputeMethod.Elimination)
        {
            return _svm.Compute(vec,method, out res);
        }

        public int Classify(double[] vec,
                             out double[] res,
                             MulticlassComputeMethod method = MulticlassComputeMethod.Elimination)
        {
            return _svm.Compute(vec, method, out res);
        }

        public int Classify(double[] vec,
                             MulticlassComputeMethod method = MulticlassComputeMethod.Elimination)
        {
            return _svm.Compute(vec,method);
        }

        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        public double Complexity
        {
            get { return _complexity; }
            set { _complexity = value; }
        }

        public SelectionStrategy Strategy
        {
            get { return _strategy; }
            set { _strategy = value; }
        }
    }
}