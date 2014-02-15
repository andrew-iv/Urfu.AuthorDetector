using System;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public static class SomeSelectedMetricProviders
    {
        private static ICommonMetricProvider _chi2Test4;
        public static ICommonMetricProvider Chi2Test4
        {
            get
            {
                return _chi2Test4 ?? (_chi2Test4 =
                    new CombinedCommonMetricProvider(
                    new SelectedPostMetricProvider(
                  new SimpleStatMetricProvider()) { Indexes = new int[] { 0, 1, 2, 3, 5, 6 } },
                    new SubstringMetricProvider(new string[] { " и ", " - ", "тся", ".\n\n", ", н", "...", "   ",
                        " а ", "е, ", ", п", "о, ", "а, ", ", т", ", к", ", ч", "\n\n\n", ", и", "еще", "зы", "тс", "ж" }),
                  new GrammemesPostMetricProvider(new[] { "NOUN", "ADVB", "PREP", "CONJ","PRCL","INTJ", "inan", "Fixd","Orgn" }, true),
                  new PunctuationPostMetricProvider(new[] { '.', ',', '-', '!', '(', ')' })
                    ));
            }
        }

        private static ICommonMetricProvider _chi2Test3;
        public static ICommonMetricProvider Chi2Test3
        {
            get
            {
                return _chi2Test3 ?? (_chi2Test3 =
                    new CombinedCommonMetricProvider(
                    new SelectedPostMetricProvider(
                  new SimpleStatMetricProvider()) { Indexes = new int[] { 0, 1, 2, 3, 5, 6 } },
                    new SubstringMetricProvider(new string[] { " и ", " - ", "\n\n\n", ", н", "...", "   ", " а ", "е, ", ", п", "о, ", "а, ", ", т", ", ч", ".\n\n", ", и", "зы" }),
                  new GrammemesPostMetricProvider(new[] { "NOUN",  "CONJ", "PRCL", "INTJ", "inan", "Fixd" }, true),
                  new PunctuationPostMetricProvider(new[] { '.', '-', '!' })
                    ));
            }
        }


        private static ICommonMetricProvider _addSelection1;

        public static ICommonMetricProvider AddSelection1
        {
            get
            {
                return _addSelection1 ?? (_addSelection1 =
                                          new CombinedCommonMetricProvider(
                                              new SelectedPostMetricProvider(
                                                  new SimpleStatMetricProvider())
                                                  {
                                                      Indexes = new int[] {3, 6}
                                                  },
                                              new SubstringMetricProvider(new string[]
                                                  {
                                                      " - ", ".\n\n", ", н", "...", "   ",
                                                      ", п", ", т", "\n\n\n", ", и", "еще", "зы"
                                                  }),
                                              new GrammemesPostMetricProvider(new[] {"Orgn"}, true),
                                              new PunctuationPostMetricProvider(new[] {'.', '!', '(', ')'})
                                              ));
            }
        }


        private static ICommonMetricProvider _addSelection2;
        public static ICommonMetricProvider AddSelection2
        {
            get
            {
                return _addSelection2 ?? (_addSelection2 =
                    new CombinedCommonMetricProvider(
                    new SelectedPostMetricProvider(
                  new SimpleStatMetricProvider()) { Indexes = new int[] { 3, 5, 6 } },
                    new SubstringMetricProvider(new string[] { " и ", " - ", ".\n\n", ", н", "...", "   ",
                        " а ", ", п", "о, ", "а, ", ", т", ", к", ", ч", "\n\n\n", ", и", "еще", "зы" }),
                  new GrammemesPostMetricProvider(new[] {  "Fixd","Orgn" }, true),
                  new PunctuationPostMetricProvider(new[] { '.', '!', '(', ')' })
                    ));
            }
        }

    }
}