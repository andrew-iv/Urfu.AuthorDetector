using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public abstract class BaseClassifierFactory : IClassifierFactory
    {

        public ICommonMetricProvider[] CommonMetricProviders { get; set; }
        public abstract IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }
}