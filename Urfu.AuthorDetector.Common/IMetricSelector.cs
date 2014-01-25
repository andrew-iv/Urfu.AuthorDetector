using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetricSelector
    {


        IEnumerable<int> SelectMetrics(IPostMetricProvider postMetricProvider);
    }
}