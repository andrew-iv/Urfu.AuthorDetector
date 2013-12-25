using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public class VBulletinGrabber
    {
        private readonly IVBulletinParser _parser;

        public VBulletinGrabber(IVBulletinParser parser)
        {
            _parser = parser;
        }

        private const int PageSize = 10;

        public IEnumerable<PostInfo> LoadMembers(int pages, int[] forums,int startPage)
        {
            var members = Enumerable.Range(startPage, pages).SelectMany(i => _parser.Users(PageSize, i)).ToArray();
            return members.SelectMany(user =>
                {
                    var res = UserPosts(user, forums).ToArray();
                    Console.WriteLine(user.Nick + " - stored");
                    return res;
                });
        }

        private IEnumerable<PostInfo> UserPosts(UserInfo user, int[] forums)
        {
            int searchId;
            int total;
            var posts = _parser.Posts(user.Nick, forums, out searchId, out total).ToArray();
            if (!posts.Any()) yield break;
            for (int page = 1; page <= (total + PageSize - 1) / PageSize ; page++)
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