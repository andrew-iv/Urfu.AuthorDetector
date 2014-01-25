using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider.Sentance
{
    public class CombinedCommonMetricProvider : ICommonMetricProvider
    {
        private ICommonMetricProvider[] _providers;

        public CombinedCommonMetricProvider(params ICommonMetricProvider[] providers)
        {
            _providers = providers;
        }

        public IEnumerable<string> Names
        {
            get { return _providers.SelectMany(x => x.Names); }
        }

        public int Size
        {
            get { return _providers.Sum(x => x.Size); }
        }

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            var arr = text.ToArray();
            var res = _providers.Select(x => x.GetMetrics(arr).ToArray()).ToArray();
            return Enumerable.Range(0, res.Min(x => x.Length))
                             .Select(i =>
                                     res.Select(x => x[i])
                                        .Aggregate<IEnumerable<double>, IEnumerable<double>>(new double[] {},
                                                                                             Enumerable.Concat)
                                        .ToArray()).ToArray();

        }
    }

    public class CombinedMultyMetricProvider : BaseSentenceMetricProvider
    {
        private readonly ISentenceMetricProvider[] _providers;
        private readonly string[] _names;

        public CombinedMultyMetricProvider(params ISentenceMetricProvider[] providers)
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