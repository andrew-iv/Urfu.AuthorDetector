using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public interface IPostMetricProvider : ICommonMetricProvider
    {

        double[] GetMetrics(string text);
    }
}