using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IClassifier : IAlogorithm
    {
        /// <summary>
        /// Записать результат в базу
        /// </summary>
        /// <param name="isSuccess"></param>
        void LogResult(bool isSuccess);

        /// <summary>
        /// На каких авторах обучен классификатор
        /// </summary>
        IEnumerable<Author> Authors { get; }

        /// <summary>
        /// Определить автора сообщений
        /// </summary>
        /// <param name="posts"></param>>
        /// <param name="reliable">Достоверный ли ответ?</param>
        /// <returns></returns>
        Author ClassificatePosts(IEnumerable<string> posts, out bool reliable);

        /// <summary>
        /// Определить возможных авторов сообщений
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        Author[] ClassificatePosts(IEnumerable<string> posts, int topN);

        
    }

    
}