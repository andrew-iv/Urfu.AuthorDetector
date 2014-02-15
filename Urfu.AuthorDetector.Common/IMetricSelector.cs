using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetricSelector
    {


        Dictionary<int, HashSet<int>> SelectMetrics(ICommonMetricProvider postMetricProvider);
    }
}