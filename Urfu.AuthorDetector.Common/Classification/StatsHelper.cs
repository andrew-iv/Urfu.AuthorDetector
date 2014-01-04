using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;
using MathNet.Numerics.Statistics;

namespace Urfu.AuthorDetector.Common.Classification
{
    public static class StatsHelper
    {



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