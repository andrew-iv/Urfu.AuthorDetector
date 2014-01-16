using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public interface IMultiplyMetricsProvider:ICommonMetricProvider
    {
        IEnumerable<double[]> GetMetrics(string text);
    }

    public interface ISentenceMetricProvider : IMultiplyMetricsProvider
    {
        
        IEnumerable<double> GetSentenceMetrics(SentanceInfo text);
    }
}