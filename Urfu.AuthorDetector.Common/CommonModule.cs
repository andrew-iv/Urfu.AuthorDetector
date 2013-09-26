using Ninject.Modules;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public class CommonModule:NinjectModule
    {
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
            Kernel.Bind<ILorStorage>().To<LorStorage>().InTransientScope();
            Kernel.Bind<IPostsQueryFilter>().To<PostsQueryFilter>().InThreadScope();
        }

    }
}