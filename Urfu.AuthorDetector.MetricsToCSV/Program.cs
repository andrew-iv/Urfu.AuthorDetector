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
    class Program
    {
        static void ConfigureWriter(CsvWriter writer)
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

        private const int TopsCount = 500;
        private const int SelectParams = 5;

        private static void Main(string[] args)
        {
            const string topAuthorsFile = "TopAuthors.csv";
            const string metricsFile = "BestMetrics.csv";

            var appSettings = ConfigurationManager.AppSettings;
            var topCount = int.Parse(appSettings.Get("TopAuthors"));
            var startYear = int.Parse(appSettings.Get("StartYear"));
            var endYear = int.Parse(appSettings.Get("EndYear"));
            var kernel = new StandardKernel(new CommonModule(),new MetricModule());
            var dataExtractor = kernel.Get<IDataExtractor>();
            using (var context = kernel.Get<IStatisticsContext>())
            {
                var filter = kernel.Get<IPostsQueryFilter>();
                var postsQuery = filter.FilterDate(filter.OnlyLor(context.Posts),
                                                   new DateTime(startYear, int.Parse(appSettings.Get("StartMonth")), 1),
                                                   new DateTime(endYear, 1, 1));
                var authors =
                    filter.TopAuthorsMinimum(postsQuery
                        ).Take(topCount).ToList();
                var authorIds = authors.Select(x => x.Author.Id).ToArray();
                var posts = postsQuery.Where(x => authorIds.Contains(x.Author.Id)).Select(dataExtractor.GetText).ToArray();
                StaticVars.InitializeTops(posts,TopsCount);

                var authorPosts =
                    authors.Select(x => x.Author.Post.Select(dataExtractor.GetText).ToArray() as IEnumerable<string>)
                           .ToArray();

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
                /*
                List<TrivialMetric> allStat = new List<TrivialMetric>();
                foreach (var author in authors)
                {
                    using (var file = File.Open(author.Author.Identity + "-stat.csv", FileMode.Create, FileAccess.Write))
                    using (var writer = new CsvWriter(new StreamWriter(file)))
                    {
                        ConfigureWriter(writer);
                        writer.WriteFields("Length", "Time",
                                           "ParagraphsShare", "PunctuationShare", "WhitespacesShare", "OtherNodesShare");
                        foreach (
                            var post in
                                context.Posts.Where(x => x.Author.Id == author.Author.Id)
                                       .Where(x => x.Text.Contains("<p>"))
                                       .ToList())
                        {
                            var stat = new TrivialMetric(post.Text);
                            allStat.Add(stat);
                            writer.WriteFields(stat.Length, stat.Time, stat.ParagraphsShare, stat.PunctuationShare,
                                               stat.WhitespacesShare, stat.OtherNodesShare);

                        }
                    }
                }

                using (var file = File.Open("all-stat.csv", FileMode.Create, FileAccess.Write))
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    ConfigureWriter(writer);
                    writer.WriteFields("Length", "Time",
                                       "ParagraphsShare", "PunctuationShare", "WhitespacesShare", "OtherNodesShare");
                    foreach (var stat in allStat)
                    {
                        writer.WriteFields(stat.Length, stat.Time, stat.ParagraphsShare, stat.PunctuationShare,
                                           stat.WhitespacesShare, stat.OtherNodesShare);

                    }
                */
            }
        }
    }
}


