using System;
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

    class MultyKSvmStatistics
    {
        private readonly MSvmClassifierParams _prms;
        private readonly int _dimension;

        private readonly int _classes;
        private MulticlassSupportVectorMachine _svm;
        private SelectionStrategy _strategy = SelectionStrategy.Sequential;
        private double _complexity = 10d;
        private double _tolerance = 0.0001;
        private IDictionary<Author, IEnumerable<string>> _authors;
        private Author[] _keys;
        private ICommonMetricProvider _provider;
        private string[] _allPosts;
        private double[] _variance;
        private int[] _allIndex;
        private double[][] _allMetrics;
        private IKernel _kernel = new Polynomial(3);

        public IKernel Kernel
        {
            get { return _kernel; }
            set { _kernel = value; }
        }


        public MultyKSvmStatistics(MSvmClassifierParams prms, IDictionary<Author, IEnumerable<string>> authors)
        {
            _prms = prms;

            _dimension = _prms.CommonProvider.Size;
            //_authors = authors;

            _keys = authors.Keys.ToArray();
            var authorsDict = Enumerable.Range(0, _keys.Length).ToDictionary(x => _keys[x], x => x);
            _provider = _prms.CommonProvider;
            var authorMetricsRow = authors.ToDictionary(x => authorsDict[x.Key],
                                                        x => _prms.Transform(_provider.GetMetrics(x.Value))
                                                            .ToArray()
                );
            _variance =
                authorMetricsRow.Select(x => x.Value.CalculateVariance()).ToArray().CalculateAverage();

            _allIndex = authorMetricsRow.SelectMany(x => x.Value.Select(xx =>
                                                                 x.Key)).ToArray();
            _allMetrics = authorMetricsRow.SelectMany(x => x.Value.Select(DevideByVariance)).ToArray();
        }

        public IKernel EstimateGaussian
        {
            get { return Gaussian.Estimate(_allMetrics); }
        }

        public bool StudyProbalistic { get; set; }

        public SupportVectorMachineLearningConfigurationFunction LearningAlgorithm
        {
            get
            {
                if (_prms.Algorithm == MSvmClassifierParams.LearningAlgorithm.LS_SVM)
                {
                    return (svm, classInputs, classOutputs, i, j) =>
                    {
                        var comp = SequentialMinimalOptimization.EstimateComplexity(svm.Kernel, classInputs);
                        return new LeastSquaresLearning(svm, classInputs, classOutputs)
                        {
                            Complexity = comp
                        };
                    };

                }
                else
                {
                    return (svm, classInputs, classOutputs, i, j) =>
                        {
                            var comp = SequentialMinimalOptimization.EstimateComplexity(svm.Kernel, classInputs);
                            // Use Platt's Sequential Minimal Optimization algorithm
                            return new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                                {
                                    Complexity = comp
                                };
                        };
                }
            }
        }


        public void Study()
        {

            
            _svm = new MulticlassSupportVectorMachine(_dimension, Kernel, _keys.Length);
            ISupportVectorMachineLearning ml = new MulticlassSupportVectorLearning(_svm, _allMetrics, _allIndex)
            {
                // Configure the learning algorithm
                Algorithm = LearningAlgorithm
            };
            ml.Run(true);
            if (StudyProbalistic)
            {
                new MulticlassSupportVectorLearning(_svm, _allMetrics, _allIndex)
                    {
                        Algorithm = (svm, classInputs, classOutputs, i, j) =>
                                    new ProbabilisticOutputLearning(svm, classInputs, classOutputs)
                                        {

                                        }
                    }.Run();
            }
        }

        private double[] DevideByVariance(IEnumerable<double> actual)
        {
            return actual.Zip(_variance, (d, d1) => d / d1).ToArray();
        }

        public IDictionary<Author, double> GetProbabilities(IEnumerable<string> posts)
        {
            int i = 0;
            return _provider.GetMetrics(posts).Select(DevideByVariance).Select(x =>
                {
                    double[] result;
                    _svm.Compute(x, out result);
                    return result;
                }).Aggregate((acc, add) =>
                    {
                        acc = acc == null ? add : add.Zip(acc, (d, d1) => d*d1).ToArray();
                        var max = acc.Max();
                        return acc.Select(x => x/max).ToArray();
                    }).ToDictionary(item => _keys[i++]);
        }

        public IDictionary<Author, int> GetTops(IEnumerable<string> posts)
        {
            int i = 0;
            var cnts =
                _provider.GetMetrics(posts)
                         .Select(DevideByVariance)
                         .Select(x => _svm.Compute(x))
                         .GroupBy(x => x)
                         .ToDictionary(x => _keys[x.Key], x => x.Count());
            foreach (var k in _keys.Where(x=>!cnts.Keys.Contains(x)))
            {
                cnts.Add(k,0);
            }

            return cnts;
        }


        public int Classify(double[] vec,
                             out double res,
                             MulticlassComputeMethod method = MulticlassComputeMethod.Elimination)
        {
            return _svm.Compute(vec, method, out res);
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
            return _svm.Compute(vec, method);
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