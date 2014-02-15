using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IBinaryClassifierFactory
    {
        double ErrorLevel { get; set; }
        ICommonMetricProvider[] CommonMetricProviders { get; set; }
        IBinaryClassifier Create(IDictionary<Author, IEnumerable<string>> authors, Author author);
    }
}