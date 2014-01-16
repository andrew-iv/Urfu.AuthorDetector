using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public class CombinedMetricProvider : BaseSentenceMetricProvider
    {
        private readonly ISentenceMetricProvider[] _providers;
        private readonly string[] _names;

        public CombinedMetricProvider(params ISentenceMetricProvider[] providers)
        {
            _providers = providers;
            _names = _providers.SelectMany(x => x.Names).ToArray();
        }


        public override IEnumerable<string> Names
        {
            get { return _names; }
        }



        public override IEnumerable<double> GetSentenceMetrics(SentanceInfo text)
        {
            return _providers.SelectMany(x => x.GetSentenceMetrics(text)).ToArray();
        }
    }
}