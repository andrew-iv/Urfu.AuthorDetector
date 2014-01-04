using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetricSelector
    {


        IEnumerable<int> SelectMetrics(IPostMetricProvider postMetricProvider);
    }
}