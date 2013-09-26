using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common
{
    public class DictionaryMetric : BaseMetric
    {
        private readonly IEnumerable<KeyValuePair<string, double>> _dictionary;

        public DictionaryMetric(IEnumerable<KeyValuePair<string, double>> dictionary)
        {
            _dictionary = dictionary.ToArray();
        }

        public override IEnumerable<KeyValuePair<string, double>> MetricValues
        {
            get { return _dictionary; }
        }
    }
}