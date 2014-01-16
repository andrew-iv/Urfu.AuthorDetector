using System;
using System.Configuration;
using Ninject.Modules;
using Opcorpora.Dictionary;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public class CommonModule : NinjectModule
    {
        private bool _needCreateDictionary = true;


        private OpcorporaDictionary CreateDictionary()
        {
            if (StaticVars.Opcorpora != null) return StaticVars.Opcorpora;
            var res = new OpcorporaDictionary(ConfigurationManager.AppSettings.Get("OpcorporaDictionary"));
            GC.Collect();
            StaticVars.Opcorpora = res;
            return res;
        }

        public bool NeedCreateDictionary
        {
            get { return _needCreateDictionary; }
            set { _needCreateDictionary = value; }
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

            Kernel.Bind<IPostsQueryFilter>().To<PostsQueryFilter>().InThreadScope();
            
            if (NeedCreateDictionary)
            {
                Kernel.Bind<IOpcorporaDictionary>().ToConstant(CreateDictionary()).InSingletonScope();
            }

            Kernel.Bind<IMultiplyMetricsProvider>().ToConstructor(x => new CombinedMetricProvider(
                    new LengthMetricProvider(),
                    new GramemmeMetricProvider(),
                    new PunctuationMetricProvider()
                    )).InSingletonScope();


            Kernel.Bind<IPostMetricProvider>().To<SelectedPostMetricProvider>().InSingletonScope();

        }
    }

    public class LorModule : NinjectModule

{
        public override void Load()
        {
            Kernel.Bind<IForumStorage>().To<LorStorage>().InTransientScope();
            Kernel.Bind<IDataExtractor>().To<LorDataExtractor>().InThreadScope();
        }

        public override void Unload()
        {
            Kernel.Unbind<IForumStorage>();
            Kernel.Unbind<IDataExtractor>();
        }
}
}