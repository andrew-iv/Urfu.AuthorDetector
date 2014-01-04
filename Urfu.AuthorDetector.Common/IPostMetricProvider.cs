using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface ICommonMetricProvider
    {
        IEnumerable<string> Names { get; }
        int Size { get; }
    }

    public interface IPostMetricProvider : ICommonMetricProvider
    {

        IEnumerable<double> GetMetrics(string text);
    }
}