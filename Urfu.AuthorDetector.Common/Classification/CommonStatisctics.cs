using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common.Classification
{
    public static class CommonStatisctics
    {
        /*public static Dictionary<string, double> CoeffOfVariation = new Dictionary<string, double>(){
            {"Length",1.24067d},
            {"Time",0.422864d},
            {"ParagraphsShare",2.12443d},
            {"PunctuationShare",0.989877d},
            {"WhitespacesShare",0.338911d},
            {"OtherNodesShare",6.2875d},
        };*/

        public static IEnumerable<KeyValuePair<string, double>> CalculateVariance(this IEnumerable<IMetric> metrics)
        {
            return metrics.SelectMany(x => x.MetricValues).GroupBy(x => x.Key).Select(x =>
                                                                                      new KeyValuePair<string, double>(
                                                                                          x.Key, x.Select(
                                                                                              val => val.Value)
                                                                                                  .StandardDeviation())).OrderBy(x => x.Key);
        }

        public static double[] CalculateVariance(this double[][] metrics)
        {
            if (metrics == null || metrics.Length == 0)
                return new double[]{};
            var m = metrics[0].Length;
            return Enumerable.Range(0, m)
                      .Select(j => Enumerable.Range(0, metrics.Length).Select(i => metrics[i][j]).StandardDeviation())
                      .ToArray();
        }

        public static double[] CalculateAverage(this double[][] metrics)
        {
            if (metrics == null || metrics.Length == 0)
                return new double[] { };
            var m = metrics[0].Length;
            return Enumerable.Range(0, m)
                      .Select(j => Enumerable.Range(0, metrics.Length).Select(i => metrics[i][j]).Average())
                      .ToArray();
        }

        public static double[] CalculateMedian(this double[][] metrics)
        {
            if (metrics == null || metrics.Length == 0)
                return new double[] { };
            var m = metrics[0].Length;
            return Enumerable.Range(0, m)
                      .Select(j => Enumerable.Range(0, metrics.Length).Select(i => metrics[i][j]).Median())
                      .ToArray();
        }

        public static double[] Standardizate(this double[] metrics, double[] variance)
        {
            const double epsilon = 0.001d;
            if (metrics == null || metrics.Length == 0)
                return new double[] { };
            var m = metrics.Length;
            return Enumerable.Range(0, m)
                      .Select(j => metrics[j]/(variance[j]+epsilon))
                      .ToArray();
        }


        public static IEnumerable<KeyValuePair<string, double>> Standardizate(this IMetric metric, IEnumerable<KeyValuePair<string, double>> variance)
        {
            const double epsilon = 0.001d;
            var dict = new Dictionary<string, double>() ;
            return  metric.MetricValues.OrderBy(x=>x.Key).Zip(variance.OrderBy(x=>x.Key),(pair, varPair) => new KeyValuePair<string, double>(pair.Key,
                pair.Value/(varPair.Value + epsilon)
                ));
        }

        public static IEnumerable<KeyValuePair<string, double>> CalculateAverage(this IEnumerable<IMetric> metrics)
        {
            return metrics.SelectMany(x => x.MetricValues).GroupBy(x => x.Key).OrderBy(x=>x.Key).Select(x =>
                                                                                      new KeyValuePair<string, double>(
                                                                                          x.Key, x.Select(
                                                                                              val => val.Value)
                                                                                                  .Average()));
        }
    }
}