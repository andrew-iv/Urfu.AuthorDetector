using System.Collections.Generic;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IPostMetricProviderStrategyProxy
    {
        IClassifierFactory Factory { get; set; }
        bool IsCorrelated(int ind1, int ind2);


        int Size { get; }
        IEnumerable<string> Names { get;  }
        void Start();
        void End();
        void SetIndexes(  int[] indexes);
    }
}