using System;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Grabber.Flamp;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    public class FlampFileLoader : BaseFilePageLoaderBase, IFlampLoader
    {
        protected const string FilesPath = "Grabber\\FilesFlamp\\";

        public FlampFileLoader(string filesPath = FilesPath)
            : base(filesPath)
        {
        }

        public HtmlDocument LoadUsers(string city, int page = 1)
        {
            return LoadDocument("experts", city, page);
        }

        public HtmlDocument LoadUser(string nick)
        {
            return LoadDocument("user", nick);
        }

        public HtmlDocument LoadUser(string nick, int first)
        {
            return LoadDocument("user", nick, first);
        }
    }

    public class FileLorPageLoader : BaseFilePageLoaderBase, ILorPageLoader
    {

        protected const string FilesPath = "Grabber\\Files_Lor\\";

        public FileLorPageLoader(string filesPath)
            : base(filesPath)
        {
        }

        public FileLorPageLoader()
            : base(FilesPath)
        {
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
            return LoadDocument("ar", category, year, month, offset);
        }


    }
}