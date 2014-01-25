using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{




    public abstract class BayesClassifierBase : IClassifier
    {
        private readonly IDictionary<Author, IEnumerable<string>> _authors;
        private readonly IPostMetricProvider _commonProvider;
        private readonly IMultiplyMetricsProvider _multiplyProvider;
        private readonly string[] _allPosts;
        private IQuantilesInfo _commonInfo;
        private IQuantilesInfo _multyInfo;
        private Dictionary<Author, IByesStats> _commonStats;
        private Dictionary<Author, IByesStats> _multyStats;

        protected BayesClassifierBase(IDictionary<Author, IEnumerable<string>> authors, IPostMetricProvider commonProvider = null, IMultiplyMetricsProvider multiplyProvider = null)
        {
            _authors = authors;
            _commonProvider = commonProvider;
            _multiplyProvider = multiplyProvider;
            _allPosts = authors.SelectMany(x => x.Value).ToArray();
            if (_commonProvider != null)
                InitPostProvider();
            if (_multiplyProvider != null)
                InitMultyProvider();
        }

        public IEnumerable<Author> Authors {
            get { return _authors.Keys; }
        }

        public string Description { get; private set; }
        public string Name { get; private set; }

        protected abstract IQuantilesInfo QuantilesInfoConstructor(int size, double[][] allMetrics);
        protected abstract IByesStats ByesStatsConstructor(IQuantilesInfo quantilesInfo, IEnumerable<double[]> allMetrics);

        private void InitPostProvider()
        {
            _commonInfo = QuantilesInfoConstructor(_commonProvider.Size, _allPosts.Select(x => _commonProvider.GetMetrics(x).ToArray()).ToArray());
            _commonStats = _authors.ToDictionary(x => x.Key,
                                                 x =>
                                                 ByesStatsConstructor(_commonInfo,
                                                                      x.Value.Select(xx => _commonProvider.GetMetrics(xx).ToArray())) as IByesStats);
        }

        private void InitMultyProvider()
        {
            _multyInfo = QuantilesInfoConstructor(_multiplyProvider.Size, _allPosts.SelectMany(x => _multiplyProvider.GetMetrics(x).ToArray()).ToArray());
            _multyStats = _authors.ToDictionary(x => x.Key,
                                                x =>
                                                ByesStatsConstructor(_multyInfo,
                                                                     x.Value.SelectMany(xx => _multiplyProvider.GetMetrics(xx).ToArray())) as IByesStats);
        }

        protected virtual IEnumerable<KeyValuePair<Author, double>> AuthorProbab(IDictionary<Author, IByesStats> stats, IEnumerable<double[]> metrics)
        {
            var statArr = stats.ToArray();
            var probab = new double[statArr.Length];
            for (int i = 0; i < probab.Length; i++)
            {
                probab[i] = 1.0;
            }

            foreach (var metr in metrics)
            {
                var mx = probab.Max();
                for (int i = 0; i < probab.Length; i++)
                {
                    probab[i] *= statArr[i].Value.Probability(metr) / mx;
                }
            }
            return statArr.Zip(probab, (x, pr) => new KeyValuePair<Author, double>(x.Key,pr )).ToArray();
        }

        public Author ClassificatePosts(IEnumerable<string> posts)
        {
            posts = posts as string[] ?? posts.ToArray();
            Dictionary<Author, double> cands = Authors.ToDictionary(x => x, x => 1d);
            if (_commonProvider != null)
            {
                foreach (var avt in AuthorProbab(_commonStats,posts.Select(x=>_commonProvider.GetMetrics(x).ToArray())))
                {
                    cands[avt.Key] *= avt.Value;
                }
            }

            if (_multiplyProvider != null)
            {
                foreach (var avt in AuthorProbab(_multyStats, posts.SelectMany(x => _multiplyProvider.GetMetrics(x).ToArray())))
                {
                    cands[avt.Key] *= avt.Value;
                }
            }

            return cands.OrderByDescending(x => x.Value).First().Key;

        }
    }
}