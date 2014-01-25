using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.MetricProvider.Sentance
{
    public class SelectedSentenceMetricProvider : IMultiplyMetricsProvider
    {
        private ISentenceMetricProvider _baseProvider;
        private int[] _indexes;


        public SelectedSentenceMetricProvider(int[] indexes, ISentenceMetricProvider baseProvider)
        {
            _indexes = indexes;
            _baseProvider = baseProvider;
            var arrNames = _baseProvider.Names.ToArray();
            Names = indexes.Select(i => arrNames[i]).ToArray();
            Size = Names.Count();
        }

        public IEnumerable<string> Names { get; private set; }
        public int Size { get; private set; }
        public IEnumerable<double[]> GetMetrics(string text)
        {
            return _baseProvider.GetMetrics(text).Select(metric =>
                {
                    var arr = metric.ToArray();
                    return _indexes.Select(i => arr[i]).ToArray();
                });
        }

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return text.SelectMany(GetMetrics).ToArray();
        }
    }
}