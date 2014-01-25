using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Ninject;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class StupidBayesClassifier : IClassifier
    {
        private readonly IPostMetricProvider _postMetricProvider;
        private Dictionary<Author, Histogram[]> _authorHistogramms;

        private const int BucketCounts = 33;

        public StupidBayesClassifier(IDictionary<Author, IEnumerable<string>> authors, IPostMetricProvider postMetricProvider)
        {
            Authors = authors.Keys;
            var dataExtractor = StaticVars.Kernel.Get<IDataExtractor>();
            _postMetricProvider = postMetricProvider;
            _postMetricProvider = postMetricProvider;
            var m = _postMetricProvider.Size;
            var authorMetrics = authors.ToDictionary(x => x.Key,
                                                     x => x.Value.Select(xx => _postMetricProvider.GetMetrics(xx).ToArray()).ToArray());

            var minVals = Enumerable.Range(0, m).Select(i => authorMetrics.Select(x => x.Value.Select(xx => xx[i]).Min()).Min()).ToArray();
            var maxVals = Enumerable.Range(0, m).Select(i => authorMetrics.Select(x => x.Value.Select(xx => xx[i]).Max()).Max()).ToArray();


            _authorHistogramms = authorMetrics.ToDictionary(x => x.Key,
                                                            x =>
                                                            Enumerable.Range(0, m)
                                                                      .Select(
                                                                          j =>
                                                                          x.Value.Select(val => val[j])
                                                                           .ToHistogramm(BucketCounts, minVals[j], maxVals[j])).ToArray());
        }

        protected virtual double GetProbability(Histogram histogram, double value, double eps = 0.00025)
        {
            if (eps <= 0)
                throw new ArgumentOutOfRangeException("eps");

            if (histogram.UpperBound < value + eps * eps * eps)
            {
                return eps;
            }

            if (histogram.LowerBound > value - eps * eps * eps)
            {
                return eps;
            }

            var val = Convert.ToDouble(histogram.GetBucketOf(value).Count) / histogram.DataCount;


            return (val + eps) / (1 + eps * histogram.BucketCount);

        }


        public IEnumerable<Author> Authors { get; private set; }
        public Author ClassificatePosts(IEnumerable<string> posts)
        {
            var resDict = Authors.ToDictionary(x => x, x => 1d);
            var postsMetrics = posts.Select(x => _postMetricProvider.GetMetrics(x).ToArray()).ToArray();
            foreach (var post in postsMetrics)
            {
                foreach (var j in Enumerable.Range(0, _postMetricProvider.Size))
                {
                    foreach (var author in Authors)
                    {
                        var hist = _authorHistogramms[author];

                        resDict[author] = resDict[author] * GetProbability(hist[j], post[j]);
                    }
                }
                var mx = resDict.Max(x => x.Value);
                foreach (var author in Authors)
                {
                    resDict[author] = resDict[author] / mx;
                }
            }
            return resDict.OrderByDescending(x => x.Value).First().Key;
        }

        public string Description { get; private set; }
        public string Name
        {
            get { return "Байсовский классификатор."; }
        }
    }
}