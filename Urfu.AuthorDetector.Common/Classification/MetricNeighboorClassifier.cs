using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /*
    public class MetricNeighboorClassifier<TMetric> : NeighboorClassifier where TMetric : IFillableMetric, new()
    {
        public MetricNeighboorClassifier(IDictionary<Author, IEnumerable<string>> authors)
            : base(authors)
        {
        }

        protected override IMetric CallculateMetric(string post)
        {
            var metric = new TMetric();
            metric.FillFromPost(post);
            return metric;
        }
    }*/
}