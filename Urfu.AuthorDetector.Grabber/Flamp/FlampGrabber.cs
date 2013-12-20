using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.Flamp
{
    public class FlampGrabber: IFlampGrabber
    {
        private readonly IFlampParser _postsParser;
        public const string LorUrl = "http://www.linux.org.ru/";

        public FlampGrabber(IFlampParser postsParser)
        {
            _postsParser = postsParser;
        }

        public IEnumerable<PostInfo> LoadAllArchive(string city, int pages)
        {
            return _postsParser.Users(city, pages).SelectMany(user =>
                    _postsParser.Posts(user)
                );
        }
    }
}