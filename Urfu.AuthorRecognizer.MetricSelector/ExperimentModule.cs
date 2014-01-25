using System;
using Ninject.Modules;
using Urfu.AuthorDetector.Common.Experiment;

namespace Urfu.AuthorRecognizer.MetricSelector
{
    public class ExperimentModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDataSource>().ToMethod(x => new DataSource()
                {
                    ForumId = 3,
                    DateStart = new DateTime(2012, 1, 1)
                }).InTransientScope();
        }
    }
}