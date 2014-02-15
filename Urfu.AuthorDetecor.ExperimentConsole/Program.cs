using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Analysis;
using CsvHelper;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetecor.Experiment;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.Common.StatMethods;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetecor.ExperimentConsole
{
    public class Program
    {
        private static readonly string[] Authors = new string[]{
"xtraeft",
"DNA_Seq",
"Quasar",
"Axon",
"Sadler",
"stevejobs",
"tailgunner",
"vurdalak",
"lazyklimm",
"Mystra_x64"}
;


        private static IKernel _kernel;

        private static void Test2(IClassifierFactory factory, string filePrefix, int authorsCount = 10)
        {
            using (var fileStat = File.Open(
                string.Format("{1}_{0}_Total.csv", DateTime.Now.Ticks, filePrefix)
                , FileMode.Create, FileAccess.Write))
            using (var writerStat = new CsvWriter(new StreamWriter(fileStat)))
            {
                var ds = StaticVars.Kernel.Get<IDataSource>().GetPosts(30);
                var benchmark = new MultyBenchmark(ds)
                    {
                        AuthorsCount = authorsCount,
                        LearningCount = 600,
                        RoundCount = 10,
                        TestsInRoundCount = 50

                    };
                foreach (var msgCount in Enumerable.Range(1, 15).Select(i => i * 2)
                    /*.Concat(Enumerable.Range(1, 4).Select(i => i * 5 + 10))*/)
                {
                    benchmark.MessageCount = msgCount;
                    var score = benchmark.ScoreTopN(factory,3, new Random().Next());
                    writerStat.WriteFields(score, msgCount);
                    Console.WriteLine("{0}% - {1}", score * 100, msgCount);
                }
            }
        }

        private static void Test2(IBinaryClassifierFactory factory, string filePrefix, int authorsCount = 10)
        {
            using (var fileStat = File.Open(
                string.Format("{1}_{0}_Total.csv", DateTime.Now.Ticks, filePrefix)
                , FileMode.Create, FileAccess.Write))
            using (var writerStat = new CsvWriter(new StreamWriter(fileStat)))
            {
                var ds = StaticVars.Kernel.Get<IDataSource>().GetPosts(40);
                var benchmark = new SingleBenchmark(ds)
                {
                    AuthorsCount = authorsCount,
                    LearningCount = 500,
                    RoundCount = 20,
                    TestsInRoundCount = 50,
                    ChangeAuthorCount = 10
                };
                foreach (var msgCount in Enumerable.Range(1, 15).Select(i => i * 2 )
                    /*.Concat(Enumerable.Range(1, 4).Select(i => i * 5 + 10))*/)
                {
                    benchmark.MessageCount = msgCount;
                    var score = benchmark.Score(factory, new Random().Next());
                    writerStat.WriteFields(msgCount, score.Item1,score.Item2);
                    Console.WriteLine("{0}% |{1}% - {2}", score.Item1 * 100, score.Item2 * 100, msgCount);
                }
            }
        }


        private static void Test(Func<IDictionary<Author, IEnumerable<string>>, IClassifier> constructor,
                                 string filePrefix, int topAuthors = 10)
        {
            const int forumId = 3;
            const int cnt1 = 1;
            const int cnt2 = 250;
            _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
                {
                    TopAuthors = topAuthors,
                    ForumId = forumId,
                    EndGeneral = new DateTime(2013, 2, 1),
                    PostsCount = 50,
                    StartGeneral = new DateTime(2012, 1, 1)
                });
            var exp = new Experimentator(_kernel, constructor);
            _kernel.Unbind<IExperiment>();

            using (var fileStat = File.Open(
                string.Format("{1}_{0}_Total.csv", DateTime.Now.Ticks, filePrefix)
                , FileMode.Create, FileAccess.Write))
            using (var writerStat = new CsvWriter(new StreamWriter(fileStat)))
            {

                foreach (var postsCount in Enumerable.Range(1, 4).Select(i => i * 5))
                {
                    using (var file = File.Open(
                        string.Format("{2}_{1}_{0}.csv", postsCount, DateTime.Now.Ticks, filePrefix)
                        , FileMode.Create, FileAccess.Write))
                    using (var writer = new CsvWriter(new StreamWriter(file)))
                    {

                        int success = 0;
                        _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
                            {
                                TopAuthors = topAuthors,
                                EndGeneral = new DateTime(2013, 2, 1),
                                ForumId = forumId,
                                PostsCount = postsCount,
                                StartGeneral = new DateTime(2012, 1, 1)
                            });

                        foreach (var res in Enumerable.Range(1, cnt2).Select(i =>
                                                                             exp.Test()))
                        {
                            writer.WriteFields(res.Actual.Identity, res.Returned.Identity, res.IsSuccess);
                            if (res.IsSuccess)
                                success++;
                        }
                        _kernel.Unbind<IExperiment>();

                        writerStat.WriteFields(success * 100d / (cnt2), postsCount);
                        Console.WriteLine("{0}% - {1}", success * 100d / (cnt2), postsCount);
                    }
                }
            }
        }





        public class ExperimentModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.Bind<IDataSource>().ToMethod(x => new DataSource()
                    {
                        ForumId = 3,
                        DateStart = new DateTime(2012, 1, 1)
                    }).InTransientScope();
                Kernel.Bind<int>().ToConstant(4).Named("BayesClsVersionId");
                Kernel.Bind<IBayesResultLogger>().To<DummyBayesResultLogger>().InSingletonScope();
            }
        }

        static void Main(string[] args)
        {
            StaticVars.Kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary = true }, new LorModule(), new ExperimentModule());
            StaticVars.InitializeTops(StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x => x.Value));
            _kernel = StaticVars.Kernel;

            PcaMetricTransformer smpTrans = null;
            
            var pp = new SelectedPostMetricProvider(StaticVars.Kernel.Get<IPostMetricProvider>())
                {
                    Indexes =
                        new int[]
                            {
                                3, 4, 6, 14, 17, 25, 29, 31, 32, 36, 38, 41, 42, 43, 44, 45, 46, 48, 50
                                , 51, 53, 55, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 68, 69, 70
                                /*, 71*/, 72, 73
                            }
                };
            foreach (var auCount in new int[] { 10 })
            {

                Test2(
                    /*new BinaryFromMultyClassifierFactory(*/
                    new KNearestSumClassifierFactory()
                {
                    K = 5,
                    CommonMetricProviders = new[] { SomeSelectedMetricProviders.AddSelection2
                        ,

                        /* , new LengthMetricProvider(), */ }
                    //StaticVars.Kernel.Get<IPostMetricProvider>(),
                    //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                }, "Chi2Test4-2-" + auCount, auCount);

                /*Test2(new StupidPerecentileBayesClassifierFactory()
                {
                    CommonMetricProviders = new[] { SomeSelectedMetricProviders.AddSelection2 }
                    //StaticVars.Kernel.Get<IPostMetricProvider>(),
                    //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                }, "Selection2-" + auCount, auCount);

                Test2(new StupidPerecentileBayesClassifierFactory()
                {
                    CommonMetricProviders = new[] { SomeSelectedMetricProviders.AddSelection1 }
                    //StaticVars.Kernel.Get<IPostMetricProvider>(),
                    //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                }, "Selection1-" + auCount, auCount);


                Test2(new StupidPerecentileBayesClassifierFactory()
                    {
                        CommonMetricProviders = new[] { SomeSelectedMetricProviders.Chi2Test4 }
                        //StaticVars.Kernel.Get<IPostMetricProvider>(),
                        //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                    }, "Chi2Test4-" + auCount, auCount);

                Test2(new StupidPerecentileBayesClassifierFactory()
                    {
                        CommonMetricProviders = new[] { SomeSelectedMetricProviders.Chi2Test3 }
                        //StaticVars.Kernel.Get<IPostMetricProvider>(),
                        //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                    }, "Chi2Test3-" + auCount, auCount);

                */

                continue;


                Test2(new MSvmClassifierClassifierFactory()
                {
                    Params = new MSvmClassifierParams()
                        {
                            CommonProvider =
                            new CombinedCommonMetricProvider(
                            new UseNgramsMetricProvider(),
                            new UseWordsMetricProvider())
                        }
                    /*//Probalistic = true,
                    CommonMetricProvider = new GroupByNMetricProvider(new AverageSingleMetricProvider(
                        ), 5),*/
                    /*MultiplyMetricsProvider = new SelectedMultiMetricProvider(PcaMetricTransformer.CreateMultiplyMetricProvider(0.995f, out smpTrans, AnalysisMethod.Standardize))
                        {
                            Indexes = new int[] { 0, 7, 28, 31, 33, 35, 40, 66, 72, 75, 77 }
                        }*/
                }, "BC5-3-" + auCount, auCount);

                Test2(new MSvmClassifierClassifierFactory()
                {
                    Params = new MSvmClassifierParams()
                    {
                        CommonProvider =
                        new UseWordsMetricProvider()
                    }
                    /*//Probalistic = true,
                    CommonMetricProvider = new GroupByNMetricProvider(new AverageSingleMetricProvider(
                        ), 5),*/
                    /*MultiplyMetricsProvider = new SelectedMultiMetricProvider(PcaMetricTransformer.CreateMultiplyMetricProvider(0.995f, out smpTrans, AnalysisMethod.Standardize))
                        {
                            Indexes = new int[] { 0, 7, 28, 31, 33, 35, 40, 66, 72, 75, 77 }
                        }*/
                }, "BC5-4-" + auCount, auCount);
            }



            /*
            Test2(new StupidPerecentileBayesClassifierFactory()
            {
                PostMetricProvider = StaticVars.Kernel.Get<IPostMetricProvider>(),
                MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
            }, "BC4", 10);

            

            Test2(new StupidPerecentileBayesClassifierFactory()
            {
                //PostMetricProvider = StaticVars.Kernel.Get<IPostMetricProvider>(),
                MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
            }, "BC6", 10);
            */
            /*


            
            Test(dict => new StupidPerecentileBayesClassifier(dict, CreateSimpeMetricProvider(0.99f), CreateMultiplyMetricProvider(0.99f))
                                                                                   , "BayesClassifier");
            Test(dict => new StupidPerecentileBayesClassifier(dict, CreateSimpeMetricProvider(0.8f), CreateMultiplyMetricProvider(0.8f))
                                                                                   , "BayesClassifier_A");


            
            Test(dict => new StupidPerecentileBayesClassifier
                (dict, StaticVars.Kernel.Get<IPostMetricProvider>(), StaticVars.Kernel.Get<IMultiplyMetricsProvider>())
                                                                       , "BayesClassifier2");

            Test(dict => new StupidPerecentileBayesClassifier
                (dict, StaticVars.Kernel.Get<IPostMetricProvider>(),null)
                                                                       , "BayesClassifier3");

            Test(dict => new StupidPerecentileBayesClassifier
                (dict, null, StaticVars.Kernel.Get<IMultiplyMetricsProvider>())
                                                                       , "BayesClassifier4");
            
            /*
            Test(x => new NeighboorClassifier(x, new SelectedPostMetricProvider()), "NeighboorClassifier");
            
            Test(x => new StupidBayesClassifier(x, new SelectedPostMetricProvider()), "BayesClassifier", 50);
            Test(x => new NeighboorClassifier(x, new SelectedPostMetricProvider()), "NeighboorClassifier", 50);
            */
            //Test(x => new MetricNeighboorClassifier<TrivialMetric>(x), "TrivialMetric");*/

        }
    }
}
