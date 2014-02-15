using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class StupidPerecentileBayesClassifier : BayesClassifierBase
    {
        public StupidPerecentileBayesClassifier(IDictionary<Author, IEnumerable<string>> authors, ICommonMetricProvider[] providers)
            : base(authors, providers)
        {
        }

        protected override IQuantilesInfo QuantilesInfoConstructor(int size, double[][] allMetrics)
        {
            return new QuantilesInfo(size, allMetrics, 20);
        }

        protected override IByesStats ByesStatsConstructor(IQuantilesInfo quantilesInfo, IEnumerable<double[]> allMetrics)
        {
            return new StuipidQuantilesStats(quantilesInfo, allMetrics);
        }
    }
}