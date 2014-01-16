using System.Collections.Generic;
using System.Linq;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public abstract class BaseSentenceMetricProvider : ISentenceMetricProvider
    {
        public abstract IEnumerable<double> GetSentenceMetrics(SentanceInfo text);

        public virtual IEnumerable<double[]> GetMetrics(string text)
        {
            return text.Sentenses().Where(x=>text.RussianWords().Count() > 1).Select(sent =>
                                                                                     GetSentenceMetrics(new SentanceInfo(sent)).ToArray()
                );
        }

        public abstract IEnumerable<string> Names { get; }

        public int Size {
            get { return Names.Count(); }
        }
    }
}