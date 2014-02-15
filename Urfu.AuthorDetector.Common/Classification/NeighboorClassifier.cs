using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class NeighboorClassifier : IClassifier
    {
        private readonly IPostMetricProvider _postMetricProvider;
        private readonly Dictionary<Author, double[]> _authorMetrics;
        private double[][] _allMetrics;
        private double[] _variance;




        protected virtual void AdditionalInitialization(IDictionary<Author, IEnumerable<string>> authors)
        {

        }

        public NeighboorClassifier(IDictionary<Author, IEnumerable<string>> authors, IPostMetricProvider postMetricProvider)
        {
            _postMetricProvider = postMetricProvider;
            AdditionalInitialization(authors);
            _allMetrics = authors.SelectMany(x => x.Value.Select(CallculateMetric)).ToArray();
            _variance = _allMetrics.CalculateVariance();
            Authors = authors.Keys;
            _authorMetrics = new Dictionary<Author, double[]>();
            foreach (var author in authors)
            {
                _authorMetrics.Add(author.Key, CallculateMetrics(author.Value));
            }


        }

        protected virtual double[] CallculateMetric(string post)
        {
            return _postMetricProvider.GetMetrics(post).ToArray();
        }


        protected virtual double[] CallculateMetrics(IEnumerable<string> posts)
        {
            return posts.Select(CallculateMetric).ToArray().CalculateMedian().Standardizate(_variance).ToArray();
        }

        protected virtual double CompareMetrics(double[] a, double[] b)
        {
            const double keyNotFoundPenalty = 100d;
            double res = 0d;
            return Math.Sqrt(b.Zip
                (a, (aKvp, bKvp) =>
                    {
                        var diff = aKvp - bKvp;
                        return diff * diff;
                    }).Sum());
        }

        public void LogResult(bool isSuccess)
        {
            
        }

        public IEnumerable<Author> Authors { get; private set; }
        public Author ClassificatePosts(IEnumerable<string> posts, out bool reliable)
        {
            reliable = true;
            return ClassificatePosts(posts,1)[0];
        }

        public Author[] ClassificatePosts(IEnumerable<string> posts,int topN)
        {
            var metric = CallculateMetrics(posts);
            var etalons = _authorMetrics.OrderBy(x => CompareMetrics(x.Value, metric)).Select(x => x.Key);
            return etalons.ToArray();
        }

        public virtual string Description
        {
            get { return "Метод ближайших соседей"; }
        }
        public virtual string Name { get { return "Метод ближайших соседей"; } }
        public double ErrorLevel { get; set; }
    }
}