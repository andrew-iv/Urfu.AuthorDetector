using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public interface IForumStorage : IDisposable
    {
        IQueryable<Post> GetPostsUser(string user);
        void SavePosts(IEnumerable<PostInfo> posts);
        void FillBriefs(IEnumerable<PostBrief> postsBriefs);
        void SavePost(PostInfo posts);
    }

    public interface IPostsQueryFilter
    {
        IQueryable<Post> OnlyForum(IQueryable<Post> posts, int id = LorStorage.LorId);


        IQueryable<Post> OnlyAuthors(IQueryable<Post> posts, string[] authorIds);
        IQueryable<Post> FilterDate(IQueryable<Post> posts, DateTime? start, DateTime? end);
        IOrderedQueryable<AuthorMinPosts> TopAuthorsMinimum(IQueryable<Post> posts, int needInMonth = 50);
        IOrderedQueryable<AuthorMinPosts> TopAuthors(IQueryable<Post> posts, int needInMonth = 10);
    }

    public class AuthorMinPosts
    {
        public Author Author { get; set; }
        public int PostCount { get; set; }
        public int MonthsParticipaiting { get; set; }
    }

    class PostsQueryFilter : IPostsQueryFilter
    {
        public IQueryable<Post> OnlyForum(IQueryable<Post> posts, int id = LorStorage.LorId)
        {
            return posts.Where(x => x.Author.Forum.Id == id);
        }

        public IQueryable<Post> OnlyAuthors(IQueryable<Post> posts, string[] authorIds)
        {
            return posts.Where(x => authorIds.Contains(x.Author.Identity));
        }

        public IQueryable<Post> FilterDate(IQueryable<Post> posts, DateTime? start, DateTime? end)
        {
            var res = posts;
            if (start.HasValue)
                res = res.Where(x => x.DateTime >= start);
            if (end.HasValue)
                res = res.Where(x => x.DateTime < end);
            return res;
        }

        public IOrderedQueryable<AuthorMinPosts> TopAuthorsMinimum(IQueryable<Post> posts, int needInMonth = 10)
        {
            return posts.GroupBy(x => x.Author).Select(
                psts =>
                    new AuthorMinPosts()
                    {
                        PostCount = psts.Count(),
                        MonthsParticipaiting = psts.GroupBy(x => new { x.DateTime.Value.Year, x.DateTime.Value.Month }).Count(x=>x.Count()>=needInMonth),
                        Author = psts.Key
                    }
                ).OrderByDescending(x => x.MonthsParticipaiting).ThenByDescending(x=>x.PostCount);


           /* return posts.GroupBy(x => x.Author).Select(
                psts =>
                    new AuthorMinPosts()
                        {
                            PostCount = psts.GroupBy(x => new {x.DateTime.Value.Year, x.DateTime.Value.Month}).Min(x=>x.Count()),
                            Author = psts.Key
                        }
                ).OrderByDescending(x=>x.PostCount);*/
        }


        public IOrderedQueryable<AuthorMinPosts> TopAuthors(IQueryable<Post> posts, int needInMonth = 10)
        {
            return posts.GroupBy(x => x.Author).Select(
                psts =>
                    new AuthorMinPosts()
                    {
                        PostCount = psts.Count(),
                        MonthsParticipaiting = psts.GroupBy(x => new { x.DateTime.Value.Year, x.DateTime.Value.Month }).Count(x => x.Count() >= needInMonth),
                        Author = psts.Key
                    }
                ).OrderByDescending(x => x.PostCount);


            /* return posts.GroupBy(x => x.Author).Select(
                 psts =>
                     new AuthorMinPosts()
                         {
                             PostCount = psts.GroupBy(x => new {x.DateTime.Value.Year, x.DateTime.Value.Month}).Min(x=>x.Count()),
                             Author = psts.Key
                         }
                 ).OrderByDescending(x=>x.PostCount);*/
        }

    }
}