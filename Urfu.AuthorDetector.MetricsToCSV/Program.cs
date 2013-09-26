using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Ninject;
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

        private static void Main(string[] args)
        {
            const string topAuthorsFile = "TopAuthors.csv";

            var appSettings = ConfigurationManager.AppSettings;
            var topCount = int.Parse(appSettings.Get("TopAuthors"));
            var startYear = int.Parse(appSettings.Get("StartYear"));
            var endYear = int.Parse(appSettings.Get("EndYear"));
            var kernel = new StandardKernel(new CommonModule());
            using (var context = kernel.Get<IStatisticsContext>())
            {
                var filter = kernel.Get<IPostsQueryFilter>();
                var authors =
                    filter.TopAuthorsMinimum(
                        filter.FilterDate(filter.OnlyLor(context.Posts), new DateTime(startYear, 1, 1),
                                          new DateTime(endYear, 1, 1))).Take(topCount).ToList();

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
                            var stat = new TrivialMetric(post);
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
                }
            }
        }
    }
}


