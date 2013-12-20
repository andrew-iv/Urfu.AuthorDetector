using System;
using System.Configuration;
using Ninject.Modules;
using Opcorpora.Dictionary;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public class CommonModule:NinjectModule
    {
        private OpcorporaDictionary CreateDictionary()
        {
            var res = new OpcorporaDictionary(ConfigurationManager.AppSettings.Get("OpcorporaDictionary"));
            GC.Collect();
            return res;
        }

        public override void Load()
        {
            Kernel.Bind<IStatisticsContext>().ToMethod(context =>
                {
                    var realContext = new StatisticsContainer();
                    realContext.Configuration.AutoDetectChangesEnabled = false;
                    realContext.Configuration.ProxyCreationEnabled = false;
                    realContext.Configuration.LazyLoadingEnabled = false;
                    return realContext;
                }).InTransientScope();
            Kernel.Bind<IForumStorage>().To<FlampStorage>().InTransientScope();
            Kernel.Bind<IPostsQueryFilter>().To<PostsQueryFilter>().InThreadScope();
            Kernel.Bind<IDataExtractor>().To<LorDataExtractor>().InThreadScope();
            Kernel.Bind<IOpcorporaDictionary>().ToConstant(CreateDictionary()).InSingletonScope();


        }

    }
}