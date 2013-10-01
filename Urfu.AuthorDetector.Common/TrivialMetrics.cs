using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;
using MathNet.Numerics;

namespace Urfu.AuthorDetector.Common
{
    public class TrigrammsTrivialMetric :TrivialMetric, IGrammsMetric
    {
        private IEnumerable<KeyValuePair<string, double>> _gramms = null;

        public string[] UseGramms { set; private get; }

        public override void FillFromPost(string post)
        {
            base.FillFromPost(post);
            if (UseGramms != null)
            {
                _gramms = UseGramms.Select(useGramm =>
                    {
                        if(Math.Abs(Length - 0) < 0.001)
                            return new KeyValuePair<string, double>("Gramm_" + useGramm, 0);
                        int occurs = 0;
                        var txt = PureText.ToLower();
                        for (int i = txt.IndexOf(useGramm); i >= 0; i = txt.IndexOf(useGramm, i + 1))
                        {
                            occurs++;
                        }
                        return  new KeyValuePair<string,double>("Gramm_"+useGramm ,occurs/Length);

                    }).ToList();
            }
        }

        private IEnumerable<KeyValuePair<string, double>> _metrics = null;

        public override IEnumerable<KeyValuePair<string, double>> MetricValues
        {
            get
            {
                return _gramms.Concat(base.MetricValues).OrderBy(x => x.Key).ToArray();
            }
        }
    }

    public class TrivialMetric : BaseMetric, IFillableMetric
    {


        public const int NewDayInSeconds = 1 * 3600;

        /// <summary>
        /// Длина сообщения
        /// </summary>
        public double Length { get; private set; }

        protected string PureText { get; private set; }

        /// <summary>
        /// Время. Отсчет начинается с NewDayInSeconds в секундах начиная от 00:00:00( GMT 0)
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// Доля пробельных символов
        /// </summary>
        public double WhitespacesShare { get; private set; }


        /// <summary>
        /// соотношение абзацев к длине
        /// </summary>
        public double ParagraphsShare { get; private set; }

        /// <summary>
        /// соотношение Пунктуационных знаков к длине
        /// </summary>
        public double PunctuationShare { get; private set; }

        /// <summary>
        /// соотношение Пунктуационных знаков к длине
        /// </summary>
        public double OtherNodesShare { get; private set; }

        /// <summary>
        /// средняя длина слова
        /// </summary>
        public double RussianWordLength { get; private set; }

     /*   /// <summary>
        /// сколько гласных
        /// </summary>
        public double VowelCount { get; set; }*/

        public override IEnumerable<KeyValuePair<string, double>> MetricValues
        {
            get
            {
                return new Dictionary<string, double>()
                    {
                        {"Length", this.Length},
                 //       {"Time", this.Time},
                        {"WhitespacesShare", this.WhitespacesShare},
                 //       {"ParagraphsShare", this.ParagraphsShare},
                        {"PunctuationShare", this.PunctuationShare},
                     //   {"OtherNodesShare", this.OtherNodesShare},
                     //   {"RussianWordLength", this.RussianWordLength},
                 //       {"VowelCount", this.VowelCount},
                    };
            }
        }

        public virtual void FillFromPost(string post)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(post);
            List<HtmlNode> nodes;
            var paragraphs = HtmlExtractor.ExtractParagrahps(htmlDoc, out nodes);
            PureText = string.Join(" ", paragraphs);
            Length = PureText.Length;
            if (Math.Abs(Length - 0d) > 0.1d)
            {
                ParagraphsShare = Convert.ToDouble(paragraphs.Count) / Length;
                PunctuationShare = Convert.ToDouble(PureText.Count(Char.IsPunctuation)) / Length;
                WhitespacesShare = Convert.ToDouble(PureText.Count(Char.IsWhiteSpace)) / Length;
                OtherNodesShare = Convert.ToDouble(nodes.Count) / Length;
                var words = PureText.RussianWords().ToArray();
                if (words.Any())
                {
                    RussianWordLength = words.Average(x => x.Length);
              //      VowelCount = words.Average(x => x.VowelCount());
                }
            }
        }


        public TrivialMetric()
        {

        }

        public TrivialMetric(IEnumerable<TrivialMetric> posts)
        {
            Length = posts.Average(x => x.Length);
            ParagraphsShare = posts.Average(x => x.ParagraphsShare);
            OtherNodesShare = posts.Average(x => x.OtherNodesShare);
            PunctuationShare = posts.Average(x => x.PunctuationShare);
            WhitespacesShare = posts.Average(x => x.WhitespacesShare);
            Time = posts.Average(x => x.Time);
        }

        public TrivialMetric(string post)
        {
            FillFromPost(post);
        }
    }
}