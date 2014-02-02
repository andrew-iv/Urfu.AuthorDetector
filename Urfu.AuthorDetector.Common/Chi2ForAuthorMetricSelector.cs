using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using MathNet.Numerics.Statistics;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    public class KolmogorovSmirnovMetricSelector : IMetricSelector
    {
        private int _topMetricsCount;
        private IEnumerable<string>[] _authorsArray;

        public KolmogorovSmirnovMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 3)
        {
            _topMetricsCount = topMetricsCount;
            _authorsArray = authors as IEnumerable<string>[] ?? authors.ToArray();
        }

        public IEnumerable<int> SelectMetrics(ICommonMetricProvider postMetricProvider)
        {
            
            var authorMetrics = new double[_authorsArray.Length][][];
            var hashRes = new HashSet<int>();
            var empArray = new EmpiricalDistribution[_authorsArray.Length][];

            foreach (var i in Enumerable.Range(0,_authorsArray.Length))
            {
                authorMetrics[i] = postMetricProvider.GetMetrics(_authorsArray[i]);
               /* empArray[i] = new EmpiricalDistribution[postMetricProvider.Size];
                foreach (var j in Enumerable.Range(0, postMetricProvider.Size))
                {
                    empArray[i][j] = new EmpiricalDistribution(authorMetrics[i].Select(x => x[j]).ToArray());
                }*/
            }



            foreach (var i in Enumerable.Range(0, _authorsArray.Length))
            {
                for (var j = i + 1; j < _authorsArray.Length; j++)
                {
                    
                    foreach (var index in Enumerable.Range(0, postMetricProvider.Size).Select(index => new
                    {
                        index,
                        distance = new TwoSampleKolmogorovSmirnovTest(authorMetrics[i].Select(x => x[index]).ToArray()
                                    ,authorMetrics[j].Select(x => x[index]).ToArray(),TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal)
                    }).Where(x=>x.distance.Significant).OrderByDescending(x => x.distance.Statistic)
                    .Take(_topMetricsCount).Select(x => x.index))
                    {
                        hashRes.Add(index);
                    }

                }
            }
            return hashRes.OrderBy(x => x).ToArray();
        }
    }

    public class Chi2HistogramTopMetricSelector : BaseHistogramTopMetricSelector
    {
        public Chi2HistogramTopMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 3) : base(authors, topMetricsCount)
        {
        }

        protected override double DistanceFunction(Histogram a, Histogram b)
        {
            return a.Chi2Distance(b);
        }
    }

    public class IntersectionHistogramTopMetricSelector : BaseHistogramTopMetricSelector
    {
        public IntersectionHistogramTopMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 3)
            : base(authors, topMetricsCount)
        {
        }

        protected override double DistanceFunction(Histogram a, Histogram b)
        {
            return 1d - a.IntersectionSquare(b);
        }
    }


    public abstract class BaseHistogramTopMetricSelector : IMetricSelector
    {
        private string _lower;
        private readonly string _upper;
        private readonly IEnumerable<string>[] _authorsArray;
        private const double eps = 0.00001;
        private const int Nbuckets = 33;
        private readonly int _topMetricsCount;

        protected BaseHistogramTopMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount=3)
        {
            _topMetricsCount = topMetricsCount;
            _authorsArray = authors as IEnumerable<string>[] ?? authors.ToArray();
        }

        protected abstract double DistanceFunction(Histogram a, Histogram b);
        


        public IEnumerable<int> SelectMetrics(ICommonMetricProvider postMetricProvider)
        {
            
            var histArray = new Histogram[_authorsArray.Length][];
            var authorMetrics = new double[_authorsArray.Length][][];
            var hashRes = new HashSet<int>();
            
            foreach (var i in Enumerable.Range(0,_authorsArray.Length))
            {
                authorMetrics[i] = postMetricProvider.GetMetrics(_authorsArray[i]);
            }

            foreach (var j in Enumerable.Range(0, postMetricProvider.Size))
            {
                var lower = authorMetrics.Select(x =>x.Select(xx=>xx[j]).Min()).Min();
                var upper = authorMetrics.Select(x => x.Select(xx => xx[j]).Max()).Max();
                if(upper-lower < eps)
                    continue;
                foreach (var i in Enumerable.Range(0, _authorsArray.Length))
                {
                    if (histArray[i] == null)
                    {
                        histArray[i] = new Histogram[postMetricProvider.Size];
                    }
                    histArray[i][j] =
                    authorMetrics[i].Select(x=>x[j]).ToHistogramm(Nbuckets, lower, upper);
                }
            }

            foreach (var i in Enumerable.Range(0, _authorsArray.Length))
            {
                for (var j = i + 1; j < _authorsArray.Length; j++)
                {
                    var a1 = histArray[i];
                    var a2 = histArray[j];
                    foreach (var index in Enumerable.Range(0, postMetricProvider.Size).Where(ind => a1[ind] != null && a2[ind] != null).Select(index => new
                        {
                            index,
                            distance = DistanceFunction(a1[index],a2[index])
                        }).OrderByDescending(x => x.distance).Take(_topMetricsCount).Select(x => x.index))
                    {
                        hashRes.Add(index);
                    }
                    
                }
            }
            return hashRes.OrderBy(x => x).ToArray();
        }
    }
}