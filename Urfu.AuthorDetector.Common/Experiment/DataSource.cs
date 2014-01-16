using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Experiment
{
    public interface IDataSource
    {
        IDictionary<Author, string[]> GetPosts(int topAuthors);
    }

    public class DataSource : IDataSource
    {
        public int? ForumId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public IDictionary<Author, string[]> GetPosts(int topAuthors)
        {
            var cont = StaticVars.Kernel.Get<IStatisticsContext>();
            var de = StaticVars.Kernel.Get<IDataExtractor>();
            var authors = ForumId.HasValue?cont.Authors.Where(x=>x.Forum.Id == ForumId) : cont.Authors;


            return authors.SelectMany(x =>
                                      x.Post.Where(post => !DateEnd.HasValue || post.DateTime <= DateEnd)
                                       .Where(post => !DateStart.HasValue || post.DateTime >= DateStart))
                          .GroupBy(x => x.Author)
                          .OrderByDescending(x => x.Count()).Take(topAuthors)
                          .ToDictionary(x => x.Key, x => x.Select(de.GetText).ToArray());
        }
    }
}