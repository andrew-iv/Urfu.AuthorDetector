using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Ninject;
using Urfu.AuthorDetecor.Experiment;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
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

        private static void Test2()


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

                foreach (var postsCount in Enumerable.Range(1, 5).Select(i => i * 2-1))
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
                        Console.WriteLine("{0}% - {1}", success * 100d / ( cnt2), postsCount);
                    }
                }
            }
        }


        static MultiplyMetricProviderAndTransformer CreateMultiplyMetricProvider(float treshold)
        {
            var filter = StaticVars.Kernel.Get<IPostsQueryFilter>();
            var dataExtractor = StaticVars.Kernel.Get<IDataExtractor>();
            var mp = StaticVars.Kernel.Get<IMultiplyMetricsProvider>();
            var posts =
                filter.TopAuthorsMinimum(
                    StaticVars.Kernel.Get<IStatisticsContext>().Posts.Where(x => x.Author.Forum.Id == 3
                        )
                    ).Take(5).SelectMany(x => x.Author.Post.Take(50)).Select(dataExtractor.GetText).SelectMany(mp.GetMetrics);

            var pca = new PcaMetricTransformer(posts, 3);
            pca.SetComponents(treshold);
            return new MultiplyMetricProviderAndTransformer(mp
                        , pca);
        }

        static SimpleMetricProviderAndTransformer CreateSimpeMetricProvider(float treshold)
        {
            var filter = StaticVars.Kernel.Get<IPostsQueryFilter>();
            var dataExtractor = StaticVars.Kernel.Get<IDataExtractor>();
            var mp = StaticVars.Kernel.Get<IPostMetricProvider>();
            var posts =
                filter.TopAuthorsMinimum(
                    StaticVars.Kernel.Get<IStatisticsContext>().Posts.Where(x => x.Author.Forum.Id == 3
                        )
                    ).Take(5).SelectMany(x => x.Author.Post.Take(100)).Select(dataExtractor.GetText).Select(xx => mp.GetMetrics(xx).ToArray());
            var pca = new PcaMetricTransformer(posts, 3);
            pca.SetComponents(treshold);
            return new SimpleMetricProviderAndTransformer(mp
                        , pca);
        }




        static void Main(string[] args)
        {
            _kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary  = true}, new LorModule());
            StaticVars.Kernel = _kernel;
            
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
            //Test(x => new MetricNeighboorClassifier<TrivialMetric>(x), "TrivialMetric");

        }
    }
}
