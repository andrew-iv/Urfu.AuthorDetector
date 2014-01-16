using System.Collections.Generic;
using System.Linq;
using Ninject;
using Opcorpora.Dictionary;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common
{
    public static class StaticVars
    {
        public static string[] TopRuWords { get; set; }
        public static string[] Top3Gramms { get; set; }
        public static IKernel Kernel;
        public static OpcorporaDictionary Opcorpora { get; set; }

        public static void InitializeTops(IEnumerable<string> tops,int topCount = 500)
        {
            var enumerable = tops as string[] ?? tops.ToArray();
            TopRuWords = enumerable.Select(x=>x.ToLower()).GetTopRuWords(topCount);
            Top3Gramms = enumerable.Select(x => x.ToLower()).GetTopGrammas(useFirst: topCount);
        }


    }
}