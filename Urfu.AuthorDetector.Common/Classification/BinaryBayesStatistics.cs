using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    class BinaryBayesStatistics : BinaryBayesBase
    {
        public BinaryBayesStatistics(IEnumerable<string> myMessages, IEnumerable<string> notMyMessages, params ICommonMetricProvider[] providers) : base(myMessages, notMyMessages, providers)
        {
        }

        

        public void LogResult(bool isMy,IEnumerable<string> txt )
        {
            var logger = StaticVars.Kernel.Get<IBayesResultLogger>();
            if (logger == null) return;
            var strings = txt as string[] ?? txt.ToArray();
            logger.Log(new BayesClassifierTest()
                {
                    FirstToAll = TextProbab(strings),
                    MessageCount = strings.Count(),
                    MessagesLength = strings.Sum(x => x.Length),
                    Success = isMy
                });
        } 
    }
}