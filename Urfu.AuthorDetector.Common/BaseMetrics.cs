using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;
using System.Linq;

namespace Urfu.AuthorDetector.Common
{
    public abstract class BaseMetric:IMetric
    {
        public virtual IEnumerable<KeyValuePair<string, double>> MetricValues
        {
            get { return new Dictionary<string, double>(); }
        }

        public virtual IEnumerable<string> Arguments { get { return MetricValues.Select(x=>x.Key); }}

    }
}