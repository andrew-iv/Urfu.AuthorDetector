using System;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IBinaryClassifierBenchmark:IBenchmark
    {
        Tuple<double,double> Score(IBinaryClassifierFactory factory, int seed = 0);
        int ChangeAuthorCount { get; set; }
    }
}