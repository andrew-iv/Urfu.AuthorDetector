using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.MetricProvider
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
}