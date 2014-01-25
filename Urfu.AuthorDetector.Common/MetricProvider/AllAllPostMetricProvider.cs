using Ninject;
using Opcorpora.Dictionary;
using System.Linq;

namespace Urfu.AuthorDetector.Common.MetricProvider
{

    public class AllAllPostMetricProvider : BaseAllPostMetricProvider
    {
        protected override string[] UseNgramms
        {
            get { return StaticVars.Top3Gramms; }
        }

        protected override string[] UseWords
        {
            get { return StaticVars.TopRuWords; }
        }

        protected override string[] Grammemes
        {
            get { return null; return StaticVars.Kernel.Get<IOpcorporaDictionary>().Grammemes.Select(x => x.name).Where(x=>x!=null).ToArray(); }
        }
    }
}