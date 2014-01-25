using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;
using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common.Classification
{
    public static class StatsHelper
    {
        public static List<int> GetNotNullDeviationIndexes(this double[][] doubles)
        {
            var indexes = new List<int>();
            if (doubles.Length == 0)
            {
                return indexes;
            }
            var frst = doubles[0];

            indexes.AddRange(Enumerable.Range(0, frst.Length)
                .Where(i => 
                    doubles.Where(x => x.Length > i).Select(x => x[i]).Distinct().Count() 
                    > 1)
                    );
            return indexes;
        }

        public static T[] GetOnIndexes<T>(this T[] doubles, IEnumerable<int> indexes)
        {
            return indexes.Select(x => doubles[x]).ToArray();
        }

        public static T[][] GetOnIndexes<T>(this T[][] doubles, IEnumerable<int> indexes)
        {
            return doubles.Select(x => x.GetOnIndexes(indexes)).ToArray();
        }



        public static string[] GetTopRuWords(this IEnumerable<string> posts, int useFirst = 500)
        {
            var words = new Dictionary<string, int>();
            foreach (var post in posts)
            {
                foreach (var gramm in post.RussianWords())
                {
                    if (words.ContainsKey(gramm))
                    {
                        words[gramm]++;
                    }
                    else
                    {
                        words[gramm] = 1;
                    }
                }
            }
            return words.OrderByDescending(x => x.Value).Take(useFirst).Select(x => x.Key).ToArray();
        }

        public static string[] GetTopGrammas(this IEnumerable<string> posts, int grammLength = 3, int useFirst = 500)
        {
            var gramms = new Dictionary<string, int>();

            foreach (var post in posts)
            {
                
                foreach (var gramm in  post.NGramms(grammLength))
                {
                    if (gramms.ContainsKey(gramm))
                    {
                        gramms[gramm]++;
                    }
                    else
                    {
                        gramms[gramm] = 1;
                    }

                }
            }
            return gramms.OrderByDescending(x => x.Value).Take(useFirst).Select(x => x.Key).ToArray();
        }
    }
}