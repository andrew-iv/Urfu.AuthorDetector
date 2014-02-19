using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// байесовский классификатор с зависимостями(фабрика)
    /// </summary>
    public class PerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new PerecentileBayesClassifier(authors, CommonMetricProviders);
        }
    }
}