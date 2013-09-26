using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber
{
    public interface ILorPostsParser
    {
        IEnumerable<LorPostBrief> GetPostsList(string nick, int offset = 0);
        IEnumerable<string> ParseThemeLinks(int offset,int year,int month, string category = "talks");
        IEnumerable<LorPostInfo> ParsePostsInTheme(int themeId, int page, out bool haveNextPage);
        LorPostInfo FillPost(LorPostBrief postBrief);
        IEnumerable<LorPostInfo> ParseComments(string url);
    }
}