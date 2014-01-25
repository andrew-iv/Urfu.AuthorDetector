using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics.Analysis;

using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common.StatMethods
{
    public static class StatisticsHelper
    {

        public static double[] NNumberSummary(this double[] data, int nums)
        {
            if (data.Length == 0) return Enumerable.Range(1, nums + 1).Select(x => double.NaN).ToArray();

            return (new[] { data.Minimum() }.Concat(Enumerable.Range(1, nums - 1)
                .Select(i => ArrayStatistics.QuantileInplace(data, (double)i / nums))).Concat(new[] { data.Maximum() })).ToArray();


        }

        /*public static double[] SrezX(this Matrix m, int x)
        {
            return m.GetColumn(x);
        }*/

        public static IEnumerable<double> MultiplyAndSum<T>(this IEnumerable<T> source, IEnumerable<double> coefficents) where T : IEnumerable<double>
        {
            return source.Select(x => MultiplyAndSum(x, coefficents));
        }

        public static double MultiplyAndSum(this IEnumerable<double> source, IEnumerable<double> coefficents)
        {
            return source.Zip(coefficents, (d, d1) => d * d1).Sum();
        }
/*
        public static Matrix CreateMatrix<T>(this IEnumerable<T> metrics) where T : IEnumerable<double>
        {
            var all = metrics.Select(x => x.ToArray()).ToArray();
            var yDim = all.Length;
            var xDim = all.Max(x => x.Length);
            var arr = new double[yDim, xDim];
            for (int i = 0; i < all.Length; i++)
            {
                var allAtI = all[i];
                for (int j = 0; j < allAtI.Length; j++)
                {
                    arr[i, j] = allAtI[j];
                }
            }
            return Matrix.Create(arr);
        }*/

        public static PrincipalComponentAnalysis GetPcas(this double[][] matr)
        {
            var anal = new PrincipalComponentAnalysis(matr, AnalysisMethod.Standardize);
                
            return anal;
        }
    }
}