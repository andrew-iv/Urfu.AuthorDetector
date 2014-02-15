using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Kernels;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.DataLayer;
using IKernel = Accord.Statistics.Kernels.IKernel;

namespace Bruter
{
    public class ExperimentModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDataSource>().ToMethod(x => new DataSource()
            {
                ForumId = 3,
                DateStart = new DateTime(2008, 1, 1)
            }).InTransientScope();

            Kernel.Bind<int>().ToConstant(2).Named("ClsVersionId");
            Kernel.Bind<int>().ToConstant(5).Named("BayesClsVersionId");
            Kernel.Bind<IExperimentLogger>().To<ExperimentLogger>().InSingletonScope();
            Kernel.Bind<IBayesResultLogger>().To<BayesResultLogger>().InSingletonScope();


            Kernel.Bind<IClassifierBenchmark>().ToMethod(x =>
                {
                    var ds = StaticVars.Kernel.Get<IDataSource>().GetPosts(30);
                    return new MultyBenchmark(ds)
                        {
                            AuthorsCount = 10,
                            LearningCount = 300,
                            RoundCount = 5,
                            TestsInRoundCount = 1000,
                            MessageCount = 5
                        };
                }
                ).InSingletonScope();
        }
    }

    class Program
    {
        static void MakeStatistics()
        {
            var stat = new BinaryBayesStatisticsMaker(StaticVars.Kernel.Get<IDataSource>().GetPosts(30),
                                                      SomeSelectedMetricProviders.AddSelection2)
            {
                LearningCount = 600,
                RoundCount = 25,
                TestsInRoundCount = 400
            };
            foreach (var msgCount in Enumerable.Range(1, 15).Select(i => i * 2))
            {
                stat.MessageCount = msgCount;
                stat.Score(new Random().Next());
            }
        }

        static void Main(string[] args)
        {
            StaticVars.Kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary = true }, new LorModule(), new ExperimentModule());
            MakeStatistics();

            /* StaticVars.InitializeTops(StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x => x.Value));
             var topPosts = StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x => x.Value).ToArray();
             StaticVars.InitializeTops(topPosts);
             var provider = new UseNgramsMetricProvider();
             var allMetrics = provider.GetMetrics(topPosts);
             var bruter = new MSvmClassifierBruter();
             bruter.K = Enumerable.Range(1, 4).Select(i => i*5).ToArray();
             bruter.Providers = new[] {provider};
             bruter.Kernels =
                 Enumerable.Range(1, 5).SelectMany(x => new[]
                     {
                         new KeyValuePair<IKernel, string>(new Polynomial(x), string.Format("Polynominal_{0}_{1}", x, 1))
                         ,
                         new KeyValuePair<IKernel, string>(new Polynomial(x, 2),
                                                           string.Format("Polynominal_{0}_{1}", x, 2)),
                         new KeyValuePair<IKernel, string>(new Polynomial(x, 0.5),
                                                           string.Format("Polynominal_{0}_{1}", x, 0.5))
                     }).Union(new[]
                         {
                             new KeyValuePair<IKernel, string>(new Gaussian(), "Gaussian"),
                             new KeyValuePair<IKernel, string>(new Laplacian(), "Laplacian"),


                         }).ToArray();
             bruter.N = 20;
             bruter.Algorithms = new MSvmClassifierParams.LearningAlgorithm[]{MSvmClassifierParams.LearningAlgorithm.LS_SVM,MSvmClassifierParams.LearningAlgorithm.SMO};
             bruter.Brute();

             var bruter = new KNearestClassifierBruter
                 {
                     K = Enumerable.Range(1, 10).Select(i => i*5).ToArray(),
                     N = 6,
                     Factories = new IKNearestClassifierFactory[]{new KNearestBayesClassifierFactory(), new KNearestSumClassifierFactory()},
                     Providers = new ICommonMetricProvider[]{new SomeWordsAndGrammsMetricProvider()}
                 };
             bruter.Brute();*/
        }
    }
}
