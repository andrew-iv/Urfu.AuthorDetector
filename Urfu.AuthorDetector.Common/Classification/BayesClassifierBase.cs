using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;
using Ninject;

namespace Urfu.AuthorDetector.Common.Classification
{
    public abstract class BayesClassifierBase : IClassifier
    {

        private class ProviderStats
        {
            public Dictionary<Author, IByesStats> Stats;
            public IQuantilesInfo Info;
            public ICommonMetricProvider Provider;

        }

        private readonly IDictionary<Author, IEnumerable<string>> _authors;
        private readonly ICommonMetricProvider[] _providers;

        private readonly string[] _allPosts;
        private readonly ProviderStats[] _stats;
        private BayesClassifierTest _lastResult;


        protected BayesClassifierBase(IDictionary<Author, IEnumerable<string>> authors, params ICommonMetricProvider[] providers)
        {
            _authors = authors;
            _providers = providers;
            _allPosts = authors.SelectMany(x => x.Value).ToArray();
            _stats = _providers.Select(InitProvider).ToArray();

        }

        public void LogResult(bool isSuccess)
        {
            var logger = StaticVars.Kernel.Get<IBayesResultLogger>();
            if (logger == null) return;
            _lastResult.Success = isSuccess;
            logger.Log(_lastResult);
        }

        public IEnumerable<Author> Authors
        {
            get { return _authors.Keys; }
        }



        public string Description { get; private set; }
        public string Name { get; private set; }
        public double ErrorLevel { get; set; }

        protected abstract IQuantilesInfo QuantilesInfoConstructor(int size, double[][] allMetrics);
        protected abstract IByesStats ByesStatsConstructor(IQuantilesInfo quantilesInfo, IEnumerable<double[]> allMetrics);

        private ProviderStats InitProvider(ICommonMetricProvider provider)
        {
            var info = QuantilesInfoConstructor(provider.Size,
                                                provider.GetMetrics(_allPosts).ToArray());
            return new ProviderStats
                {
                    Info = info,
                    Stats = _authors.ToDictionary(x => x.Key,
                                                  x =>
                                                  ByesStatsConstructor(info,
                                                                       provider.GetMetrics(x.Value)) as
                                                  IByesStats),
                    Provider = provider
                };
        }



        protected virtual IEnumerable<KeyValuePair<Author, double>> AuthorProbab(IEnumerable<KeyValuePair<Author, IByesStats>> stats, IEnumerable<double[]> metrics)
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
            return statArr.Zip(probab, (x, pr) => new KeyValuePair<Author, double>(x.Key, pr)).ToArray();
        }

        public Author ClassificatePosts(IEnumerable<string> posts, out bool reliable)
        {
            reliable = true;
            return ClassificatePosts(posts, 1)[0];
        }

        public Author[] ClassificatePosts(IEnumerable<string> posts, int topN)
        {
            posts = posts as string[] ?? posts.ToArray();
            Dictionary<Author, double> cands = Authors.ToDictionary(x => x, x => 1d);

            foreach (var stat in _stats)
            {
                foreach (var avt in AuthorProbab(stat.Stats, stat.Provider.GetMetrics(posts)))
                {
                    cands[avt.Key] *= avt.Value;
                }
            }
            var ordered = cands.OrderByDescending(x => x.Value).ToArray();
            _lastResult = new BayesClassifierTest()
                {
                    FirstToAll = ordered[0].Value / cands.Skip(1).Sum(x => x.Value),
                    FirstToSecond
                        = ordered[0].Value / ordered[1].Value,
                    MessageCount = posts.Count()
                };
            _lastResult.MessagesLength = posts.Select(x => x.Length).Sum();
            return ordered.Select(x => x.Key).Take(topN).ToArray();
        }

    }
}