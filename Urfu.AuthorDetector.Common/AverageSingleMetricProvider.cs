using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    /// <summary>
    /// Вычисляет среднее значение
    /// </summary>
    public class AverageSingleMetricProvider : ISingleMetricProvider
    {
        private readonly ICommonMetricProvider _pmProvider;

        public AverageSingleMetricProvider(ICommonMetricProvider pmProvider)
        {
            _pmProvider = pmProvider;
        }

        public IEnumerable<string> Names { get { return _pmProvider.Names; } }
        public int Size { get { return _pmProvider.Size; } }
        double[][] ICommonMetricProvider.GetMetrics(IEnumerable<string> text)
        {
            return  new []{GetMetrics(text)};
        }

        public double[] GetMetrics(IEnumerable<string> text)
        {
            var arr= text.ToArray();
            var summ = _pmProvider.GetMetrics(text)
                          .Aggregate(null as double[],
                                     (acc, next) => acc == null ? next : acc.Zip(next, (d, d1) => d + d1).ToArray());
            return summ.Select(x => x/arr.Length).ToArray();

        }
    }
}