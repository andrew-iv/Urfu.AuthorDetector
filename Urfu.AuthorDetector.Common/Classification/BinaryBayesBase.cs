using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common.Classification
{
    public abstract class BinaryBayesBase
    {
        private readonly string[] _myMessages;
        private readonly string[] _notMyMessages;
        private readonly ICommonMetricProvider[] _providers;
        private readonly IEnumerable<string> _allPosts;
        private ProviderStats[] Stats { get; set; }

        protected class ProviderStats
        {
            public IByesStats MyStat;
            public IByesStats OtherStat;
            public IQuantilesInfo Info;
            public ICommonMetricProvider Provider;

        }

        protected virtual double AuthorProbab(IByesStats myStat, IByesStats otherStat, IEnumerable<double[]> metrics)
        {
            return metrics.Aggregate(1d, (current, metr) => current*myStat.Probability(metr)/otherStat.Probability(metr));
        }


        protected virtual IQuantilesInfo QuantilesInfoConstructor(int size, double[][] allMetrics)
        {
            return new QuantilesInfo(size, allMetrics, 20);
        }

        protected virtual IByesStats BayesStatsConstructor(IQuantilesInfo quantilesInfo, IEnumerable<double[]> allMetrics)
        {
            return new StuipidQuantilesStats(quantilesInfo, allMetrics);
        }

        private ProviderStats InitProvider(ICommonMetricProvider provider)
        {
            var info = QuantilesInfoConstructor(provider.Size,
                                                provider.GetMetrics(_allPosts).ToArray());
            return new ProviderStats
                {
                    Info = info,
                    MyStat = BayesStatsConstructor(info,provider.GetMetrics(_myMessages)),
                    OtherStat = BayesStatsConstructor(info,provider.GetMetrics(_notMyMessages)),
                    Provider = provider
                };
        }

        protected double TextProbab(IEnumerable<string> post)
        {
            post = post as string[]?? post.ToArray();
            return Stats.Select(stat =>
                                AuthorProbab(stat.MyStat, stat.OtherStat, stat.Provider.GetMetrics(post)))
                        .Aggregate(1d, (d, d1) => d*d1);
        }

      
        protected BinaryBayesBase(IEnumerable<string> myMessages, IEnumerable<string> notMyMessages, params ICommonMetricProvider[] providers)
        {
            
            _myMessages = myMessages as string[] ?? myMessages.ToArray();
            _notMyMessages = notMyMessages as string[] ?? notMyMessages.ToArray();
            _providers = providers;
            _allPosts = _myMessages.Concat(_notMyMessages);
            Stats = _providers.Select(InitProvider).ToArray();
           

        }
    }
}