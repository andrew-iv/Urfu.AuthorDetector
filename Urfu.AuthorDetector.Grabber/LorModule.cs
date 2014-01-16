using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Grabber.Flamp;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Grabber
{
    public class RealModule : CommonModule
    {
        public RealModule()
        {
            NeedCreateDictionary = false;
        }

        private static void RegisterContext(IKernel kernel)
        {
        }

        public override void Load()
        {
            base.Load();
            Kernel.Bind<ILorPageLoader>().To<LorPageLoader>().InSingletonScope();
            Kernel.Bind<ILorPostsParser>().To<LorPostsParser>().InSingletonScope();
            Kernel.Bind<ILorGrabber>().To<LorGrabber>().InSingletonScope();

            Kernel.Bind<IFlampLoader>().To<FlampLoader>().InSingletonScope();
            Kernel.Bind<IFlampParser>().To<FlampParser>().InSingletonScope();
            Kernel.Bind<IFlampGrabber>().To<FlampGrabber>().InSingletonScope();
        }
    }
}