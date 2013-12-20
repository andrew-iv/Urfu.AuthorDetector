using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Grabber.Lor
{
    public interface  ILorPageLoader : IPageLoader
    {
        HtmlDocument LoadPostsList(string nick, int offset);
        HtmlDocument LoadThemeByPostId(int themeId, int postId);
        HtmlDocument LoadTheme(int themeId, int page);
        HtmlDocument LoadArchive(int year, int month, int offset, string category="talks");
    }
}   