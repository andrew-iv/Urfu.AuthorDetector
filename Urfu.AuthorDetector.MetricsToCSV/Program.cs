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
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.MetricsToCSV
{
    internal class Program
    {
        private static void ConfigureWriter(CsvWriter writer)
        {
            writer.Configuration.QuoteAllFields = false;
        }

        public class MetricModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.Bind<IMetricProvider>().To<AllMetricProvider>().InThreadScope();
                /*Kernel.Bind<IMetricSelector>().ToConstructor(x=>new Chi2ForAuthorMetricSelector(x.Context.Request.Parameters))
                    (x=>new Chi2ForAuthorMetricSelector(x.Context.Parameters.,) ).InTransientScope();
                //Kernel.Bind<>().To<Chi2ForAuthorMetricSelector>().InThreadScope();*/
            }
        }

        private const int TopsCount = 300;
        private const int SelectParams = 2;

        private static void Main(string[] args)
        {
            const string topAuthorsFile = "TopAuthors.csv";
            const string metricsFile = "BestMetrics.csv";

            var appSettings = ConfigurationManager.AppSettings;
            var topCount = int.Parse(appSettings.Get("TopAuthors"));
            var startYear = int.Parse(appSettings.Get("StartYear"));
            var endYear = int.Parse(appSettings.Get("EndYear"));
            var kernel = new StandardKernel(new CommonModule(), new MetricModule());
            StaticVars.Kernel = kernel;

            var dataExtractor = kernel.Get<IDataExtractor>();
            IEnumerable<string>[] authorPosts;
            List<AuthorMinPosts> authors;
            using (var context = kernel.Get<IStatisticsContext>())
            {
                var filter = kernel.Get<IPostsQueryFilter>();
                var postsQuery = filter.FilterDate(filter.OnlyLor(context.Posts),
                                                   new DateTime(startYear, int.Parse(appSettings.Get("StartMonth")), 1),
                                                   new DateTime(endYear, 1, 1));
                authors = filter.TopAuthorsMinimum(postsQuery
                    ).Take(topCount).ToList();
                var authorIds = authors.Select(x => x.Author.Id).ToArray();
                var posts =
                    postsQuery.Where(x => authorIds.Contains(x.Author.Id)).Select(dataExtractor.GetText).ToArray();
                StaticVars.InitializeTops(posts, TopsCount);
                authorPosts = authors.Select(x => x.Author.Post.Select(dataExtractor.GetText).ToArray() as IEnumerable<string>)
                                     .ToArray();

            }
            GC.Collect();

            Console.WriteLine("Average Length - {0}",
                              authorPosts.SelectMany(x => x.Select(xx => xx.Length)).ToArray().Average());

            var metricSelector = new Chi2ForAuthorMetricSelector(authorPosts, SelectParams);
            var topMetricIds = metricSelector.SelectMetrics(kernel.Get<IMetricProvider>());

            using (var file = File.Open(metricsFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    var names = kernel.Get<IMetricProvider>().Names.ToArray();
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
                    foreach (var author in authors)
                    {
                        writer.WriteFields(author.Author.Identity, author.PostCount);

                    }
                }
            }


        }

    }
}


