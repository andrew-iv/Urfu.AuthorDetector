using System;
using System.Collections.Generic;
using System.Linq;
using EeekSoft.Text;
using Ninject;
using Opcorpora.Dictionary;
using Opcorpora.Dictionary.Xsd;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common
{
    public abstract class BaseMetricProvider : IMetricProvider
    {
        protected abstract string[] UseNgramms { get; }
        protected abstract string[] UseWords { get; }
        protected abstract string[] Grammemes { get; }


        public IEnumerable<string> Names
        {
            get
            {
                return new string[] { "Length", "PunctuationShare", "WhitespacesShare", 
                    "UpperShare", "DigitShare","VovelShare","NewLinesShare" }.Concat((UseNgramms ?? new string[] { }).Select(x => "Top3Gramms_" + x))
                                                                             .Concat((UseWords ?? new string[] { }).Select(x => "TopRuWords_" + x))
                                                                             .Concat((Grammemes ?? new string[] { }).Select(x=>"Grammeme_"+x));
            }
        }
        public int Size
        {
            get
            {
                return TrivialCount +
                       (UseNgramms != null ? UseNgramms.Length : 0) +
                       (UseWords != null ? UseWords.Length : 0) +
                       (Grammemes != null ? Grammemes.Length : 0)
                    ;
            }
        }



        private const int TrivialCount = 7;
        public IEnumerable<double> GetMetrics(string text)
        {

            //Trivial
            var length = text.Length;
            yield return length;
            if (Math.Abs(length - 0d) > 0.1d)
            {
                yield return Convert.ToDouble(text.Count(Char.IsPunctuation)) / length;
                yield return Convert.ToDouble(text.Count(Char.IsWhiteSpace)) / length;
                yield return Convert.ToDouble(text.Count(Char.IsUpper)) / length;
                yield return Convert.ToDouble(text.Count(Char.IsDigit)) / length;
                yield return Convert.ToDouble(text.VowelCount()) / length;
                yield return Convert.ToDouble(text.Count(x => x == '\n')) / length;
            }
            else
            {
                foreach (var i in Enumerable.Range(0, TrivialCount - 1))
                {
                    yield return 0d;
                }
            }

            if (UseNgramms != null)
            {
                foreach (var metrics in SubstringMetrics(UseNgramms, text, length))
                {
                    yield return metrics;
                }
            }

            if (UseWords != null)
            {
                foreach (var metrics in SubstringMetrics(UseWords, text, length))
                {
                    yield return metrics;
                }
            }

            if (Grammemes != null)
            {
                foreach (var metrics in GrammemesMetrics(Grammemes, text))
                {
                    yield return metrics;
                }
            }
        }

        private IEnumerable<double> GrammemesMetrics(string[] grammemes, string text)
        {
            var dictionary = StaticVars.Kernel.Get<IOpcorporaDictionary>();
            var ruWords = text.RussianWords().ToArray();
            if (ruWords.Length == 0)
            {
                return grammemes.Select(x => 0d);
            }

            var counts = new Dictionary<string, int>();
            foreach (var lemma in ruWords.SelectMany(dictionary.GetLemma))
            {
                foreach (var gr in lemma.Parent.l.g.Union(lemma.g??new lemmaItemG[]{}))
                {
                    if (counts.ContainsKey(gr.v))
                        counts[gr.v]++;
                    else
                        counts[gr.v] = 1;
                }
            }
            return grammemes.Select(x => counts.ContainsKey(x) ? ((double)counts[x] / ruWords.Length) : 0d).ToArray();
        }

        private IEnumerable<double> SubstringMetrics(string[] keywords, string text, int length)
        {
            if (length > 0)
            {
                var ss = new StringSearch(keywords);
                var dictCount = new Dictionary<string, int>(keywords.Count());
                var allRes = ss.FindAll(text.ToLower()).GroupBy(x => x.Keyword).ToDictionary(x => x.Key, x => x.Count());
                foreach (var x in keywords)
                {
                    int val;
                    allRes.TryGetValue(x, out val);
                    yield return Convert.ToDouble(val) / length;
                }
            }
            else
            {
                foreach (var i in Enumerable.Range(0, keywords.Length))
                {
                    yield return 0d;
                }
            }
        }
    }
}