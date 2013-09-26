using System;
using System.IO;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    public class FileLorPageLoader : ILorPageLoader
    {
        private readonly string _filesPath;
        protected const string FilesPath = "Grabber\\Files\\";

        public string LoadFormUrlPath { get; set; }

        public FileLorPageLoader(string filesPath = FilesPath)
        {
            _filesPath = filesPath;
        }

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

        public virtual HtmlDocument LoadPostsList(string nick, int offset)
        {

            return LoadDocument("list", nick, offset);

        }

        public virtual HtmlDocument LoadThemeByPostId(int themeId, int postId)
        {
            return LoadDocument("post", themeId);
        }

        public HtmlDocument LoadTheme(int themeId, int page)
        {
            return LoadDocument("post", themeId, page);
        }

        public HtmlDocument LoadArchive(int year, int month, int offset, string category)
        {
            return LoadDocument("ar", category, year,month, offset);
        }

        public HtmlDocument Load(string url)
        {
            return LoadDocument(LoadFormUrlPath);
        }
    }
}