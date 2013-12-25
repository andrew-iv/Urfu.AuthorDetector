using System.Linq;
using System.Text;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Grabber.VBullitin;

namespace Urfu.AuthorDetector.Tests.Grabber.Parsers
{
    [TestFixture]
    public class VBParserTests : TestsBase
    {
        private VBulletinParser _parser;
        

        protected override void OnSetup()
        {
            base.OnSetup();
            _parser = new VBulletinParser(new VBulletinPageLoader("http://forum.antichat.ru",Encoding.GetEncoding(1251)));
        }


        [Test]
        public void Members()
        {
            const int n = 100;

            var userInfoes = _parser.Users(n).Concat(_parser.Users(n,2)) .ToArray();
            var users = userInfoes.Select(x => x.Nick).ToArray();
            Assert.AreEqual(n*2,users.Length);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreUnique(userInfoes.Select(x=>x.IdOnForum).ToArray());

            Assert.Contains("Rebz", users);
            Assert.Contains("Isis", users);
            Assert.Contains("GreenBear", users);
            Assert.Contains("Kusto", users);
        }

        [Test]
        public void Posts()
        {
            int searchId;
            int total;
            var posts = _parser.Posts("Rebz", new[] {6, 35}, out searchId, out total).ToArray();
            Assert.AreEqual(25,posts.Length);
            Assert.AreEqual(500,total);
            CollectionAssert.AllItemsAreUnique(posts.Select(x=>x.PostId).ToArray());
            Assert.That(posts.Select(x=>x.ThemeId),Is.All.GreaterThan(0));
            CollectionAssert.AllItemsAreUnique(posts.Select(x => x.Time).ToArray());

            var morePosts = _parser.MorePosts(searchId, 1, 100).Concat(_parser.MorePosts(searchId, 2, 100)).ToArray();
            Assert.AreEqual(200, morePosts.Length);
            CollectionAssert.AllItemsAreUnique(morePosts.Select(x => x.PostId).ToArray());
            Assert.That(morePosts.Select(x => x.ThemeId), Is.All.GreaterThan(0));
            Assert.Greater(morePosts.Select(x => x.Time).Distinct().Count(),1);
        }

        [Test]
        public void FillPostInfo()
        {
            int searchId;
            int total;
            var post = _parser.Posts("Rebz", new[] { 6, 35 }, out searchId, out total).ToArray().First();
            Assert.NotNull(_parser.FillPostInfo(new PostBrief() { PostId = post.PostId }).HtmlText);
        }
    }
}