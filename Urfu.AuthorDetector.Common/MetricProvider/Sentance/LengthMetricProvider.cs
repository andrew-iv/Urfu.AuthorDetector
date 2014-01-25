using System.Collections.Generic;
using Urfu.AuthorDetector.Common.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider.Sentance
{
    public class LengthMetricProvider : BaseSentenceMetricProvider
    {
        public override IEnumerable<double> GetSentenceMetrics(SentanceInfo si)
        {
            yield return si.Length;
        }

        public override IEnumerable<string> Names { get { return new string[] {"Length"}; } }
    }
}