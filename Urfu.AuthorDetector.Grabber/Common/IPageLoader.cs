using HtmlAgilityPack;

namespace Urfu.AuthorDetector.Grabber.Common
{
    public interface IPageLoader
    {
        HtmlDocument Load(string url, ILoadDocumentParameters parameters = null);
    }
}