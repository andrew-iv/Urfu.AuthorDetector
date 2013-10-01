using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetricProvider
    {
        IEnumerable<string> Names { get; }
        int Size { get; }
        IEnumerable<double> GetMetrics(string text);
    }
}