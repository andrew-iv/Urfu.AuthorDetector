using Ninject;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber
{
    public class RealModule : CommonModule
    {

        private static void RegisterContext(IKernel kernel)
        {
            
        }

        public override void Load()
        {
            base.Load();
            Kernel.Bind<ILorPageLoader>().To<LorPageLoader>().InSingletonScope();
            Kernel.Bind<ILorPostsParser>().To<LorPostsParser>().InSingletonScope();
            Kernel.Bind<ILorGrabber>().To<LorGrabber>().InSingletonScope();
        }
    }
}