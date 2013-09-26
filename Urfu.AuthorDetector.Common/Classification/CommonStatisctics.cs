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