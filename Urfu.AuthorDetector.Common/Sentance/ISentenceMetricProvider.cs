using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public interface ISentenceMetricProvider :ICommonMetricProvider
    {
        IEnumerable<IEnumerable<int>> GetMetrics(string text);
    }
}