﻿using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// фабрика для KNearestSumClassifier
    /// </summary>
    public class KNearestSumClassifierFactory : BaseClassifierFactory, IKNearestClassifierFactory
    {
        private int _k=25;

        public int K
        {
            get { return _k; }
            set { _k = value; }
        }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            var cls = new KNearestSumClassifier() { K = K };
            cls.Init(authors, CommonMetricProviders.First());
            return cls;
        }
    }
}