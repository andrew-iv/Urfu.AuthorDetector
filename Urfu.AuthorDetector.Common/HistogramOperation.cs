using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common
{
    public static class HistogramOperation
    {
        public static IEnumerable<double> DecreaseToDiapason(this IEnumerable<double> items, double lower, double upper)
        {
            if(lower>upper) throw new ArgumentOutOfRangeException("upper","upper должен быть больше чем lower");

            return items.Select(x => lower > x ? lower : (upper < x ? upper : x));
        }

        public static Histogram ToHistogramm(this IEnumerable<double> items, int nbuckets, double lower, double upper)
        {
            const double eps = 0.00000001;
            return new Histogram(items.DecreaseToDiapason(
                lower, upper), nbuckets, lower - eps * Math.Abs(upper - lower + eps), upper + eps * Math.Abs(upper - lower + eps));
        }
        

        private static double Compare(Histogram a, Histogram b, Func<double, double, double> metric)
        {
            var distMin = Math.Min(a.UpperBound, b.UpperBound) - Math.Max(a.LowerBound, b.LowerBound);
            if (distMin <= 0)
                return 0;

            var distAll = Math.Max(a.UpperBound, b.UpperBound) - Math.Min(a.LowerBound, b.LowerBound);

            var res = 0d;
            int i = 0;
            int j = 0;
            var buckA = a[i++];
            var buckB = b[j++];


            while (buckB != null && buckA != null)
            {
                if (buckA.UpperBound <= buckB.LowerBound)
                {
                    if (i == a.BucketCount)
                        break;
                    buckA = a[i++];
                    continue;
                }
                if (buckB.UpperBound <= buckA.LowerBound)
                {
                    if (j == b.BucketCount)
                        break;
                    buckB = b[j++];
                    continue;
                }

                var maxLow = Math.Max(buckA.LowerBound, buckB.LowerBound);
                var minUp = Math.Min(buckA.UpperBound, buckB.UpperBound);
                var dist = minUp - maxLow;
                var aShare = dist / buckA.Width * buckA.Count / a.DataCount;
                var bShare = dist / buckB.Width * buckB.Count / b.DataCount;
                res += metric(aShare, bShare);
                //Math.Min(aShare, bShare) * dist / distAll;

                //Переход к следующей
                if (minUp == buckA.UpperBound)
                {
                    if (i == a.BucketCount)
                        break;
                    buckA = a[i++];
                }
                else
                {
                    if (j == b.BucketCount)
                        break;
                    buckB = b[j++];
                }
            }
            return res;
        }


        public static double Chi2Distance(this Histogram a, Histogram b)
        {

            const double eps = 0.00001;
            if (Math.Abs(a.LowerBound - b.LowerBound) > eps
                ||
                Math.Abs(a.UpperBound - b.UpperBound) > eps
                ) throw new ArgumentOutOfRangeException("Границы гистограмм должны совпадать");

            return Compare(a, b, (p1, p2) => Math.Abs(p1 + p2 - 0d) > eps ? (p1 - p2) * (p1 - p2) / (p1 + p2) : 0d);
        }


        public static double IntersectionSquare(this Histogram a, Histogram b)
        {
            return Compare(a, b, Math.Min);
        }
    }
}