using NUnit.Framework;
using Opcorpora.Dictionary;
using System.Linq;

namespace Urfu.AuthorDetector.Tests.Opencorpora
{
    public class OpenCorporaTests:TestsBase
    {
        private OpcorporaDictionary _dictionary;

        protected override void OnInit()
        {
            base.OnInit();
            _dictionary = new OpcorporaDictionary(@"C:\Users\andrew-iv\Downloads\dict.opcorpora.xml");
        }

        [TestCase("печь",3)]
        [TestCase("фывпфыпвфывпрфыв",0)]
        public void GetLemma(string lemma, int count)
        {
             Assert.AreEqual(count,_dictionary.GetLemma(lemma).Count());
        }

    }
}