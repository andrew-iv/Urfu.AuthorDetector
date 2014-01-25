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


        private static StandardKernel _kernel;

        private static void Test2(IClassifierFactory factory, string filePrefix, int authorsCount = 10)
        {
            using (var fileStat = File.Open(
                string.Format("{1}_{0}_Total.csv", DateTime.Now.Ticks, filePrefix)
                , FileMode.Create, FileAccess.Write))
            using (var writerStat = new CsvWriter(new StreamWriter(fileStat)))
            {
                var ds = StaticVars.Kernel.Get<IDataSource>().GetPosts(30);
                var benchmark = new ForumClassifierBenchmark(ds)
                    {
                        AuthorsCount = authorsCount,
                        LearningCount = 300,
                        RoundCount = 3,
                        TestsInRoundCount = 2000

                    };
                foreach (var msgCount in Enumerable.Range(1, 2).Select(i => i * 25)/*.Concat(Enumerable.Range(1, 6).Select(i => i * 5 + 20))*/)
                {
                    benchmark.MessageCount = msgCount;
                    var score = benchmark.Score(factory, new Random().Next());
                    writerStat.WriteFields(score, msgCount);
                    Console.WriteLine("{0}% - {1}", score * 100, msgCount);
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

                foreach (var postsCount in Enumerable.Range(1, 5).Select(i => i * 2 - 1))
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
            }
        }

        static void Main(string[] args)
        {
            _kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary = false }, new LorModule(), new ExperimentModule());
            StaticVars.Kernel = _kernel;
            StaticVars.InitializeTops(StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x=>x.Value));

            PcaMetricTransformer smpTrans = null;
            var smp = PcaMetricTransformer.CreateSimpeMetricProvider(0.99f, out smpTrans, AnalysisMethod.Standardize);
            /* PcaMetricTransformer mmpTrans = null;
             var mmp = PcaMetricTransformer.CreateMultiplyMetricProvider(0.99f, out mmpTrans);


             Test2(new StupidPerecentileBayesClassifierFactory()
                 {
                     PostMetricProvider = smp,
                     MultiplyMetricsProvider = mmp
                 },"BC1",10);

             Test2(new StupidPerecentileBayesClassifierFactory()
             {
                 //PostMetricProvider = smp,
                 MultiplyMetricsProvider = mmp
             }, "BC2", 10);
             * */

            /*Test2(new StupidPerecentileBayesClassifierFactory()
            {
                PostMetricProvider = new SelectedPostMetricProvider(smp) { Indexes = new[] { 1, 5, 6, 7, 9,12,15,17,18,23,33,36,47,52,54,55 } },
                //MultiplyMetricsProvider = mmp
            }, "BC3-1", 10);

            Test2(new StupidPerecentileBayesClassifierFactory()
            {
                PostMetricProvider = new SelectedPostMetricProvider(smp) { Indexes = new[] { 1, 6, 7, 8 } },
                //MultiplyMetricsProvider = mmp
            }, "BC3-1", 10);

            
            Test2(new StupidPerecentileBayesClassifierFactory()
            {
                PostMetricProvider = smp,
                //MultiplyMetricsProvider = mmp
            }, "BC3-2", 10);
             * */
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
                if (auCount != 10)
                {
                    Test2(new StupidPerecentileBayesClassifierFactory()
                        {
                            PostMetricProvider = StaticVars.Kernel.Get<IPostMetricProvider>(),
                            //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                        }, "BC5-1-" + auCount, auCount);

                    Test2(new StupidPerecentileBayesClassifierFactory()
                        {
                            PostMetricProvider = pp,
                            //MultiplyMetricsProvider = StaticVars.Kernel.Get<IMultiplyMetricsProvider>()
                        }, "BC5-2-" + auCount, auCount);
                }

                Test2(new MSvmClassifierClassifierFactory()
                {
                    CommonMetricProvider = new GroupByNMetricProvider(new AverageSingleMetricProvider(
                        new CombinedCommonMetricProvider(
                        new UseNgramsMetricProvider() /*, new UseWordsMetricProvider()*/)), 25),
                    /*MultiplyMetricsProvider = new SelectedMultiMetricProvider(PcaMetricTransformer.CreateMultiplyMetricProvider(0.995f, out smpTrans, AnalysisMethod.Standardize))
                        {
                            Indexes = new int[] { 0, 7, 28, 31, 33, 35, 40, 66, 72, 75, 77 }
                        }*/
                }, "BC5-3-" + auCount, auCount);
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
