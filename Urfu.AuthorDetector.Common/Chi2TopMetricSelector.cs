using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    public class Chi2TopMetricSelector : IMetricSelector
    {
        private int _topMetricsCount;
        private IEnumerable<string>[] _authorsArray;

        public Chi2TopMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 10)
        {
            _topMetricsCount = topMetricsCount;
            _authorsArray = authors as IEnumerable<string>[] ?? authors.ToArray();
        }

        
        

        public Dictionary<int,HashSet<int>> SelectMetrics(ICommonMetricProvider postMetricProvider)
        {
            
            var authorMetrics = new double[_authorsArray.Length][][];
            var hashRes = Enumerable.Range(1,10).ToDictionary(x=>x,x=>new HashSet<int>());
            foreach (var i in Enumerable.Range(0, _authorsArray.Length))
            {
                authorMetrics[i] = postMetricProvider.GetMetrics(_authorsArray[i]);
            }
            var allMetrics = authorMetrics.SelectMany(x => x).ToArray();


            var qi = new QuantilesInfo(postMetricProvider.Size, allMetrics, 10);

            var counts = new int[_authorsArray.Length, postMetricProvider.Size, qi.QuantileCounts+2];

            foreach (var i in Enumerable.Range(0, _authorsArray.Length))
            {
                foreach (var metricPerc in authorMetrics[i].Select(qi.GetQuantiles))
                {
                    for (var j = 0; j < postMetricProvider.Size; j++)
                    {
                        counts[i, j, metricPerc[j]+1]++;
                    }
                }
            }
            

            //new ChiSquareTest()
            foreach (var i in Enumerable.Range(0, _authorsArray.Length))
            {
                for (var j = i + 1; j < _authorsArray.Length; j++)
                {
                    var skipCount = 0;
                    foreach (var index in Enumerable.Range(0, postMetricProvider.Size).Select(delegate(int metricIndex)
                        {
                            var arr1 = Enumerable.Range(0, qi.QuantileCounts + 2)
                                                 .Select(
                                                     o => counts[i, metricIndex, o] / (double)authorMetrics[i].Length + 1E-30)
                                                 .ToArray();
                            var arr2 = Enumerable.Range(0, qi.QuantileCounts + 2)
                                                 .Select(
                                                     o => counts[j, metricIndex, o]/(double) authorMetrics[j].Length+1E-30)
                                                 .ToArray();

                            return
                                new
                                    {
                                        distance =
                                            Enumerable.Range(0, arr1.Length).Select(ii =>

                                                                                    (arr1[ii] - arr2[ii])*
                                                                                    (arr1[ii] - arr2[ii])/
                                                                                    (arr1[ii] + arr2[ii])).Sum(),
                                        index = metricIndex
                                    };

                        }).OrderByDescending(x => x.distance)
                                                    .Take(_topMetricsCount).Select(x => x.index))
                    {
                        for (int k = (++skipCount); k <= _topMetricsCount; k++)
                        {
                            hashRes[k].Add(index);
                        }
                        
                    }

                }
            }
            return hashRes;
        }
    }
}