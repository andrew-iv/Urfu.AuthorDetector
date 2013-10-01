using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
   /* public class GrammsMetricNeighboorClassifier<TMetric> : NeighboorClassifier where TMetric : IFillableMetric, IGrammsMetric, new()
    {
        protected string[] Gramms;
        protected override void AdditionalInitialization(IDictionary<Author, IEnumerable<string>> authors)
        {
            Gramms = authors.SelectMany(xx => xx.Value).GetTopGrammas();
        }
        public GrammsMetricNeighboorClassifier(IDictionary<Author, IEnumerable<string>> authors)
            : base(authors)
        {
            
            
        }

        protected override IMetric CallculateMetric(string post)
        {
            var metric = new TMetric() { UseGramms = Gramms };
            metric.FillFromPost(post);
            return metric;
        }
    }*/
}