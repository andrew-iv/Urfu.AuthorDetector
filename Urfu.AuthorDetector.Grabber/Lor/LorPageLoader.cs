using System;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Grabber.Lor
{
    public class LorPageLoader: BasePageLoader, ILorPageLoader
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
            return Load(GetPostsListUrl(nick, offset));
        }

        public HtmlDocument LoadThemeByPostId(int themeId, int postId)
        {
            return Load(GetThemeUrlByPostId(themeId, postId));
        }

        public HtmlDocument LoadTheme(int themeId, int page)
        {
            return Load(GetThemeUrl(themeId, page));
        }

        public HtmlDocument LoadArchive(int year, int month, int offset, string category)
        {
            //http://www.linux.org.ru/forum/talks/2012/3/
            return
                Load(
                String.Format(LorGrabber.LorUrl + "forum/{2}/{0}/{1}/?offset={3}", year, month, category,offset));
        }

        private string GetThemeUrl(int themeId, int page,string category="talks")
        {
            return String.Format(LorGrabber.LorUrl + "forum/{2}/{0}/page{1}", themeId, page,category);
        }
    }
}