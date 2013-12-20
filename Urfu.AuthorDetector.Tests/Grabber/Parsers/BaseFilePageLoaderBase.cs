using System;
using System.IO;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    public class BaseFilePageLoaderBase : BasePageLoader
    {
        protected readonly string _filesPath;
        protected BaseFilePageLoaderBase(string filesPath)
        {
            _filesPath = filesPath;
        }

        public string LoadFormUrlPath { get; set; }

        protected virtual HtmlDocument LoadDocument(params object[] args)
        {
            try
            {
                var doc = new HtmlDocument();
                using (var stream = File.Open(
                    Path.Combine(
                        _filesPath, string.Format("{0}.html", string.Join("_", args))), FileMode.Open))
                {
                    doc.Load(stream);
                }
                return doc;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}