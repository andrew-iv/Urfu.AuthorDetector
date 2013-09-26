using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common
{
    public class LorStorage : ILorStorage
    {
        private readonly IStatisticsContext _context;
        public const int LorId = 1;
        private Forum _lorForum;
        private HashSet<int> _storedIds;
        private IDictionary<string, Author> _authors;
        private IDictionary<int, Theme> _themes;

        private void ReinitializeCache()
        {

            _lorForum = _context.ForumSet.First(x => x.Id == LorId);
            _storedIds = new HashSet<int>(_context.Posts.Where(x => x.Theme.Forum.Id == LorId).Select(x => x.IdOnForum));
            _authors = _context.Authors.Where(x => x.Forum.Id == LorId).ToDictionary(x => x.Identity);
            _themes = _context.Themes.Where(x => x.Forum.Id == LorId).ToDictionary(x => x.IdOnForum);
        }

        [Inject]
        public LorStorage(IStatisticsContext context)
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

        public IQueryable<Post> GetPostsUser(string user)
        {
            return _context.Posts.Where(x=>x.Author.Forum.Id == LorId);
        }

        private Post ToPost(LorPostInfo lorPostInfo, Author author, Theme theme)
        {
            return new Post()
                {
                    Author = author,
                    Text = lorPostInfo.HtmlText,
                    DateTime = lorPostInfo.Time,
                    IdOnForum = lorPostInfo.PostId,
                    Theme = theme
                };
        }

        private Author CreateAuthor(LorPostInfo lorPostInfo)
        {
            return new Author()
                {
                    Identity = lorPostInfo.Nick.CutString(),
                    DisplayName = lorPostInfo.Nick.CutString(),
                    Forum = _lorForum
                };
        }

        public void SavePosts(IEnumerable<LorPostInfo> posts)
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

        private Theme CreateTheme(LorPostInfo lorPostInfo)
        {
            return new Theme
                {
                    Forum = _lorForum,
                    Title = lorPostInfo.Theme.CutString(),
                    IdOnForum = lorPostInfo.ThemeId
                };
        }

        public void FillBriefs(IEnumerable<LorPostBrief> postsBriefs)
        {

        }

        public void SavePost(LorPostInfo posts)
        {
            SavePosts(new LorPostInfo[] { posts });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}