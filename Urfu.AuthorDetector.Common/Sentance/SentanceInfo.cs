using System.Collections.Generic;
using System.Linq;
using Ninject;
using Opcorpora.Dictionary;
using Opcorpora.Dictionary.Xsd;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public class SentanceInfo
    {
        public string Sentence { get; private set; }
        public int Length { get { return Words.Length; } }
        public lemmaItemG[][] Grammemes { get; private set; }
        public string[] Words { get; private set; }
        public IDictionary<char, int> PunctuationsStats { get; private set; }

        public SentanceInfo(string sentence)
        {
            Sentence = sentence;
            var dict = StaticVars.Kernel.Get<IOpcorporaDictionary>();
            Words = sentence.RussianWords().ToArray();
            Grammemes = Words.Select(word => dict.GetLemma(word).
                                                  SelectMany(x => x.
                                                                      GetlemmaItemGs()).ToArray()).ToArray();
            PunctuationsStats = sentence.Where(char.IsPunctuation).GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        }
    }
}