using HtmlAgilityPack;

namespace Urfu.AuthorDetector.Grabber
{
    public interface  ILorPageLoader
    {
        HtmlDocument LoadPostsList(string nick, int offset);
        HtmlDocument LoadThemeByPostId(int themeId, int postId);
        HtmlDocument LoadTheme(int themeId, int page);
        HtmlDocument LoadArchive(int year, int month, int offset, string category="talks");
        HtmlDocument Load(string url);
    }
}   