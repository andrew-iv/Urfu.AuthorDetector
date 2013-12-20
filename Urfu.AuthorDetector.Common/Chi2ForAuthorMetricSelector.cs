using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common
{
    //.Chi2Distance

    public class Chi2ForAuthorMetricSelector : BaseForAuthorMetricSelector
    {
        public Chi2ForAuthorMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 3) : base(authors, topMetricsCount)
        {
        }

        protected override double DistanceFunction(Histogram a, Histogram b)
        {
            return a.Chi2Distance(b);
        }
    }

    public class IntersectionForAuthorMetricSelector : BaseForAuthorMetricSelector
    {
        public IntersectionForAuthorMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount = 3)
            : base(authors, topMetricsCount)
        {
        }

        protected override double DistanceFunction(Histogram a, Histogram b)
        {
            return 1d - a.IntersectionSquare(b);
        }
    }


    public abstract class BaseForAuthorMetricSelector : IMetricSelector
    {
        private string _lower;
        private readonly string _upper;
        private readonly IEnumerable<string>[] _authorsArray;
        private const double eps = 0.00001;
        private const int Nbuckets = 33;
        private readonly int _topMetricsCount;

        protected BaseForAuthorMetricSelector(IEnumerable<IEnumerable<string>> authors, int topMetricsCount=3)
        {
            _topMetricsCount = topMetricsCount;
            _authorsArray = authors as IEnumerable<string>[] ?? authors.ToArray();
        }

        protected abstract double DistanceFunction(Histogram a, Histogram b);
        


        public IEnumerable<int> SelectMetrics(IMetricProvider metricProvider)
        {
            
            var histArray = new Histogram[_authorsArray.Length][];
            var authorMetrics = new double[_authorsArray.Length][][];
            var hashRes = new HashSet<int>();
            
            foreach (var i in Enumerable.Range(0,_authorsArray.Length))
            {
                authorMetrics[i] = _authorsArray[i].Select(x => metricProvider.GetMetrics(x).ToArray()).ToArray();
            }

            foreach (var j in Enumerable.Range(0, metricProvider.Size))
            {
                var lower = authorMetrics.Select(x =>x.Select(xx=>xx[j]).Min()).Min();
                var upper = authorMetrics.Select(x => x.Select(xx => xx[j]).Max()).Max();
                if(upper-lower < eps)
                    continue;
                foreach (var i in Enumerable.Range(0, _authorsArray.Length))
                {
                    if (histArray[i] == null)
                    {
                        histArray[i] = new Histogram[metricProvider.Size];
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
                    foreach (var index in Enumerable.Range(0, metricProvider.Size).Where(ind => a1[ind] != null && a2[ind] != null).Select(index => new
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