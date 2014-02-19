using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// Наивный байесовский классификатор(фабрика)
    /// </summary>
    public class StupidPerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new StupidPerecentileBayesClassifier(authors, CommonMetricProviders);
        }
    }
}