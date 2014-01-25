using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class PostMetricProviderStrategyProxy : StrategyProxyBase, IPostMetricProviderStrategyProxy
    {
            private IPostMetricProvider _postMetricProvider;

            public void Start()
            {
                _postMetricProvider = Factory.PostMetricProvider;
            }

            public void End()
            {
                Factory.PostMetricProvider = _postMetricProvider;
            }

            public void SetIndexes(  int[] indexes)
            {
                Factory.PostMetricProvider = new SelectedPostMetricProvider(_postMetricProvider)
                            {
                                Indexes = indexes
                            };
            }

            public int Size
            {
                get { return _postMetricProvider.Size; }
            }

            public IEnumerable<string> Names
            {
                get { return _postMetricProvider.Names; }
            }
        }

    public class MultyMetricProviderStrategyProxy : StrategyProxyBase, IPostMetricProviderStrategyProxy
    {
        private IMultiplyMetricsProvider _postMetricProvider;

        public void Start()
        {
            _postMetricProvider = Factory.MultiplyMetricsProvider;
        }

        public void End()
        {
            Factory.MultiplyMetricsProvider = _postMetricProvider;
        }

        public void SetIndexes(int[] indexes)
        {
            Factory.MultiplyMetricsProvider = new SelectedMultiMetricProvider(_postMetricProvider)
            {
                Indexes = indexes
            };
        }

        public int Size
        {
            get { return _postMetricProvider.Size; }
        }

        public IEnumerable<string> Names
        {
            get { return _postMetricProvider.Names; }
        }
    }
}