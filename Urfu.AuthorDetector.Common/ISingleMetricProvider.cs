using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface ISingleMetricProvider : ICommonMetricProvider
    {
        double[] GetMetrics(IEnumerable<string> text);
    }
}