using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IClassifier
    {
        IEnumerable<Author> Authors { get; }

        /// <summary>
        /// Определить автора сообщений
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        Author ClassificatePosts(IEnumerable<string> posts);

        /// <summary>
        /// Определить возможных авторов сообщений
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        Author[] ClassificatePosts(IEnumerable<string> posts, int topN);

        string Description { get; }
        string Name { get; }
    }

    
}