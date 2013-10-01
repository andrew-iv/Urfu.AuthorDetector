using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public interface IDataExtractor
    {
        string GetText(Post post);
    }
}