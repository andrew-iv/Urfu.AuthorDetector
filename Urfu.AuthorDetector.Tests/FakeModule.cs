using Moq;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.DataLayer;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Tests.Grabber.Parsers;

namespace Urfu.AuthorDetector.Tests
{
    public class FakeModule : NinjectModule
    {
        private readonly DbContextBuilder _contextBuilder;

        public FakeModule(DbContextBuilder contextBuilder)
        {
            _contextBuilder = contextBuilder;
        }


        public Mock<ILorPageLoader> LorPageLoaderMock { get; set; }
        public Mock<ILorPostsParser> LorPostsParserMock { get; set; }
        public Mock<ILorStorage> LorStorageMock { get; set; }

        private static void RegisterContext(IKernel kernel)
        {

        }

        private void BindInSingletonScope<T1, T2>(IMock<T1> mock) where T1 : class where T2 : T1
        {
            if (mock == null)
            {
                Kernel.Bind<T1>().To<T2>().InSingletonScope();
            }
            else
            {
                Kernel.Bind<T1>().ToMethod(x => mock.Object).InTransientScope();
            }
        }

        private void BindInTransientScope<T1, T2>(IMock<T1> mock)
            where T1 : class
            where T2 : T1
        {
            if (mock == null)
            {
                Kernel.Bind<T1>().To<T2>().InTransientScope();
            }
            else
            {
                Kernel.Bind<T1>().ToMethod(x => mock.Object).InTransientScope();
            }
        }

        public override void Load()
        {
            Kernel.Bind<IStatisticsContext>().ToMethod(context => _contextBuilder.BuildMockDbContext().Object).InTransientScope();
            BindInSingletonScope<ILorPageLoader,FileLorPageLoader>(LorPageLoaderMock);
            BindInSingletonScope<ILorPostsParser,LorPostsParser>(LorPostsParserMock);
            BindInSingletonScope<ILorStorage, LorStorage>(LorStorageMock);
        }
    }
}