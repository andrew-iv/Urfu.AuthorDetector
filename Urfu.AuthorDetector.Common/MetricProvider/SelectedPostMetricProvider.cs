using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
   /* public class MetricAggregatorBase
    {
        abstract Aggregate
    }*/

    public class SelectedPostMetricProvider : BaseSelectedMetricProvider<IPostMetricProvider>, IPostMetricProvider
    {
        public SelectedPostMetricProvider(IPostMetricProvider baseProvider)
            : base(baseProvider)
        {
        }

        public double[] GetMetrics(string text)
        {
            var metr = BaseProvider.GetMetrics(text);
            return ((metr as double[]) ?? metr.ToArray()).GetOnIndexes(Indexes);
        }

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return text.Select(GetMetrics).ToArray();
        }
    }
}