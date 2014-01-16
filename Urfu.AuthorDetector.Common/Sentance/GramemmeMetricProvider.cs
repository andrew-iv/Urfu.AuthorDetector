using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public class SelectedSentenceMetricProvider : IMultiplyMetricsProvider
    {
        private ISentenceMetricProvider _baseProvider;
        private int[] _indexes;


        public SelectedSentenceMetricProvider(int[] indexes, ISentenceMetricProvider baseProvider)
        {
            _indexes = indexes;
            _baseProvider = baseProvider;
            var arrNames = _baseProvider.Names.ToArray();
            Names = indexes.Select(i => arrNames[i]).ToArray();
            Size = Names.Count();
        }

        public IEnumerable<string> Names { get; private set; }
        public int Size { get; private set; }
        public IEnumerable<double[]> GetMetrics(string text)
        {
            return _baseProvider.GetMetrics(text).Select(metric =>
                {
                    var arr = metric.ToArray();
                    return _indexes.Select(i => arr[i]).ToArray();
                });
        }
    }


    public class GramemmeMetricProvider : BaseSentenceMetricProvider
    {
        public GramemmeMetricProvider()
        {
            var dict = StaticVars.Kernel.Get<Opcorpora.Dictionary.IOpcorporaDictionary>();
            _grammemes = dict.Grammemes.Select(x => x.name).ToArray();
        }


        private readonly string[] _grammemes;

        public override IEnumerable<string> Names
        {
            get
            {
                return _grammemes.Select(x => "Grammeme_" + x);
            }
        }


        public override IEnumerable<double> GetSentenceMetrics(SentanceInfo si)
        {
            var allG = si.Grammemes.SelectMany(x => x.Select(xx => xx.v)).GroupBy(x => x).ToDictionary(x => x.Key);
            foreach (var grammeme in _grammemes)
            {
                if (allG.ContainsKey(grammeme))
                {
                    yield return allG[grammeme].Count();
                }
                else
                {
                    yield return 0;
                }
            }
        }
    }
}