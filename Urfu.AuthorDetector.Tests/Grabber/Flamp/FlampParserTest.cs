using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Grabber.Flamp;
using Urfu.AuthorDetector.Tests.Grabber.Parsers;

namespace Urfu.AuthorDetector.Tests.Grabber.Flamp
{
    public class FlampParserTest : TestsBase
    {
        private IFlampParser _parser;
        public readonly Dictionary<string, string> Cities = new Dictionary<string, string> { { "novosibirsk", "Новосибирск" }, { "ekaterinburg", "Екатеринбург" } };

        protected override void OnSetup()
        {
            base.OnSetup();
            _parser = new FlampParser(new FlampFileLoader());
        }


        [Test]
        public void TestListUsers()
        {

            string[] mustBe = new string[] { "user54082", "alinakakmalina", "marion", "user69095", "hom_mudruj" };
            string[] notMustBe = new string[] { "alinakakmalina444",""," " };

            var res = _parser.Users("novosibirsk", 2).ToArray();
            Assert.AreEqual(40, res.Count());
            Assert.IsEmpty(res.Where(notMustBe.Contains).ToArray());
            Assert.IsEmpty(mustBe.Where(x=>!res.Contains(x)).ToArray());
        }

        [Test]
        public void TestParseUser()
        {
            var res = _parser.Posts("nika_nsk").ToArray();
            CollectionAssert.AllItemsAreUnique(res.Select(x=>x.HtmlText).ToArray());
            CollectionAssert.AllItemsAreUnique(res.Select(x => x.PostId).ToArray());
            CollectionAssert.AllItemsAreUnique(res.Select(x => x.Theme+x.Time).ToArray());
            CollectionAssert.AllItemsAreUnique(res.Select(x => x.Time).ToArray());
            CollectionAssert.AllItemsAreUnique(res.Select(x => x.ThemeId.ToString()+x.Time).ToArray());
        }
        


    }

}
