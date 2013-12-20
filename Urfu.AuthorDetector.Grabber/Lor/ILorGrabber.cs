using System.Collections.Generic;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber
{
    public interface ILorGrabber
    {
        IEnumerable<PostInfo> LoadAllArchive(int year, int month, string category = "talks");
    }
}