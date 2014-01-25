using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface ICommonMetricProvider : IMetricProviderInfo
    {
        
        double[][] GetMetrics(IEnumerable<string> text);
    }

    public interface IMetricProviderInfo
    {
        IEnumerable<string> Names { get; }
        int Size { get; }
    }
}