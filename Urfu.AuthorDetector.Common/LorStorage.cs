using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common
{
    public abstract class ForumStorageBase : IForumStorage
    {
        private readonly IStatisticsContext _context;
        private Forum _forum;
        private HashSet<int> _storedIds;
        private IDictionary<string, Author> _authors;
        private IDictionary<long, Theme> _themes;

        protected abstract int ForumId { get; }

        [Inject]
        protected ForumStorageBase(IStatisticsContext context)
        {
            _context = context;
            var realContext = _context as StatisticsContainer;
            if (realContext != null)
            {
                realContext.Configuration.AutoDetectChangesEnabled = false;
                realContext.Configuration.ProxyCreationEnabled = false;
                realContext.Configuration.LazyLoadingEnabled = false;
            }
            ReinitializeCache();
        }

        private void ReinitializeCache()
        {

            _forum = _context.ForumSet.First(x => x.Id == ForumId);
            _storedIds = new HashSet<int>(_context.Posts.Where(x => x.Theme.Forum.Id == ForumId).Select(x => x.IdOnForum));
            _authors = _context.Authors.Where(x => x.Forum.Id == ForumId).ToDictionary(x => x.Identity);
            _themes = _context.Themes.Where(x => x.Forum.Id == ForumId).ToDictionary(x => x.IdOnForum);
        }

        public IQueryable<Post> GetPostsUser(string user)
        {
            return _context.Posts.Where(x => x.Author.Forum.Id == ForumId);
        }

        private Post ToPost(PostInfo postInfo, Author author, Theme theme)
        {
            return new Post()
                {
                    Author = author,
                    Text = postInfo.HtmlText,
                    DateTime = postInfo.Time,
                    IdOnForum = postInfo.PostId,
                    Theme = theme
                };
        }

        private Author CreateAuthor(PostInfo postInfo)
        {
            return new Author()
                {
                    Identity = postInfo.Nick.CutString(),
                    DisplayName = postInfo.Nick.CutString(),
                    Forum = _forum
                };
        }

        public void SavePosts(IEnumerable<PostInfo> posts)
        {
            foreach (var lorPostInfo in posts.Where(x => !_storedIds.Contains(x.PostId)))
            {
                Author author;
                if (!_authors.ContainsKey(lorPostInfo.Nick))
                {
                    author = CreateAuthor(lorPostInfo);
                    _context.Authors.Add(author);
                    _authors.Add(lorPostInfo.Nick, author);
                }
                else
                {
                    author = _authors[lorPostInfo.Nick];
                }

                Theme theme;
                if (!_themes.ContainsKey(lorPostInfo.ThemeId))
                {
                    theme = CreateTheme(lorPostInfo);
                    _context.Themes.Add(theme);
                    _themes.Add(lorPostInfo.ThemeId, theme);
                }
                else
                {
                    theme = _themes[lorPostInfo.ThemeId];
                }

                _context.Posts.Add(ToPost(lorPostInfo, author, theme));
            }
            _context.SaveChanges();
        }

        private Theme CreateTheme(PostInfo postInfo)
        {
            return new Theme
                {
                    Forum = _forum,
                    Title = postInfo.Theme.CutString(),
                    IdOnForum = postInfo.ThemeId
                };
        }

        public void FillBriefs(IEnumerable<PostBrief> postsBriefs)
        {

        }

        public void SavePost(PostInfo posts)
        {
            SavePosts(new PostInfo[] { posts });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class LorStorage : ForumStorageBase, IForumStorage
    {
        public const int LorId = 1;

        [Inject]
        public LorStorage(IStatisticsContext context) : base(context)
        {
        }

        protected override int ForumId
        {
            get { return LorId; }
        }
    }

    public class FlampStorage : ForumStorageBase, IForumStorage
    {
        
        [Inject]
        public FlampStorage(IStatisticsContext context)
            : base(context)
        {
        }

        protected override int ForumId
        {
            get { return 2; }
        }
    }
}