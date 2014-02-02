using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common
{
    /// <summary>
    /// Группирует тексты по K
    /// </summary>
    public class GroupByNMetricProvider : ICommonMetricProvider
    {
        public int K { get; set; }
        private readonly ISingleMetricProvider _provider;

        public GroupByNMetricProvider(ISingleMetricProvider provider, int k=5)
        {
            K = k;
            _provider = provider;
        }

        public IEnumerable<string> Names { get { return _provider.Names; } }
        public int Size { get { return _provider.Size; } }
        public double[][] GetMetrics(IEnumerable<string> text)
        {
            if(K<=1) throw new ArgumentOutOfRangeException("K","K must be >1");

            var arr = text.ToArray();

            return
                Enumerable.Range(0, (arr.Length - 1)/K + 1)
                          .Select(x => _provider.GetMetrics(arr.Skip(x*K).Take(K)))
                          .ToArray();

        }
    }
}