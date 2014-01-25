using Moq;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    public class LorGrabberTests : TestsBase
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            FakeModule.LorPostsParserMock = new Mock<ILorPostsParser>(MockBehavior.Loose)
                {
                    CallBase = true,
                    DefaultValue = DefaultValue.Empty,
                };
            FakeModule.LorPostsParserMock.SetReturnsDefault(new LorPostsParser(new FileLorPageLoader()));
        }
    }
}