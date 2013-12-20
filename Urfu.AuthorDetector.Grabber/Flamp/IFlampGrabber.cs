using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.Flamp
{
    public interface IFlampGrabber
    {
        IEnumerable<PostInfo> LoadAllArchive(string city, int pages);
    }
}