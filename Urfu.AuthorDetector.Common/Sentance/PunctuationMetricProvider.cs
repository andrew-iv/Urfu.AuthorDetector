using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public class PunctuationMetricProvider : BaseSentenceMetricProvider
    {
        private static readonly char[] _punctuations = new[] { '.', ',', '-', '!', '?', ':', '(', ')' };

        public override IEnumerable<double> GetSentenceMetrics(SentanceInfo si)
        {
            return _punctuations.Select(punc => (double)si.Sentence.Count(x => x == punc));
        }

        public override IEnumerable<string> Names
        {
            get { return _punctuations.Select(x => "Punctuation_" + x); }
        }
    }
}