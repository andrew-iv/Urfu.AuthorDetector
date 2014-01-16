using System.Collections.Generic;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IClassifierFactory
    {
        IPostMetricProvider PostMetricProvider { get; set; }
        IMultiplyMetricsProvider MultiplyMetricsProvider { get; set; }
        IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    abstract class BaseClassifierFactory: IClassifierFactory
    {
        public IPostMetricProvider PostMetricProvider { get; set; }
        public IMultiplyMetricsProvider MultiplyMetricsProvider { get; set; }
        public abstract IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    class PerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new PerecentileBayesClassifier(authors,PostMetricProvider,MultiplyMetricsProvider);
        }
    }

    class StupidPerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new StupidPerecentileBayesClassifier(authors, PostMetricProvider, MultiplyMetricsProvider);
        }
    }
}