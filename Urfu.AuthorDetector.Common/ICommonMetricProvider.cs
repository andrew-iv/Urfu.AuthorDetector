using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetricProviderInfo
    {
        IEnumerable<string> Names { get; }
        int Size { get; }
    }
}