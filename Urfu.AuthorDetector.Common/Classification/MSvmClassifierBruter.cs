using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using ResolutionExtensions = Ninject.ResolutionExtensions;

namespace Urfu.AuthorDetector.Common.Classification
{
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
            var logger = ResolutionExtensions.Get<IExperimentLogger>(StaticVars.Kernel);
            var prms = new Dictionary<string, string>();
            var topPosts =
                ResolutionExtensions.Get<IDataSource>(StaticVars.Kernel)
                                    .GetPosts(10)
                                    .SelectMany(x => x.Value.Take(100))
                                    .ToArray();

            foreach (var prob in new[] {false, true})
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
                                        Laplacian.Estimate(
                                            GroupMetricFunctions.GetAverages(provider.GetMetrics(topPosts), k)),
                                        kernel.Value
                                        );
                                if (kernel.Key is Gaussian)
                                    krnl = new KeyValuePair<IKernel, string>(
                                        Gaussian.Estimate(GroupMetricFunctions.GetAverages(
                                            provider.GetMetrics(topPosts), k)),
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
                                                Transform = x => GroupMetricFunctions.GetAverages(x, k)
                                            }
                                    }, prms);
                            }
                        }
                    }
                }
            }
        }
    }


    public class KNearestClassifierBruter
    {
        public int N { get; set; }
        public int[] K { get; set; }
        public ICommonMetricProvider[] Providers { get; set; }
        public IKNearestClassifierFactory[] Factories { get; set; }



        public void Brute()
        {
            //var bench = new ForumClassifierBenchmark(StaticVars.Kernel.Get<IDataSource>().GetPosts(30));
            var logger = ResolutionExtensions.Get<IExperimentLogger>(StaticVars.Kernel);
            var prms = new Dictionary<string, string>();
            foreach (var k in K)
            {
                prms["k"] = k.ToString();
                foreach (var factory in Factories)
                {
                    factory.K = k;
                    prms["factory"] = factory.GetType().ToString();
                    foreach (var provider in Providers)
                    {
                        factory.CommonMetricProviders = new[]{provider};

                        logger.LogBenchmark(factory, prms);
                    }
                }
            }


        }
    }
}

