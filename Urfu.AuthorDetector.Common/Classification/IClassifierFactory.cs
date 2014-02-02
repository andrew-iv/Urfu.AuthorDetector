using System;
using System.Collections.Generic;
using Accord.Statistics.Kernels;
using Ninject;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;
using IKernel = Accord.Statistics.Kernels.IKernel;
using System.Linq;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IClassifierFactory
    {
        IPostMetricProvider PostMetricProvider { get; set; }
        IMultiplyMetricsProvider MultiplyMetricsProvider { get; set; }
        IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    public abstract class BaseClassifierFactory : IClassifierFactory
    {

        public ICommonMetricProvider CommonMetricProvider { get; set; }
        public IPostMetricProvider PostMetricProvider { get; set; }
        public IMultiplyMetricsProvider MultiplyMetricsProvider { get; set; }
        public abstract IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    public class PerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new PerecentileBayesClassifier(authors, PostMetricProvider, MultiplyMetricsProvider);
        }
    }

    public class StupidPerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new StupidPerecentileBayesClassifier(authors, PostMetricProvider, MultiplyMetricsProvider);
        }
    }


    public interface IExperimentLogger
    {
        void LogBenchmark(IClassifierFactory factory, IEnumerable<KeyValuePair<string, string>> parameters);
    }

    public class ExperimentLogger : IExperimentLogger
    {
        private readonly IStatisticsContext _context;
        private readonly IClassifierBenchmark _benchmark;

        [Named("ClsVersionId")]
        [Inject]
        public int VersionId { get; set; }

        public ExperimentLogger(IStatisticsContext context, IClassifierBenchmark benchmark)
        {
            _context = context;
            _benchmark = benchmark;
        }

        public void LogBenchmark(IClassifierFactory factory, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _context.ClassifierResultSet.Add(
            new ClassifierResult()
                {
                    ClassifierVersion = _context.ClassifierVersionSet.FirstOrDefault(x => x.Id == VersionId),
                    MessageCount = _benchmark.MessageCount,
                    RoundCount = _benchmark.RoundCount,
                    Result = _benchmark.Score(factory),
                    DateTime = DateTime.Now,
                    LearningCount = _benchmark.LearningCount,
                    TestsPerRound = _benchmark.TestsInRoundCount,
                    ClassifierParams = parameters.Select(x => new ClassifierParams()
                        {
                            Key = x.Key,
                            Value = x.Value
                        }).ToList()
                });
            _context.SaveChanges();
        }
    }



    public class MSvmClassifierBruter
    {
        public int N { get; set; }
        public int[] K { get; set; }
        public KeyValuePair<IKernel, string>[] Kernels { get; set; }
        public MSvmClassifierParams.LearningAlgorithm[] Algorithms { get; set; }
        public ICommonMetricProvider[] Providers { get; set; }



        public void Brute()
        {
            //var bench = new ForumClassifierBenchmark(StaticVars.Kernel.Get<IDataSource>().GetPosts(30));
            var logger = StaticVars.Kernel.Get<IExperimentLogger>();
            var prms = new Dictionary<string, string>();
            var topPosts = StaticVars.Kernel.Get<IDataSource>().GetPosts(10).SelectMany(x => x.Value.Take(100)).ToArray();

            foreach (var prob in new[]{false,true})
            {
                
            
            foreach (var k in K)
            {
                prms["k"] = k.ToString();
                foreach (var kernel in Kernels)
                {
                    prms["kernel"] = kernel.Value;
                    
                            
                            
                    foreach (var algo in Algorithms)
                    {
                        prms["algo"] = algo.ToString();
                        foreach (var provider in Providers)
                        {
                            prms["provider"] = provider.ToString();
                            var krnl = kernel;
                            if (kernel.Key is Laplacian)
                                krnl = new KeyValuePair<IKernel, string>(
                                    Laplacian.Estimate(GroupMetricFunctions.GetAverages(provider.GetMetrics(topPosts), k)),
                                    kernel.Value
                                    );
                            if (kernel.Key is Gaussian)
                                krnl = new KeyValuePair<IKernel, string>(
                                    Gaussian.Estimate(GroupMetricFunctions.GetAverages(provider.GetMetrics(topPosts), k)),
                                    kernel.Value
                                    );
                            logger.LogBenchmark(new MSvmClassifierClassifierFactory()
                                {
                                    
                                    Params = new MSvmClassifierParams()
                                        {
                                            Algorithm = algo,
                                            CommonProvider = provider,
                                            Kernel = krnl.Key,
                                            Probalistic = prob,
                                            Transform = x => GroupMetricFunctions.GetAverages(x,k)
                                        }
                                },prms);
                        }
                    }
                }
                }
            }
        }

    }

    public class MSvmClassifierClassifierFactory : BaseClassifierFactory
    {

        public MSvmClassifierParams Params { get; set; }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new MSvmClassifier(Params, authors);
        }
    }
}
