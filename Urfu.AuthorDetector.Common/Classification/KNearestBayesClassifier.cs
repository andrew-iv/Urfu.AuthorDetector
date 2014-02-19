using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Math;
using Accord.Math;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public abstract class ClassifierBase : IClassifier
    {
        protected IDictionary<Author, IEnumerable<string>> AuthorsTexts;
        protected int[] AllIndex;
        protected double[][] AllMetrics;
        protected ICommonMetricProvider MetricProvider;
        protected IDictionary<Author, double[][]> AuthorMetrics;
        protected Dictionary<Author, int> KeysDict { get; private set; }

        public virtual void Init(IDictionary<Author, IEnumerable<string>> authors,
                                       ICommonMetricProvider metricProvider)
        {
            AuthorsTexts = authors;
            MetricProvider = metricProvider;
            var keys = AuthorsTexts.Keys.ToArray();
            AuthorMetrics = authors.ToDictionary(x => x.Key,
                                                   x => Enumerable.ToArray(metricProvider.GetMetrics(x.Value)));
            //_matrix = GetMatrix(_authorMetrics.Select(x => x.Value));

            KeysDict = Enumerable.Range(0, keys.Length).ToDictionary(keys.ElementAt);

            AllIndex = AuthorMetrics.SelectMany(x =>  x.Value.Select(xx =>
                                                                 KeysDict[x.Key])).ToArray();
            AllMetrics = AuthorMetrics.SelectMany(x => x.Value).ToArray();

        }

        public void LogResult(bool isSuccess)
        {
        }

        public IEnumerable<Author> Authors { get { return KeysDict.Keys; } }
        public Author ClassificatePosts(IEnumerable<string> posts, out bool reliable)
        {
            reliable = true;
            return ClassificatePosts(posts, 1)[0];
        }

        public abstract Author[] ClassificatePosts(IEnumerable<string> posts,int topN);
        public abstract string Description { get; }
        public abstract string Name { get; }
        public double ErrorLevel { get; set; }
    }

    public interface IKNearestClassifier: IClassifier
    {
        int K { get; set; }
    }

    /// <summary>
    /// классификатор метода ближайших сособей с байесовской оценкой плотности
    /// </summary>
    public class KNearestBayesClassifier : KNearestClassifierBase, IClassifier
    {
        public override Author[] ClassificatePosts(IEnumerable<string> posts, int topN)
        {
            var alpha = 0.25d / K /K;
            var vars = Enumerable.ToArray(MetricProvider.GetMetrics(posts).Select(x =>
                    {
                        double[] scores;
                        _classifier.Compute(x, out scores);
                        return scores;
                    }).Aggregate(Enumerable.Range(0,KeysDict.Count).Select(x=>1d).ToArray(),
                                 (x1, x2) =>
                                     {
                                         var mulRes =
                                             Enumerable.Range(0, KeysDict.Count).Select(i => x1[i]
                                                                                             *(x2[i]+alpha)).ToArray();
                                         var max = mulRes.Max();
                                         return mulRes.Select(x => x/max).ToArray();
                                     }
                              ));
            var ind = Enumerable.Range(0, KeysDict.Count).OrderByDescending(x => vars[x]).Take(topN).ToArray();
            return ind.Select(i=>KeysDict.First(x=>x.Value == i).Key).ToArray();
        }
    }
}