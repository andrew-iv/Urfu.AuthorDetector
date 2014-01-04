using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.MetricsToCSV
{
    internal class Program
    {
        private static IStatisticsContext _context;
        private static List<AuthorMinPosts> _authors;
        private static IEnumerable<string>[] _authorPosts;
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
                Kernel.Bind<IPostMetricProvider>().To<AllPostMetricProvider>().InThreadScope();
                /*Kernel.Bind<IMetricSelector>().ToConstructor(x=>new Chi2ForAuthorMetricSelector(x.Context.Request.Parameters))
                    (x=>new Chi2ForAuthorMetricSelector(x.Context.Parameters.,) ).InTransientScope();
                //Kernel.Bind<>().To<Chi2ForAuthorMetricSelector>().InThreadScope();*/
            }
        }

        private const int TopsCount = 400;
        private const int SelectParams = 1;


        private static void Init()
        {

            var appSettings = ConfigurationManager.AppSettings;
            var topCount = int.Parse(appSettings.Get("TopAuthors"));
            var startYear = int.Parse(appSettings.Get("StartYear"));
            var endYear = int.Parse(appSettings.Get("EndYear"));
            _kernel = new StandardKernel(new CommonModule(), new MetricModule());
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
            StaticVars.InitializeTops(posts, TopsCount);
            _authorPosts = _authors.Select(x => x.Author.Post.Select(_dataExtractor.GetText).ToArray() as IEnumerable<string>)
                                 .ToArray();
        }

        private static void Main(string[] args)
        {
            const string topAuthorsFile = "TopAuthors.csv";
            const string metricsFile = "BestMetrics.csv";

            Init();

            GC.Collect();

            Console.WriteLine("Average Length - {0}",
                              _authorPosts.SelectMany(x => x.Select(xx => xx.Length)).ToArray().Average());

            var metricSelector = new Chi2ForAuthorMetricSelector(_authorPosts, SelectParams);
            var topMetricIds = metricSelector.SelectMetrics(_kernel.Get<IPostMetricProvider>());

            using (var file = File.Open(metricsFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    var names = _kernel.Get<IPostMetricProvider>().Names.ToArray();
                    ConfigureWriter(writer);
                    foreach (var id in topMetricIds)
                    {
                        writer.WriteFields(names[id]);

                    }
                }
            }


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
            }

            WriteSentenceStats();
        }

        private static void WriteMetricsToFile(string fileName, IEnumerable<IEnumerable<int>> metrics, ISentenceMetricProvider setprovider = null)
        {
            var provider = setprovider?? _kernel.Get<ISentenceMetricProvider>();
            using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write))
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
            
            var provider = _kernel.Get<ISentenceMetricProvider>();
            var allMetrics = new List<IEnumerable<int>>();


            foreach (var author in _authors)
            {
                IEnumerable<int>[] metrics = _context.Posts.Where(x => x.Author.Id == author.Author.Id)
                                      .ToList().SelectMany(x => provider.GetMetrics(_dataExtractor.GetText(x)).ToArray()).ToArray();
                allMetrics.AddRange(metrics);


                var correlationTable = metrics.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray().CorrelationTablePearson(provider.Size);


                WriteMetricsToFile(author.Author.Identity + "-sent-stat.csv", metrics);

              

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

            WriteMetricsToFile("all-stat.csv", allMetrics);

            var newProvider = new SelectedSentenceMetricProvider(Enumerable.Range(0, provider.Size).Where(i => allMetrics.Select(x => x.ElementAt(i)).Distinct().Count() > 2).ToArray(),provider);

            var cutMetrics = _authors.SelectMany(author => _context.Posts.Where(x => x.Author.Id == author.Author.Id))
                    .ToList()
                    .SelectMany(x => newProvider.GetMetrics(_dataExtractor.GetText(x)).ToArray())
                    .ToArray();

            WriteMetricsToFile("all-cut-stat.csv", cutMetrics,newProvider);
        }
    }
}


