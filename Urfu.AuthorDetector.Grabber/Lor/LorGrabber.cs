using System;
using System.Collections.Generic;
using Urfu.AuthorDetector.Common;
using System.Linq;

namespace Urfu.AuthorDetector.Grabber.Lor{
    

    public class LorGrabber : ILorGrabber
    {
        private readonly ILorPostsParser _postsParser;
        public const string LorUrl = "http://www.linux.org.ru/";

        public LorGrabber(ILorPostsParser postsParser)
        {
            _postsParser = postsParser;
        }

        public IEnumerable<PostInfo> LoadAllArchive(int year, int month, string category = "talks")
        {
            const int perPageCount = 30;
            var offset = 0;
            var linksToParse = new List<string>();
            string[] links;
            while ( (links = _postsParser.ParseThemeLinks(offset, year, month, category).ToArray()).Length > 0)
            {
                linksToParse.AddRange(links);
                offset += perPageCount;
            }
            return linksToParse.SelectMany(link =>
                {
                    try
                    {
                        return _postsParser.ParseComments(link);
                    }
                    catch (Exception)
                    {
                        return Enumerable.Empty<PostInfo>();
                    }
                });
        }
    }
}