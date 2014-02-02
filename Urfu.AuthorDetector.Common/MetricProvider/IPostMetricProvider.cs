using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public interface IPostMetricProvider : ICommonMetricProvider
    {

        double[] GetMetrics(string text);
    }

    public interface ICommonMetricProvider : IMetricProviderInfo
    {

        double[][] GetMetrics(IEnumerable<string> text);
    }
}