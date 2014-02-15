using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.MetricProvider.Sentance
{
    public class SelectedMultiMetricProvider : BaseSelectedMetricProvider<IMultiplyMetricsProvider>, IMultiplyMetricsProvider
    {
        public SelectedMultiMetricProvider(IMultiplyMetricsProvider baseProvider)
            : base(baseProvider)
        {
        }

        public IEnumerable<double[]> GetMetrics(string text)
        {
            var metr = BaseProvider.GetMetrics(text);
            return metr.Select<double[], double[]>(x=>x.GetOnIndexes(Indexes));
        }

        

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return text.SelectMany(GetMetrics).ToArray();
        }
    }

    public class SelectedCommonMetricProvider : BaseSelectedMetricProvider<ICommonMetricProvider>, ICommonMetricProvider
    {
        public SelectedCommonMetricProvider(ICommonMetricProvider baseProvider)
            : base(baseProvider)
        {
        }

        


        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return base.BaseProvider.GetMetrics(text).Select(x => x.GetOnIndexes(Indexes)).ToArray();
        }
    }
}