using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class StuipidQuantilesStats : IByesStats
    {
        private readonly IQuantilesInfo _info;
        private readonly double _eps;

        private int[,] _counts;

        public double Probability(IEnumerable<double> item)
        {
            var quant = _info.GetQuantiles(item).ToArray();
            var res = 1d;


            for (int i = 0; i < quant.Length; i++)
            {
                if (quant[i] == -1)
                {
                    res *= _eps;
                    continue;
                }


                var prev = quant[i] > 1 ? _counts[i, quant[i] - 2] : 0;
                var next = quant[i] < _info.QuantileCounts ? _counts[i, quant[i]] : 0;
                res *= ((double)_counts[i, quant[i] - 1] + (prev+next)*0.15d )  / Count + _eps;
            }return res;
            // return _set.ContainsKey(code) ? ((double) _set[code])/Count +_eps : _eps;
        }

        public int Count { get; private set; }

        private readonly Dictionary<long, int> _set = new Dictionary<long, int>();

        public StuipidQuantilesStats(IQuantilesInfo info, IEnumerable<double[]> sample, double eps = 0.00005)
        {
            _info = info;
            _eps = eps;
            _counts = new int[info.Size, info.QuantileCounts];

            foreach (var metric in sample)
            {
                var res = _info.GetQuantiles(metric);
                for (int i = 0; i < _info.Size; i++)
                {
                    if (res[i] == -1) continue;
                    _counts[i, res[i] - 1]++;
                }
                Count++;
            }

        }
    }
}