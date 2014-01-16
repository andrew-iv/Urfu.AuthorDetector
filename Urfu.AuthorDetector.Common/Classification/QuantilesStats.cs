using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class QuantilesStats : IByesStats
    {
        private readonly IQuantilesInfo _info;
        private readonly double _eps;

        private long GetHashCode(int[] items)
        {
            unchecked
            {
                return items.Aggregate<int, long>(items.Length, (current, item) => (current * _info.QuantileCounts) ^ item);
            }
        }

        public double Probability(IEnumerable<double> item)
        {
            var code = GetHashCode(_info.GetQuantiles(item));
            return _set.ContainsKey(code) ? ((double)_set[code]) / Count + _eps : _eps;
        }

        public int Count { get; private set; }

        private readonly Dictionary<long, int> _set = new Dictionary<long, int>();

        public QuantilesStats(IQuantilesInfo info, IEnumerable<double[]> sample, double eps = 0.00001)
        {
            _info = info;
            _eps = eps;
            foreach (var metric in sample)
            {
                var res = info.GetQuantiles(metric);
                if (res.Any(x => x == -1))
                {
                    continue;
                }
                Count++;
                var code = GetHashCode(res);
                if (_set.ContainsKey(code)) _set[code]++;
                else _set[code] = 1;

            }

        }
    }
}