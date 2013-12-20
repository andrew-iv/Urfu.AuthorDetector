using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Grabber.Flamp;

namespace Urfu.AuthorDetector.Tests.Grabber.Flamp
{
    [Ignore("Network")]
    public class FlampLoaderTest : TestsBase
    {
        private IFlampLoader _loader;
        public readonly Dictionary<string, string> Cities = new Dictionary<string, string> { { "novosibirsk", "Новосибирск" }, { "ekaterinburg", "Екатеринбург" } };

        protected override void OnSetup()
        {
            base.OnSetup();
            _loader = new FlampLoader();
        }


        [TestCaseSource("Cities")]
        public void TestLoadMainPage(KeyValuePair<string, string> kvp)
        {

            foreach (var i in Enumerable.Range(1, 3))
            {
                var doc = _loader.LoadUsers(kvp.Key, i).DocumentNode.InnerHtml;
                foreach (var word in new[] { kvp.Value, "перты", "10", "Эксперты — самые важные люди на Флампе. Мы любим их, дарим подарки и устраиваем вечеринки.", "<li>" + i + "</li>" })
                {
                    StringAssert.Contains(word, doc, kvp.ToString() + " -  i=" + i);
                }
            }

        }

        [TestCase("nika_nsk")]
        [TestCase("yana_key")]
        public void TestLoadUserPage(string user)
        {

            var doc = _loader.LoadUser(user).DocumentNode.InnerHtml;
            foreach (var word in new[] { user, "читает" })
            {
                StringAssert.Contains(word, doc);
            }
        }

    }

}
