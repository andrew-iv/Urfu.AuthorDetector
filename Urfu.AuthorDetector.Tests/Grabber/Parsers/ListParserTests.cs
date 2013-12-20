using System;
using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
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

    [TestFixture]
    public class ParsePostTests : LorParserTests
    {
        [Test]
        public void FillPost1()
        {
            var post = Parser.FillPost(new PostBrief() { PostId = 9587173, ThemeId = 9586947 });
            Assert.That(post.HtmlText, Is.StringContaining("Лучше под J2ME"));
            Assert.That(post.HtmlText, Is.Not.StringContaining("<div"));
        }

        [Test]
        public void ParsePostsInTheme1()
        {
            const int themeId = 9591916;
            var msgMustContains = new[]
                {
                    "Чтобы больше было объектов для ненависти.",
                    "Кем гарантируется работоспособность приложений от андроид?"
                };
            var authorMustContains = new[]
                {
                    "Reset",
                    "vurdalak",
                    "leg0las"
                };
            bool nextPage;
            var posts = Parser.ParsePostsInTheme(themeId, 0, out nextPage).ToArray();
            Assert.IsTrue(nextPage);
            Assert.AreEqual(50, posts.Length);
            foreach (var msgMustContain in msgMustContains)
            {
                Assert.That(posts.Select(x => x.HtmlText).ToArray(), Is.Not.All.Not.Contains(msgMustContain));
            }
            foreach (var expected in authorMustContains)
            {
                Assert.That(posts.Select(x => x.Nick).ToArray(), Is.Not.All.Not.Contains(expected));
            }
            Assert.That(posts, Is.All.Not.StringContaining("<div>"));
            Assert.That(posts.Select(x => x.ThemeId).ToArray(), Is.All.EqualTo(themeId));
            Assert.That(posts.Select(x => x.Theme).ToArray(), Is.All.EqualTo("Sailfish обзавелась полноценной поддержкой приложений для Android - Talks - Форум"));
        }

        [Test]
        [Ignore("RealLor")]
        public void TestReal()
        {
            var post = RealParser.FillPost(new PostBrief() { PostId = 9587173, ThemeId = 9587170 });
            Assert.That(post.HtmlText, Is.StringContaining("Лучше под J2ME"));
            Assert.That(post.HtmlText, Is.Not.StringContaining("<div"));
        }

    }




}
