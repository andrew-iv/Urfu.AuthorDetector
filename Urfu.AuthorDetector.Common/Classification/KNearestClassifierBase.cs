using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.MachineLearning;
using Accord.Statistics;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;
using Distance = Accord.Math.Distance;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// Базовый функционал метода ближайших соседей
    /// </summary>
    public abstract class KNearestClassifierBase : ClassifierBase, IKNearestClassifier
    {
        protected double[,] _matrix;

        protected int _k = 25;
        protected KNearestNeighbors _classifier;

        public int K
        {
            get { return _k; }
            set
            {
                if(value <= 0)
                    throw new PropertyConstraintException("K must be >= 1");
                _k = value;
            }
        }

        public override string Description
        {
            get { return "Ближайшие соседи"; }
        }

        protected virtual double[,] GetMatrix(IEnumerable<double[][]> classMetrics)
        {
            return Tools.Covariance(classMetrics.First());
        }

        public override string Name { get { return "KNearestBayesClassifier"; } }

        public override void Init(IDictionary<Author, IEnumerable<string>> authors,
                                  ICommonMetricProvider metricProvider)
        {
            base.Init(authors,metricProvider);
            _matrix = GetMatrix(AuthorMetrics.Select(x => x.Value));
            _classifier = new KNearestNeighbors(K,authors.Count, AllMetrics, AllIndex,
                                                (x, y) => Distance.Mahalanobis(x, y, (double[,]) _matrix));

        }
    }
}