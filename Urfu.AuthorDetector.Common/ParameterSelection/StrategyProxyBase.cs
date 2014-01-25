using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public abstract class StrategyProxyBase
    {
        public IClassifierFactory Factory { get; set; }
    }
}