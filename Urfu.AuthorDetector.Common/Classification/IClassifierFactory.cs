using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
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

    public abstract class BaseClassifierFactory : IClassifierFactory
    {

        public ICommonMetricProvider CommonMetricProvider { get; set; }
        public IPostMetricProvider PostMetricProvider { get; set; }
        public IMultiplyMetricsProvider MultiplyMetricsProvider { get; set; }
        public abstract IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    public class PerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new PerecentileBayesClassifier(authors,PostMetricProvider,MultiplyMetricsProvider);
        }
    }

    public class StupidPerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new StupidPerecentileBayesClassifier(authors, PostMetricProvider, MultiplyMetricsProvider);
        }
    }

    public class MSvmClassifierClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new MSvmClassifier(authors, CommonMetricProvider);
        }
    }
}
