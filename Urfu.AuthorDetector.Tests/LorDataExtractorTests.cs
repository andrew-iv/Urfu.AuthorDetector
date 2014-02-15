using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Tests
{
    [TestFixture]
    public class StringTests
    {
        [TestCase("First sentence. Second sentence! Third sentence? Yes.", new[] { "First sentence.","Second sentence!","Third sentence?","Yes." })]
        public void TestSentence(string text,string[] sentences)
        {
            CollectionAssert.AreEquivalent(sentences, text.Sentenses());
        }
    }


    [TestFixture]
    public class LorDataExtractorTests
    {
        private FromHtmlDataExtractor _extractor;

        [SetUp]
        public void SetUp()
        {
            _extractor = new FromHtmlDataExtractor();
        }


        public IEnumerable<TestCaseData> TestTextData
        {
            get
            {
                int testId = 0;

                yield return new TestCaseData(new Post
                    {
                        Text = @"<p>Для сервера только Intel. И только готовые брендовые платформы. Так уж повелось.
А дома юзаю что-попало без особых претензий:
<div class=""code""><pre class=""no-highlight""><code>processor	: 0
vendor_id	: AuthenticAMD
cpu family	: 6
model		: 8
model name	: AMD Athlon(tm) XP 2200+
stepping	: 1
cpu MHz		: 1794.638
cache size	: 256 KB
fdiv_bug	: no
hlt_bug		: no
f00f_bug	: no
coma_bug	: no
fpu		: yes
fpu_exception	: yes
cpuid level	: 1
wp		: yes
flags		: fpu vme de pse tsc msr pae mce cx8 apic sep mtrr pge mca cmov pat pse36 mmx fxsr sse syscall mmxext 3dnowext 3dnow up
bogomips	: 3589.27
clflush size	: 32
cache_alignment	: 32
address sizes	: 34 bits physical, 32 bits virtual
power management: ts

</code></pre></div>
</p>"
                    }, new[] { "Для сервера только Intel.", "Так уж повелось" }, new[] { "stepping", "code" }).SetName((++testId).ToString());

                yield return new TestCaseData(new Post
                    {
                        Text = @"<p>кому интересно замерил скорость работы  SwapBuffers</p><p>при SwapbuffersWait on
<div class=""code""><pre class=""no-highlight""><code>/home/download/git/git/demos/src/perf/swapbuffers
Reshape 320x240
   Swapbuffers      320x240: 59.9 swaps/second 4.6 million pixels/second
   Swap/Clear       320x240: 59.9 swaps/second 4.6 million pixels/second
   Swap/Clear/Draw  320x240: 59.9 swaps/second 4.6 million pixels/second
Reshape 640x480
   Swapbuffers      640x480: 59.9 swaps/second 18.4 million pixels/second
   Swap/Clear       640x480: 59.9 swaps/second 18.4 million pixels/second
   Swap/Clear/Draw  640x480: 59.9 swaps/second 18.4 million pixels/second
Reshape 1024x768
   Swapbuffers      1024x768: 59.9 swaps/second 47.1 million pixels/second
   Swap/Clear       1024x768: 59.9 swaps/second 47.1 million pixels/second
   Swap/Clear/Draw  1024x768: 59.9 swaps/second 47.1 million pixels/second
</code></pre></div>
при SwapbuffersWait off</p>"
                    }, new[] { "при SwapbuffersWait off", "кому интересно замерил скорость работы" }, new[] { "47.1 million pixels/second" }).SetName((++testId).ToString());

                yield return new TestCaseData(new Post
                    {
                        Text = @"<p>&gt;<i>Сталина и Гитлера нельзя уравнивать - слишком разные цели у них были.</i></p><p>Дано:
</p><ul><li>Гитлер: немцы помнившие времена мощной империи и терпящие унизительные условия поражения предидущей войны.</li><li>Сталин: громадные территории бардака, быстро плодящееся безграмотное население, верящее в &#171;царя&#187;.
</li></ul><p>Цель: Победить.</p>"
                    }, new[] { "Дано", "немцы помнившие времена мощной империи и терпящие унизительные условия", "громадные территории бардака, быстро плодящееся безграмотное население", ">  Дано: Гитлер", "войны.Сталин:" }, new[] { "слишком разные цели у них были" }).SetName((++testId).ToString());

                yield return  new TestCaseData(new Post
                    {
                        Text = @"<div style=""margin-left: 20px; margin-top:5px; "">
<div class=""smallfont"">Цитата:</div>



</div>
<div style=""margin-right: 20px; margin-left: 20px; padding: 10px; background: #44474f; border: 1px solid #e3e3e5; border-left: 1px solid #808084; border-top: 1px solid #808084; margin-bottom: 10px;"">

Сообщение от <strong>XHTTP</strong> &nbsp;
<div style=""font-style:italic"">правила форума не я придумал</div>
<else></else>

</div><br>
Разьясни мне свой ответй 1вый в топике<br>Тестирование"
                    }, new[] { "Разьясни мне свой ответй 1вый в топике" + Environment.NewLine + "Тестирование" }, new string[] { "Сообщение от" }).SetName("4");

            }
        }

        [TestCaseSource("TestTextData")]
        public void TestText(Post post, string[] mustContains, string[] mustNotContains)
        {
            var txt = _extractor.GetText(post);
            foreach (var contain in mustContains)
            {
                Assert.That(txt,Is.StringContaining(contain));
            }
            foreach (var contain in mustNotContains)
            {
                Assert.That(txt, Is.Not.StringContaining(contain));
            }
        }
    }
}