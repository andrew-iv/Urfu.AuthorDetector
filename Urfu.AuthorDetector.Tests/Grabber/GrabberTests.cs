using NUnit.Framework;
using Urfu.AuthorDetector.Grabber;
using System.Linq;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Tests.Grabber
{
    public class GrabberTests:TestsBase
    {
        private LorGrabber _grabber { get; set; }

        protected override void OnSetup()
        {
            base.OnSetup();
            _grabber = new LorGrabber(new LorPostsParser(new LorPageLoader()));
        }

        [Ignore("Real Tests")]
        [TestCase(2013, 3, new[] { "quest", "ist76", "Mordrag", "SergMarkov", "morpheus" })]
        [TestCase(2003, 3, new[] { "GogaN", "deadhead"})]
        public void LoadArchive(int year, int month, string[] authorMust)
        {
            var posts = _grabber.LoadAllArchive(year, month).ToArray();
            Assert.That(posts.Select(x=>x.HtmlText).ToArray(),Is.All.Not.Null.Or.Empty);
            Assert.That(posts.Select(x => x.Nick).ToArray(), Is.All.Not.Null.Or.Empty);
            Assert.That(posts.Select(x => x.Theme).ToArray(), Is.All.Not.Null.Or.Empty);
            Assert.That(posts.Select(x => x.PostId).ToArray(), Is.All.Not.EqualTo(0));
            Assert.That(posts.Select(x => x.ThemeId).ToArray(), Is.All.Not.EqualTo(0));
            Assert.That(posts.Select(x => x.Time).ToArray(), Is.All.Not.Null);
            CollectionAssert.AllItemsAreUnique(posts.Select(x => x.PostId).ToArray());
        }
    }
}