using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.Sentance
{
    public class SelectedSentenceMetricProvider : ISentenceMetricProvider
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
        public IEnumerable<IEnumerable<int>> GetMetrics(string text)
        {
            return _baseProvider.GetMetrics(text).Select(metric =>
                {
                    var arr = metric.ToArray();
                    return _indexes.Select(i => arr[i]);
                });
        }
    }


    public class SentenceMetricProvider : ISentenceMetricProvider
    {
        public SentenceMetricProvider ()
        {
            var dict = StaticVars.Kernel.Get<Opcorpora.Dictionary.IOpcorporaDictionary>();
            _grammemes = dict.Grammemes.Select(x=>x.name).ToArray();
            Names = new string[] { "Length" }.Concat(_grammemes.Select(x => "Grammeme_" + x)).Concat(_punctuations.Select(x => "Punctuation_" + x));
        }
         

        private readonly string[] _grammemes;

        private static readonly char[] _punctuations = new []{'.',',','-','!','?',':','(',')'};


        protected virtual IEnumerable<int> GetSentenceMetrics(string text)
        {
            var si = new SentanceInfo(text);
            yield return si.Length;
            var allG = si.Grammemes.SelectMany(x => x.Select(xx=>xx.v)).GroupBy(x=>x).ToDictionary(x=>x.Key);
            foreach (var grammeme in _grammemes)
            {
                if(allG.ContainsKey(grammeme))
                {
                    yield return allG[grammeme].Count();
                }
                else
                {
                    yield return 0;
                }
            }

            foreach (var punc in _punctuations)
            {
                yield return text.Count(x => x == punc);
            }
        }

        public IEnumerable<IEnumerable<int>> GetMetrics(string text)
        {
            return text.Sentenses().Where(x=>text.RussianWords().Count() > 1).Select(sent =>
                                                                                     GetSentenceMetrics(sent).ToArray()
                );
        }
        
        public virtual IEnumerable<string> Names { get; private set; }
        public int Size {
            get { return Names.Count(); }
        }

    }
}