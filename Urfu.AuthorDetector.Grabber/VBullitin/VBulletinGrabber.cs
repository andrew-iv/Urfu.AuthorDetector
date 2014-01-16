using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public interface IVBulletinLog
    {
        void ThreadStored(ThemeInfo info);
    }

    public class VBulletinLog : IVBulletinLog
    {
        public void ThreadStored(ThemeInfo info)
        {
            Console.WriteLine("Скачана тема '{0}' id = {1} author = {2}", info.Title, info.Id, info.Owner);
        }
    }

    public class VBulletinGrabber
    {
        private readonly IVBulletinParser _parser;
        private readonly IVBulletinLog _log;

        public VBulletinGrabber(IVBulletinParser parser, IVBulletinLog log)
        {
            _parser = parser;
            _log = log;
        }

        private const int PageSize = 10;

        public IEnumerable<PostInfo> LoadMembers(int pages, int[] forums, int startPage)
        {
            var members = Enumerable.Range(startPage, pages).SelectMany(i => _parser.Users(PageSize, i)).ToArray();
            return members.SelectMany(user =>
                {
                    var res = UserPosts(user, forums).ToArray();
                    Console.WriteLine(user.Nick + " - stored");
                    return res;
                });
        }

        public IEnumerable<PostInfo> LoadForum(int forumId, int startPages = 2, int maxPage = 999999999, ISet<long> ignoreThemes = null)
        {
            if (startPages < 2) startPages = 2;
            int ignoreInt;
            int pageCount;
            if(ignoreThemes == null)
                ignoreThemes = new SortedSet<long>();
            return _parser.ForumDisplay(forumId, 1, out pageCount)
                                .Concat(
                                    Enumerable.Range(startPages, Math.Min(pageCount - startPages + 1, maxPage - 1))
                                              .SelectMany(i => _parser.ForumDisplay(forumId, i, out ignoreInt)))
                                .ToArray().
            SelectMany(info =>
                {
                    if (
                        ignoreThemes.Contains(info.Id))
                        return Enumerable.Empty<PostInfo>();
                    ignoreThemes.Add(info.Id);
                    var posts = GetPosts(info).ToArray();
                    _log.ThreadStored(info);
                    return posts;
                }).GroupBy(x => x.PostId).Select(x => x.First());

        }

        private IEnumerable<PostInfo> GetPosts(ThemeInfo theme)
        {


            int ignoreInt;
            int pageCount;
            return _parser.Showthread(theme, 1, out pageCount)
                                .Concat(
                                    Enumerable.Range(2, pageCount - 1)
                                              .SelectMany(i => _parser.Showthread(theme, i, out ignoreInt)))
                                .ToArray();

        }


        private IEnumerable<PostInfo> UserPosts(UserInfo user, int[] forums)
        {
            int searchId;
            int total;
            var posts = _parser.Posts(user.Nick, forums, out searchId, out total).ToArray();
            if (!posts.Any()) yield break;
            for (int page = 1; page <= (total + PageSize - 1) / PageSize; page++)
            {
                PostBrief[] postBriefs;
                try
                {
                    postBriefs = _parser.MorePosts(searchId, page, PageSize).ToArray();
                }
                catch (Exception)
                {
                    continue;
                }
                foreach (var brief in postBriefs)
                {
                    brief.Nick = user.Nick;
                    brief.UserId = user.IdOnForum;
                    PostInfo res = null;
                    try
                    {
                        res = _parser.FillPostInfo(brief);
                    }
                    catch
                    {

                    }
                    if (res != null)
                        yield return res;

                }

            }
        }
    }
}