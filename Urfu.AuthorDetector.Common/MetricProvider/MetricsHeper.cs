using System;
using System.Collections.Generic;
using System.Linq;
using EeekSoft.Text;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public class MetricsHeper
    {
        public static IEnumerable<double> SubstringMetrics(string[] keywords, string text, int length)
        {
            if (length > 0)
            {
                var ss = new StringSearch(keywords);
                var dictCount = new Dictionary<string, int>(keywords.Count());
                var allRes = ss.FindAll(text.ToLower()).GroupBy(x => x.Keyword).ToDictionary(x => x.Key, x => x.Count());
                foreach (var x in keywords)
                {
                    int val;
                    allRes.TryGetValue(x, out val);
                    yield return Convert.ToDouble(val) / length;
                }
            }
            else
            {
                foreach (var i in Enumerable.Range(0, keywords.Length))
                {
                    yield return 0d;
                }
            }
        }
    }
}