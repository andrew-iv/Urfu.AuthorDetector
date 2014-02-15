using System.Collections.Generic;
using System.Linq;
using Ninject;
using Opcorpora.Dictionary;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.Common.StatMethods;

namespace Urfu.AuthorDetector.Common
{
    public static class StaticVars
    {
        public static string[] TopRuWords { get; set; }
        public static string[] Top3Gramms { get; set; }
        public static IKernel Kernel;
        public static OpcorporaDictionary Opcorpora { get; set; }

        public static PcaMetricTransformer MultyMetricTransformer { get; set; }
        public static PcaMetricTransformer SingleMetricTransformer { get; set; }


        public static void InitializeTops(IEnumerable<string> tops,int topChars = 500, int topWords = 1000)
        {
            var enumerable = tops as string[] ?? tops.ToArray();
            TopRuWords = enumerable.Select(x => x.ToLower()).GetTopRuWords(topWords);
            Top3Gramms = enumerable.Select(x => x.ToLower()).GetTopGrammas(useFirst: topChars);
/*
            var mp = Kernel.Get<IMultiplyMetricsProvider>();
            MultyMetricTransformer = new PcaMetricTransformer(enumerable.SelectMany(mp.GetMetrics));

            var pp = Kernel.Get<IPostMetricProvider>();
            MultyMetricTransformer = new PcaMetricTransformer(enumerable.Select(xx => pp.GetMetrics(xx).ToArray()));*/
        }

        


    }
}