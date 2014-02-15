using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public class GrammemesPostMetricProvider : BasePostMetricProvider
    {
        private readonly bool _unknown;

        public GrammemesPostMetricProvider(string[] grammemes=null, bool unknown =true):base()
        {
            _unknown = unknown;
            _grammemes =  grammemes ?? StaticVars.Kernel.Get<Opcorpora.Dictionary.IOpcorporaDictionary>().Grammemes.Select(x => x.name).ToArray();
        }


        private readonly string[] _grammemes;

        public override IEnumerable<string> Names
        {
            get
            {
                return _grammemes.Select(x => "Grammeme_" + x).Union(
                    _unknown?
                    new string[]{"Grammeme_UNKNOWN"}
                    :
                    new string[] {}
                    );
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
            if(_unknown)
                yield return si.UnknownWords.Length /(double) si.Length;
        }
    }
}