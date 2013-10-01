using System.Collections.Generic;
using NUnit.Framework;
using Urfu.Utils;
using System.Linq;

namespace Urfu.AuthorDetector.Tests.Utils
{
    [TestFixture]
    public class StringTests
    {

        public IEnumerable<TestCaseData>  TestRuWordsData
        {
            get
            {
                yield return new TestCaseData("Привет, всем Trololo",new []{"Привет","всем"});
                yield return new TestCaseData("Trololo12 12 342? .dg  ", new string[] {});
                yield return new TestCaseData("Иван &lt; Trololo12 12 342?  сделает диплом  ", new string[] { "Иван", "сделает", "диплом" });
                yield break;
            }
        }

        [TestCaseSource("TestRuWordsData")]
        public void TestRuWords(string word, string[] must)
        {
            CollectionAssert.AreEqual(must,word.RussianWords().ToArray());
        }
    }
}