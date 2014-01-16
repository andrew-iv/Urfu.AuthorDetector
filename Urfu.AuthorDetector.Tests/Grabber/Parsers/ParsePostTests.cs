using System;
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
        private AchatBulletinParser _parser;
        

        protected override void OnSetup()
        {
            base.OnSetup();
            _parser = new AchatBulletinParser(new VBulletinPageLoader("http://forum.antichat.ru",Encoding.GetEncoding(1251)));
        }

        [Test]
        public void ForumDisplay()
        {
            for (int i = 1; i < 3; i++)
            {
                int pageCount;
                var res = _parser.ForumDisplay(46, 1, out pageCount).ToArray();
                Assert.Greater(pageCount, 100);
                Assert.IsEmpty(res.Where(x => string.IsNullOrWhiteSpace(x.Title)));
                Assert.IsEmpty(res.Where(x => string.IsNullOrWhiteSpace(x.Owner)));
                Assert.IsEmpty(res.Where(x => x.Id == 0));
                Assert.IsNotEmpty(res.Where(x => x.Title.Contains("Весёлые картинки")));
            }
            


        }
        
        [Test]
        [Ignore("Dohera")]
        public void TestLoadForum()
        {
            var moq = new Moq.Mock<IVBulletinLog>();
            var grabber = new VBulletinGrabber(_parser, moq.Object);
            var res = grabber.LoadForum(46, maxPage: 2).ToArray();
            Assert.IsEmpty(res.Where(x => string.IsNullOrWhiteSpace(x.Nick)));
            Assert.IsEmpty(res.Where(x => string.IsNullOrWhiteSpace(x.Theme)));
            Assert.IsEmpty(res.Where(x => string.IsNullOrWhiteSpace(x.HtmlText)).Select(x=>x.HtmlText));
            Assert.IsEmpty(res.Where(x => x.PostId == 0));
            Assert.IsEmpty(res.Where(x => x.ThemeId == 0));
            Assert.IsEmpty(res.Where(x => x.Time == default(DateTime)));
            CollectionAssert.AllItemsAreUnique(res.Select(x=>x.PostId));
        }

        [Test]
        public void ShowThread()
        {
            int pageCount;
            var res = _parser.Showthread(new ThemeInfo() { Id = 65993 }, 1, out pageCount).ToArray();
            Assert.Greater(pageCount, 20);
            Assert.IsNotEmpty(res.Where(x => x.HtmlText.Contains("Alekzzzander")));
            Assert.That(res.Select(x=>x.Time),Is.All.GreaterThan(new DateTime(1990,1,1)));
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