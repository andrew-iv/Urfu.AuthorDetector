using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public class GrammemesPostMetricProvider : BasePostMetricProvider
    {
        public GrammemesPostMetricProvider():base()
        {
            var dict = StaticVars.Kernel.Get<Opcorpora.Dictionary.IOpcorporaDictionary>();
            _grammemes = dict.Grammemes.Select(x => x.name).ToArray();
        }


        private readonly string[] _grammemes;

        public override IEnumerable<string> Names
        {
            get
            {
                return _grammemes.Select(x => "Grammeme_" + x).Union(new string[]{"Grammeme_UNKNOWN"});
            }
        }

        public override double[] GetMetrics(string text)
        {
            return GetSentenceMetrics(new SentanceInfo(text)).ToArray();
        }

        private IEnumerable<double> GetSentenceMetrics(SentanceInfo si)
        {
            var allG = si.Grammemes.SelectMany(x => x.Select(xx => xx.v)).GroupBy(x => x).ToDictionary(x => x.Key);
            foreach (var grammeme in _grammemes)
            {
                if (allG.ContainsKey(grammeme))
                {
                    yield return allG[grammeme].Count()/(double) si.Length;
                }
                else
                {
                    yield return 0;
                }
            }
            yield return si.UnknownWords.Length /(double) si.Length;
        }
    }
}