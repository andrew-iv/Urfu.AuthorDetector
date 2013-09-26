using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class MetricNeighboorClassifier<TMetric> : BaseNeighboorClassifier where TMetric : IFillableMetric, new()
    {
        public MetricNeighboorClassifier(IDictionary<Author, IEnumerable<Post>> authors)
            : base(authors)
        {
        }

        protected override IMetric CallculateMetric(Post post)
        {
            var metric = new TMetric();
            metric.FillFromPost(post);
            return metric;
        }
    }
}