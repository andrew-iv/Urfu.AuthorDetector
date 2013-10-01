using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IClassifier
    {
        IEnumerable<Author> Authors { get; }
        Author ClassificatePosts(IEnumerable<string> posts);
        string Description { get; }
        string Name { get; }
    }
}