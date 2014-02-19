using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// Классификатор с суммой ближайших соседей
    /// </summary>
    public class KNearestSumClassifier : KNearestClassifierBase, IClassifier
    {
        public override Author[] ClassificatePosts(IEnumerable<string> posts, int topN)
        {
            var vars = Enumerable.ToArray(MetricProvider.GetMetrics(posts).Select(x =>
                {
                    double[] scores;
                    _classifier.Compute(x, out scores);
                    return scores;
                }).Aggregate(Enumerable.Range(0, KeysDict.Count).Select(x => 1d).ToArray(),
                             (x1, x2) => Enumerable.Range(0, KeysDict.Count).Select(i => x1[i]
                                                                                         +x2[i]).ToArray()));
            var ind = Enumerable.Range(0, KeysDict.Count).OrderByDescending(x => vars[x]).Take(topN).ToArray();
            return ind.Select(i => KeysDict.First(x => x.Value == i).Key).ToArray();
        }
    }
}