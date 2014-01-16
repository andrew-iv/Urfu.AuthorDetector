using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public interface IVBulletinParser
    {
        IEnumerable<UserInfo> Users(int count, int page = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="themIds"> Для Ачата - 6, 35 </param>
        /// <param name="searchId"></param>
        /// <param name="childForums">Включить дочерние форумы </param>
        /// <returns></returns>
        IEnumerable<PostBrief> Posts(string user, int[] themIds, out int searchId, out int total, bool childForums = true);

        IEnumerable<PostBrief> MorePosts(int searchId, int page, int pp);
        PostInfo FillPostInfo(PostBrief brief);
        IEnumerable<PostInfo> Showthread(ThemeInfo thread,int page, out int pageCount);
        IEnumerable<ThemeInfo> ForumDisplay(int forumId, int page, out int pageCount);
    }
}