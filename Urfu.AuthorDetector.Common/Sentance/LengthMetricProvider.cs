using System.Collections.Generic;

namespace Urfu.AuthorDetector.Common.Sentance
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