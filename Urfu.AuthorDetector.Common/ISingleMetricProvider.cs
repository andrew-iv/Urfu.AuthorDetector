using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common
{
    public interface ISingleMetricProvider : ICommonMetricProvider
    {
        double[] GetMetrics(IEnumerable<string> text);
    }


    public static class GroupMetricFunctions
    {
        public static double[][] GetGrouped(double[][] vals, int k,  Func<double[][],double[]> func, bool addTail = true)
        {

            if (addTail)
            {
                int n = vals.Length/k;
                k += (vals.Length%k)/n;
            }

            return Enumerable.Range(0, vals.Length/k)
                .Select(i =>
                    func(
                        vals.Skip(i*k).Take(k).ToArray())
                ).ToArray();
        }

        public static double[][] GetAverages(double[][] vals, int k,bool needTail = true)
        {
            return GetGrouped(vals, k, CommonStatisctics.CalculateAverage, needTail);
        }

        public static double[][] GetMedians(double[][] vals, int k, bool needTail = true)
        {
            return GetGrouped(vals, k, CommonStatisctics.CalculateMedian, needTail);
        }
    }
}