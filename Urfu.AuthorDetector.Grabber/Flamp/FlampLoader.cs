using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Grabber.Flamp
{
    public interface IFlampLoader
    {
        HtmlDocument LoadUsers(string city, int page=1);
        HtmlDocument LoadUser(string nick);
    }

    public class FlampLoader:BasePageLoader, IFlampLoader
    {
        public const string Site = "flamp.ru";

        public HtmlDocument LoadUsers(string city, int page=1)
        {
            return Load(string.Format("http://{0}." + Site + "/experts?page={1}", city, page));
        }

        public HtmlDocument LoadUser(string nick)
        {
            return Load(string.Format("http://" + Site + "/{0}?from=1999999999&limit=10000", nick));
        }
    }
}