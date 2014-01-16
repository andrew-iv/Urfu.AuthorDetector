using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Urfu.AuthorDetector.Common.StatMethods;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IQuantilesInfo
    {
        int Size { get; }
        int QuantileCounts { get; }
        int[] GetQuantiles(IEnumerable<double> vals);
    }

    public class QuantilesInfo : IQuantilesInfo
    {
        public int Size { get; private set; }
        private readonly double[][] _metrics;
        private readonly int _numOfQuants;
        private readonly double[][] _commonQuantiles;
        public int QuantileCounts { get { return _numOfQuants; } }

        public int[] GetQuantiles(IEnumerable<double> vals)
        {
            return vals.Zip(_commonQuantiles, (d, doubles) =>
                {
                    if (d > doubles[QuantileCounts]) return -1;
                    if (d < doubles[0]) return -1;

                    for (int i = 1; i <= QuantileCounts; i++)
                    {
                        if (d <= doubles[i]) return i;
                    }
                    return QuantileCounts;
                }).ToArray();
        }

        public QuantilesInfo(int size, double[][] allMetrics, int numOfQuants = 4)
        {
            Size = size;
            _metrics = allMetrics;
            _numOfQuants = numOfQuants;
            _commonQuantiles = CalculateQuantiles(size, allMetrics);
        }




        protected double[][] CalculateQuantiles(int size, double[][] allMetrics)
        {
            return Enumerable.Range(0, size).Select(i => allMetrics.Select(x => x[i]).ToArray().NNumberSummary(_numOfQuants)).ToArray();
        }
    }


}