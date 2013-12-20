using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber
{
    public interface ILorPostsParser
    {
        IEnumerable<PostBrief> GetPostsList(string nick, int offset = 0);
        IEnumerable<string> ParseThemeLinks(int offset,int year,int month, string category = "talks");
        IEnumerable<PostInfo> ParsePostsInTheme(int themeId, int page, out bool haveNextPage);
        PostInfo FillPost(PostBrief postBrief);
        IEnumerable<PostInfo> ParseComments(string url);
    }
}