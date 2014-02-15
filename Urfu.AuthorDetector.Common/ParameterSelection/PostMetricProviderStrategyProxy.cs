using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    
    public class CommonProviderStrategyProxy : StrategyProxyBase, IPostMetricProviderStrategyProxy
    {
        private ICommonMetricProvider _postMetricProvider;

        public void Start()
        {
            _postMetricProvider = Factory.CommonMetricProviders[0];
        }

        public void End()
        {
            Factory.CommonMetricProviders[0] = _postMetricProvider;
        }

        public void SetIndexes(int[] indexes)
        {
            Factory.CommonMetricProviders[0] = new SelectedCommonMetricProvider(_postMetricProvider)
            {
                Indexes = indexes
            };
        }

        public bool IsCorrelated(int ind1, int ind2)
        {
            throw new System.NotImplementedException();
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