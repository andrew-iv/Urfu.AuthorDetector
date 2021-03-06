﻿using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IByesStats
    {
        double Probability(IEnumerable<double> item);
        int Count { get; }
    }


    public class PerecentileBayesClassifier : BayesClassifierBase
    {
        public PerecentileBayesClassifier(IDictionary<Author, IEnumerable<string>> authors, ICommonMetricProvider[] providers )
            : base(authors, providers)
        {
        }

        protected override IQuantilesInfo QuantilesInfoConstructor(int size, double[][] allMetrics)
        {
            return new QuantilesInfo(size,allMetrics,5);
        }

        protected override IByesStats ByesStatsConstructor(IQuantilesInfo quantilesInfo, IEnumerable<double[]> allMetrics)
        {
            return new QuantilesStats(quantilesInfo, allMetrics);
        }
    }
}