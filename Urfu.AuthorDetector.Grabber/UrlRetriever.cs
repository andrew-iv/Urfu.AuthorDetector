using System;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Grabber
{
    public static class UrlRetriever
    {
        private static HtmlDocument GetHtmlDocSyncuExc(string url, Encoding encoding = null)
        {
            var req = WebRequest.Create(url);
            using (var resp = req.GetResponse())
            {
                var doc = new HtmlDocument();
                doc.Load(resp.GetResponseStream(), encoding ?? Encoding.UTF8);
                return doc;
            }
        }


        public static HtmlDocument GetHtmlDocSync(string url, Encoding encoding = null)
        {
            try
            {
                return GetHtmlDocSyncuExc(url, encoding);
            }
            catch (Exception)
            {
                try
                {
                    Thread.Sleep(200);
                    return GetHtmlDocSyncuExc(url, encoding);
                }
                catch (Exception)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        return GetHtmlDocSyncuExc(url, encoding);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }
    }
}