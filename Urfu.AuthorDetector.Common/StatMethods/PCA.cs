using System.Collections.Generic;
using System.Web.UI;
using Accord.Math;
using Accord.Statistics.Analysis;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;
using IDataSource = Urfu.AuthorDetector.Common.Experiment.IDataSource;

namespace Urfu.AuthorDetector.Common.StatMethods
{
    public interface IMetricTransformer : IMetricProviderInfo
    {
        IEnumerable<IEnumerable<double[]>> MetricByAuthor { get; }

        double[] GetMetric(IEnumerable<double> doubles);

     //   void ApplyFilter<T>(IEnumerable<T> byAuthor, IMetricValuenceFilter filter) where T : IEnumerable<double[]>;
    }

    public class MultiplyMetricProviderAndTransformer: IMultiplyMetricsProvider
    {
        private readonly IMultiplyMetricsProvider _provider;
        private readonly IMetricTransformer _transformer;

        public MultiplyMetricProviderAndTransformer(IMultiplyMetricsProvider provider, IMetricTransformer transformer)
        {
            _provider = provider;
            _transformer = transformer;
        }

        public IEnumerable<string> Names { get { return _transformer.Names; } }
        public int Size { get { return _transformer.Size; } }
        public IEnumerable<double[]> GetMetrics(string text)
        {
            return _provider.GetMetrics(text).Select(_transformer.GetMetric);
        }

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return text.SelectMany(GetMetrics).ToArray();
        }
    }

    public class SimpleMetricProviderAndTransformer : IPostMetricProvider
    {
        private readonly IPostMetricProvider _provider;
        private readonly IMetricTransformer _transformer;

        public SimpleMetricProviderAndTransformer(IPostMetricProvider  provider, IMetricTransformer transformer)
        {
            _provider = provider;
            _transformer = transformer;
        }

        public IEnumerable<string> Names { get { return _transformer.Names; } }
        public int Size { get { return _transformer.Size; } }
        public double[] GetMetrics(string text)
        {
            return Enumerable.ToArray(_transformer.GetMetric(_provider.GetMetrics(text)));
        }

        public double[][] GetMetrics(IEnumerable<string> text)
        {
            return text.Select(GetMetrics).ToArray();
        }
    }



    public class PcaMetricTransformer : IMetricTransformer
    {

        public static MultiplyMetricProviderAndTransformer CreateMultiplyMetricProvider(float treshold, out PcaMetricTransformer pca, AnalysisMethod? method = AnalysisMethod.Center)
        {
            var ds = StaticVars.Kernel.Get<IDataSource>();
            
            var mp = StaticVars.Kernel.Get<IMultiplyMetricsProvider>();
            var posts = ds.GetPosts(10).SelectMany(x => x.Value.Take(200).SelectMany(mp.GetMetrics));
            pca = method.HasValue ? new PcaMetricTransformer(posts, 3, method.Value) : new PcaMetricTransformer(posts, 20);
            pca.SetComponents(treshold);
            return new MultiplyMetricProviderAndTransformer(mp
                        , pca);
        }

        public static SimpleMetricProviderAndTransformer CreateSimpeMetricProvider(float treshold,out PcaMetricTransformer pca,AnalysisMethod? method = AnalysisMethod.Center)
        {
            var ds = StaticVars.Kernel.Get<IDataSource>();
            
            var mp = StaticVars.Kernel.Get<IPostMetricProvider>();
            var posts = ds.GetPosts(10).SelectMany(x => x.Value.Take(400).Select(
                mp.GetMetrics));
            pca = method.HasValue ? new PcaMetricTransformer(posts, 20, method.Value) : new PcaMetricTransformer(posts, 20);
            pca.SetComponents(treshold);
            return new SimpleMetricProviderAndTransformer(mp
                        , pca);
        }



        /*private class PCAMetric:BaseMetric
        {
            private IEnumerable<KeyValuePair<string, double>> _metricValues;

            public PCAMetric(IEnumerable<PrincipalComponent> comps,IEnumerable<double> oldVals )
            {
                var principalComponents = comps as PrincipalComponent[] ?? comps.ToArray();
                var oldValsArr = oldVals as double[] ?? oldVals.ToArray();
                
                _metricValues = principalComponents.Select(x =>
                    new KeyValuePair<string, double>(
                        string.Format("Component_{0}", x.Index),
                            oldValsArr.MultiplyAndSum(x.Value)
                        ));
            }

            public override IEnumerable<KeyValuePair<string, double>> MetricValues
            {
                get { return _metricValues; }
                
            }
        }*/

        
        private int _topComponents;
        private readonly double[][] _metrics;
        private readonly PrincipalComponentAnalysis _pca;

        public PrincipalComponentAnalysis PCA { get { return _pca; } }

        private List<PrincipalComponent> _components;
        private readonly List<int> _indexes;

        public PcaMetricTransformer(IEnumerable<double[]> metrics, int topComponents = 5, AnalysisMethod method = AnalysisMethod.Standardize)
        {
            var metricsArray = metrics.ToArray();
            _topComponents = topComponents;
            _indexes = metricsArray.GetNotNullDeviationIndexes();
            _metrics = metricsArray.GetOnIndexes(_indexes);
            _pca = new PrincipalComponentAnalysis(_metrics, method);
            _pca.Compute();

        }

        public IEnumerable<IEnumerable<double[]>> MetricByAuthor { get; private set; }

        public double[] GetMetric(IEnumerable<double> doubles)
        {
            var asArray = doubles as double[] ?? doubles.ToArray();
            return _pca.Transform(asArray.GetOnIndexes(_indexes),_topComponents);
        }

        /// <summary>
        /// Устанавливает необходимое число компонентов чтобы объяснить долю вариации
        /// </summary>
        /// <param name="threshold">The percentile of the data requiring representation.</param>
        public int SetComponents(float threshold)
        {
            return _topComponents = _pca.GetNumberOfComponents(threshold);
        }


        /*
        public void ApplyFilter<T>(IEnumerable<T> byAuthor, IMetricValuenceFilter filter) where T : IEnumerable<double[]>
        {
            _components = new List<PrincipalComponent>();
            MetricByAuthor = byAuthor.Select(x=>x as IEnumerable<double[]>);
            _authorsMetrics = byAuthor.Select(x => x.CreateMatrix()).ToArray();
            _filter = filter;
            foreach (var comp in _pca.Components)
            {
                var vals = comp.Value.ToArray();
                if (!filter.TestAll(_metrics.Rows.MultiplyAndSum(vals))) continue;

                if (!
                _filter.TestByOtherAuthors(
                    _authorsMetrics.Select(author => author.Rows.MultiplyAndSum(vals))
                    )) continue;
                _components.Add(comp);
                if (_components.Count >= _topComponents)
                    return;
            }
        }
        */

        public IEnumerable<PrincipalComponent> Components
        {
            get
            {
                return
                    _components ?? _pca.Components.Take(_topComponents);
            }
        }
        ///
        public IEnumerable<string> Names { get { return Components.Select(x =>
                    string.Format("Component_{0}", x.Index)
                            ) ; } }
        public int Size { get { return Names.Count(); } }
    }
}