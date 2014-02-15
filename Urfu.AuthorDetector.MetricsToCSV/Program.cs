using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using CsvHelper;
using MathNet.Numerics.Statistics;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.Common.StatMethods;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.MetricsToCSV
{
    internal class Program
    {
        private static IStatisticsContext _context;
        private static List<AuthorMinPosts> _authors;
        private static Dictionary<string, string[]> _authorPosts;
        private static StandardKernel _kernel;
        private static IDataExtractor _dataExtractor;

        private static void ConfigureWriter(CsvWriter writer)
        {
            writer.Configuration.QuoteAllFields = false;
        }

        public class MetricModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.Unbind<IPostMetricProvider>();
                Kernel.Bind<IPostMetricProvider>().To<AllAllPostMetricProvider>().InThreadScope();
                //Kernel.Bind<IMultiplyMetricsProvider>().To<ISentenceMetricProvider>().InSingletonScope();



                /*Kernel.Bind<IMetricSelector>().ToConstructor(x=>new Chi2ForAuthorMetricSelector(x.Context.Request.Parameters))
                    (x=>new Chi2ForAuthorMetricSelector(x.Context.Parameters.,) ).InTransientScope();
                //Kernel.Bind<>().To<Chi2ForAuthorMetricSelector>().InThreadScope();*/
            }
        }

        private const int TopsCount = 500;
        private const int SelectParams = 10;


        private static void Init()
        {

            var appSettings = ConfigurationManager.AppSettings;
            var topCount = int.Parse(appSettings.Get("TopAuthors"));
            var startYear = int.Parse(appSettings.Get("StartYear"));
            var endYear = int.Parse(appSettings.Get("EndYear"));
            _kernel = new StandardKernel(new CommonModule(){NeedCreateDictionary = false}, new LorModule() /*, new MetricModule()*/);
            
            StaticVars.Kernel = _kernel;

            _dataExtractor = _kernel.Get<IDataExtractor>();
            _context = _kernel.Get<IStatisticsContext>();

            var filter = _kernel.Get<IPostsQueryFilter>();
            var postsQuery = filter.FilterDate(filter.OnlyForum(_context.Posts, 3),
                                               new DateTime(startYear, int.Parse(appSettings.Get("StartMonth")), 1),
                                               new DateTime(endYear, 1, 1));
            _authors = filter.TopAuthors(postsQuery, 0
                ).Take(topCount).ToList();
            var authorIds = _authors.Select(x => x.Author.Id).ToArray();
            var posts =
                postsQuery.Where(x => authorIds.Contains(x.Author.Id)).Select(_dataExtractor.GetText).ToArray();
            StaticVars.InitializeTops(posts, TopsCount, TopsCount *2);
            _authorPosts = 
                _authors.ToDictionary(x=>x.Author.Identity,x=>  x.Author.Post.Select(_dataExtractor.GetText).ToArray());
        }

        private static void Main(string[] args)
        {
            const string topAuthorsFile = "TopAuthors.csv";
            string metricsFile = DateTime.Now.Ticks +  "_Chi2TopMetricSelector_BestMetrics.csv";

            Init();

            GC.Collect();
            var provider = new CombinedCommonMetricProvider(
                            new SimpleStatMetricProvider()  
                            /*new UseNgramsMetricProvider(),
                            new UseWordsMetricProvider(),
                            new GrammemesPostMetricProvider(),
                            new PunctuationMetricProvider()*/
                );
            Console.WriteLine("Average Length - {0}",
                              _authorPosts.SelectMany(x => x.Value.Select(xx => xx.Length)).ToArray().Average());
            Console.WriteLine("Median Length - {0}",
                              _authorPosts.SelectMany(x => x.Value.Select(xx => (double)xx.Length)).Median());
            var allLength = _authorPosts.Select(x => x.Value.Select(xx => xx.Length).ToArray());

            WriteMetricsToFile("length_median.csv", Enumerable.Range(2, 29).Select(delegate(int i)
                {
                    var lengths = allLength.SelectMany(
                        x => Enumerable.Range(0, x.Length - i + 1).Select(j => (double) x.Skip(j).Take(i).Sum())).ToArray();
                    return new double[]
                        {
                            i,
                            lengths.Quantile(0.25),
                            lengths.Median(),
                            lengths.Quantile(0.75)
                        };
                }).ToArray(), provider);
            

            WriteMetricsToFile("all-stat_.csv", provider.GetMetrics(_authorPosts.SelectMany(x => x.Value)), provider);                          
            return;
            var metricSelector = new Chi2TopMetricSelector(_authorPosts.Select(x => x.Value).ToArray(), SelectParams);
                        var topMetricIds = metricSelector.SelectMetrics(provider
                            ) ;
            foreach (var kvp in topMetricIds)
            {
                using (var file = File.Open(string.Format("Count_{0}_{1}", kvp.Key, metricsFile), FileMode.Create, FileAccess.Write))
                        {
                            using (var writer = new CsvWriter(new StreamWriter(file)))
                            {
                                var names = provider.Names.ToArray();
                                ConfigureWriter(writer);
                                foreach (var id in kvp.Value.OrderBy(x=>x))
                                {
                                    writer.WriteFields(names[id].Split('_').Last(),id,names[id]);
                                }
                            }
                        }
            }
                      
            
            /*
            using (var file = File.Open(topAuthorsFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    ConfigureWriter(writer);
                    foreach (var author in _authors)
                    {
                        writer.WriteFields(author.Author.Identity, author.PostCount);

                    }
                }
            }*/

            //WriteSentenceStats();
        }

        private static void WriteMetricsToFile(string fileName, IEnumerable<double[]> metrics, ICommonMetricProvider setprovider = null)
        {
            var provider = setprovider?? _kernel.Get<IMultiplyMetricsProvider>();
            using (var file = File.Open(DateTime.Now.Ticks+"_"+ fileName, FileMode.Create, FileAccess.Write))
            using (var writer = new CsvWriter(new StreamWriter(file)))
            {
                ConfigureWriter(writer);
                writer.WriteFields(strings: provider.Names.ToArray());
                foreach (
                    var text in metrics
                        )
                {
                    writer.WriteFields(strings: text.Cast<object>().ToArray());
                }
            }

        }

        

        private static void WriteSentenceStats()
        {
            var statTime = DateTime.Now.Ticks;
            var provider = _kernel.Get<IMultiplyMetricsProvider>();
            //var allMetrics = new List<double[]>();

            var allMetrics = _authorPosts.ToDictionary(x=>x.Key,x=>x.Value.SelectMany( provider.GetMetrics) .ToArray());
            WriteMetricsToFile("all-stat_" + statTime + ".csv", allMetrics.SelectMany(x => x.Value));                          

            var trans = new PcaMetricTransformer(allMetrics.SelectMany(x=>x.Value),method:AnalysisMethod.Standardize);
            //trans.ApplyFilter(allMetrics,new );
            //trans.ApplyFilter(allMetrics.Select(x=>x.Value),new KSTwoSampleTestMetricValuenceFilterImpl());


            foreach (var author in allMetrics)
            {

                //var correlationTable = metrics.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray().CorrelationTablePearson(provider.Size);


                WriteMetricsToFile(author.Key + " " + statTime + "-sent-stat.csv", author.Value, provider);

                //WriteMetricsToFile(author.Key + "-sent-stat-comp.csv",  author.Value.Select(trans.GetMetric) , trans);

              

               /* using (var file = File.Open(author.Author.Identity + "-sent-correlation.csv", FileMode.Create, FileAccess.Write))
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    ConfigureWriter(writer);
                    writer.WriteFields(strings: new object[]{ "" }.Concat(provider.Names).ToArray());
                    foreach (
                        var i in Enumerable.Range(0, provider.Size)
                            )
                    {
                        writer.WriteFields(strings: new object[]{ provider.Names.ElementAt(i) }.Concat(Enumerable.Range(0, provider.Size).Select(j=>correlationTable[i,j]).Cast<object>()).ToArray());
                    }
                }*/
            }

            WriteMetricsToFile("all-stat_"+statTime+".csv", allMetrics.SelectMany(x=>x.Value));

           /*var newProvider = new SelectedSentenceMetricProvider(Enumerable.Range(0, provider.Size).Where(i => allMetrics.Select(x => x.ElementAt(i)).Distinct().Count() > 2).ToArray(),provider);

            var cutMetrics = _authors.SelectMany(author => _context.Posts.Where(x => x.Author.Id == author.Author.Id))
                    .ToList()
                    .SelectMany(x => newProvider.GetMetrics(_dataExtractor.GetText(x)).Select(xx=>xx.Select(xxx=>(double)xxx).ToArray()).ToArray())
                    .ToArray();

            WriteMetricsToFile("all-cut-stat.csv", cutMetrics,newProvider);*/
        }
    }
}


