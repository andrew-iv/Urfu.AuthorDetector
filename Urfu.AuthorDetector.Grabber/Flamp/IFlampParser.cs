using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.Flamp
{
    public interface IFlampParser
    {
        IEnumerable<string> Users(string city, int pages);
        IEnumerable<PostInfo> Posts(string user);
    }
}