using System.Collections.Generic;
using Urfu.AuthorDetector.Common.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider.Sentance
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