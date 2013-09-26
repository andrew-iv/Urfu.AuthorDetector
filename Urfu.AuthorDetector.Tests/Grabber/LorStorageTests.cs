using System;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.DataLayer;
using System.Linq;
using Ninject;

namespace Urfu.AuthorDetector.Tests.Grabber
{
    public class LorStorageTests : TestsBase
    {

        [TearDown]
        public void TestAddOnce()
        {

        }

        protected override void OnSetup()
        {
            base.OnSetup();
            var forum = ContextBuilder.ForumSet.FirstOrDefault(x => x.Id == LorStorage.LorId);
            ContextBuilder.Authors.Add(new[] { "author1", "author2" }.Select(x => new Author()
                {
                    Identity = x,
                    DisplayName = x,
                    Forum = forum,
                }));
            int idCounter = 0;
            ContextBuilder.Themes = new InMemoryDbSet<Theme>(
                Enumerable.Range(1, 2).Select(x => new Theme()
                    {
                        Forum = forum,
                        Id = x,
                        IdOnForum = x,
                        Title = "ThemePost" + x
                    })
                );

            ContextBuilder.Posts = new InMemoryDbSet<Post>(
            ContextBuilder.Authors.AsEnumerable().SelectMany(author =>
                                              Enumerable.Range(1, 3).Select(i =>
                                                  {
                                                      idCounter++;
                                                      return new Post()
                                                          {
                                                              Theme = ContextBuilder.Themes.FirstOrDefault(x=>x.IdOnForum== idCounter % 2+1),
                                                              Author = author,
                                                              Id = idCounter,
                                                              IdOnForum = idCounter,
                                                              Text = "text_" + author.Identity + "_" + i
                                                          };
                                                  })));

        }

        private void AddPostsGeneralAssertions()
        {
            Assert.That(ContextBuilder.Themes.Select(x => x.Title).ToArray(), Is.All.StartsWith("Theme"));
            Assert.AreEqual(1, ContextBuilder.Themes.Select(x => x.Forum).Distinct().Count());

            Assert.That(ContextBuilder.Authors.Select(x => x.DisplayName).ToArray(), Is.All.StartsWith("author"));
            Assert.That(ContextBuilder.Authors.Select(x => x.Identity).ToArray(), Is.All.Unique);
            Assert.That(ContextBuilder.Authors.Select(x => x.Forum).ToArray(), Is.All.Not.Null);
            Assert.AreEqual(1, ContextBuilder.Authors.Select(x => x.Forum).Distinct().Count());


            Assert.That(ContextBuilder.Posts.Select(x => x.Text).ToArray(), Is.All.StartsWith("text_author"));
            Assert.That(ContextBuilder.Posts.Select(x => x.DateTime).Where(x => x != null).ToArray(), Is.All.GreaterThan(new DateTime(2012, 1, 1)));
            Assert.That(ContextBuilder.Posts.Select(x => x.Author).ToArray(), Is.All.Not.Null);
            Assert.That(ContextBuilder.Posts.Select(x => x.Theme).ToArray(), Is.All.Not.Null);
        }

        [Test]
        public void AddPosts1()
        {
            var storage = Ninject.Get<ILorStorage>();
            storage.SavePosts(Enumerable.Range(4, 5).Select(i => new LorPostInfo(new LorPostBrief()
                {
                    Nick = "author2",
                    PostId = i,
                    ThemeId = (i % 2) + 1 + (i / 6),
                    Theme = "Theme" + ((i % 2) + 1 + (i / 6)),
                    Time = new DateTime(2012, 2, 1, i + 1, 0, 0),
                })
                {
                    HtmlText = "text_author2_" + i
                }));
            Assert.AreEqual(3, ContextBuilder.Themes.Count());
            

            Assert.AreEqual(2, ContextBuilder.Authors.Count());
            

            Assert.AreEqual(8, ContextBuilder.Posts.Count());
            AddPostsGeneralAssertions();
        }

        [Test]
        public void AddPosts2()
        {
            var storage = Ninject.Get<ILorStorage>();
            storage.SavePosts(Enumerable.Range(4, 5).Select(i => new LorPostInfo(new LorPostBrief()
            {
                Nick = "author3",
                PostId = i,
                ThemeId = (i % 2) + 1 + (i / 6),
                Theme = "Theme" + ((i % 2) + 1 + (i / 6)),
                Time = new DateTime(2012, 2, 1, i + 1, 0, 0),
            })
            {
                HtmlText = "text_author2_" + i
            }));
            Assert.AreEqual(3, ContextBuilder.Themes.Count());
            Assert.AreEqual(3, ContextBuilder.Authors.Count());
            Assert.AreEqual(8, ContextBuilder.Posts.Count());
            AddPostsGeneralAssertions();
        }

    }
}