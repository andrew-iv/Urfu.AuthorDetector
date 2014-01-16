using System;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Grabber.Common
{
    public interface ILoadDocumentParameters
    {
        Encoding Encoding { get;}
        CookieContainer CookieContainer { get; }
    }


    class LoadDocumentParameters : ILoadDocumentParameters
    {
        public Encoding Encoding { get; set; }
        public CookieContainer CookieContainer{ get; set; }

        public LoadDocumentParameters(ILoadDocumentParameters bs)
        {
            Encoding = bs.Encoding;
            CookieContainer = bs.CookieContainer;
        }

        public LoadDocumentParameters()
        {
        }
    }

    public abstract class BasePageLoader:IPageLoader
    {

        private readonly CookieContainer _cookieContainer = new CookieContainer();

        protected virtual ILoadDocumentParameters DefaultParameters
        {
            get
            {
                return
                    new LoadDocumentParameters() { Encoding = Encoding.UTF8, CookieContainer = _cookieContainer };
            }}

        private HtmlDocument GetHtmlDocSyncuExc(string url, ILoadDocumentParameters parameters)
        {
            var defaultParams = DefaultParameters;


            var req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = parameters.CookieContainer;
            req.UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
            using (var resp = req.GetResponse())
            {
                var doc = new HtmlDocument();
                doc.Load(resp.GetResponseStream(), parameters.Encoding ?? defaultParams.Encoding);
                return doc;
            }
        }

        public HtmlDocument Load(string url, ILoadDocumentParameters parameters = null)
        {
            if (parameters == null)
            {
                parameters = DefaultParameters;
            }
            Func<HtmlDocument> loaderMethod = () => GetHtmlDocSyncuExc(url, parameters);

            try
            {
                return loaderMethod();
            }
            catch (Exception)
            {
                try
                {
                    Thread.Sleep(200);
                    return loaderMethod();
                }
                catch (Exception)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        return loaderMethod();
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