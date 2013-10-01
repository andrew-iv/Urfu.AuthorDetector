using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public interface IMetric
    {
        IEnumerable<KeyValuePair<string, double>> MetricValues { get; }
        IEnumerable<string> Arguments { get; }
    }

    public interface IFillableMetric:IMetric
    {
        void FillFromPost(string post);
    }

    public interface IGrammsMetric : IMetric
    {
        string[] UseGramms { set; }
    }


}