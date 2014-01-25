using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public abstract class BaseSelectedMetricProvider<TMetricProvider>  where TMetricProvider:  ICommonMetricProvider
    {
        protected TMetricProvider BaseProvider { get; private set; }
        private int[] _indexes;

        protected BaseSelectedMetricProvider(TMetricProvider baseProvider)
        {
            BaseProvider = baseProvider;
        }

        public IEnumerable<int> Indexes
        {
            get { return _indexes; }
            set { _indexes = (value as int[])??value.ToArray(); }
        }

        public IEnumerable<string> Names
        {
            get
            {
                var bn = BaseProvider.Names;
                
                return ((bn as string[])??bn.ToArray()).GetOnIndexes(Indexes);
            }
        }
        public int Size
        {
            get { return _indexes.Length; }
        }

        
    }
}