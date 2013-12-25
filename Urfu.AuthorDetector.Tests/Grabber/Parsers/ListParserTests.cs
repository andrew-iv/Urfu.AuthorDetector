using System;
using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Grabber;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    public class LorParserTests : TestsBase
    {
        private readonly FileLorPageLoader _pageLoader = new FileLorPageLoader();

        protected LorPostsParser Parser; // = new LorPostsParser();
        protected LorPostsParser RealParser;

        protected override void OnSetup()
        {
            base.OnSetup();
            Parser = new LorPostsParser(_pageLoader);
            RealParser = new LorPostsParser(new LorPageLoader());
        }
    }

    [TestFixture]
    public class ListParserTests : LorParserTests
    {
        [Test]
        public void TestListUser1()
        {
            var items = Parser.GetPostsList("user1").ToArray();
            Assert.AreEqual(50, items.Length);
            Assert.That(items.Select(x => x.Time).ToArray(), Is.All.GreaterThan(default(DateTime)));
            Assert.That(items.Select(x => x.ThemeId).ToArray(), Is.All.GreaterThan(default(int)));
            Assert.That(items.Select(x => x.PostId).ToArray(), Is.All.GreaterThan(default(int)));
            Assert.That(items.Select(x => x.Theme).ToArray(), Is.All.Not.Empty);
        }

        [Test]
        public void TestListArchive()
        {
            var items = Parser.ParseThemeLinks(0, 2012, 3).ToArray();
            Assert.AreEqual(46, items.Length);
            Assert.That(items, Is.All.Not.Empty);
            Assert.That(items, Is.All.StringContaining("/forum/talks/"));
            Assert.That(items, Is.Not.All.Not.StringContaining("page1"));
            Assert.That(items, Is.Not.All.Not.StringContaining("page2"));
            Assert.That(items, Is.Not.All.Not.StringContaining("page3"));
            Assert.That(items, Is.Not.All.Not.StringContaining("page4"));
            Assert.That(items, Is.All.Not.StringContaining("page5"));
        }
    }
}
