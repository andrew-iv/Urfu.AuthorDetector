using System;
using HtmlAgilityPack;

namespace Urfu.AuthorDetector.Grabber
{
    public class LorPageLoader:ILorPageLoader
    {
        private static string GetPostsListUrl(string nick, int offset)
        {
            return String.Format(LorGrabber.LorUrl + "show-comments.jsp?nick={0}&offset={1}", nick, offset);
        }

        private static string GetThemeUrlByPostId(int themeId, int postId)
        {
            return String.Format(LorGrabber.LorUrl + "forum/talks/{0}?cid={1}", themeId,postId);
        }

        public HtmlDocument LoadPostsList(string nick, int offset)
        {
            return UrlRetriever.GetHtmlDocSync(GetPostsListUrl(nick, offset));
        }

        public HtmlDocument LoadThemeByPostId(int themeId, int postId)
        {
            return UrlRetriever.GetHtmlDocSync(GetThemeUrlByPostId(themeId, postId));
        }

        public HtmlDocument LoadTheme(int themeId, int page)
        {
            return UrlRetriever.GetHtmlDocSync(GetThemeUrl(themeId, page));
        }

        public HtmlDocument LoadArchive(int year, int month, int offset, string category)
        {
            //http://www.linux.org.ru/forum/talks/2012/3/
            return
                UrlRetriever.GetHtmlDocSync(
                String.Format(LorGrabber.LorUrl + "forum/{2}/{0}/{1}/?offset={3}", year, month, category,offset));
        }

        public HtmlDocument Load(string url)
        {
            return
                UrlRetriever.GetHtmlDocSync(url);
        }

        private string GetThemeUrl(int themeId, int page,string category="talks")
        {
            return String.Format(LorGrabber.LorUrl + "forum/{2}/{0}/page{1}", themeId, page,category);
        }
    }
}