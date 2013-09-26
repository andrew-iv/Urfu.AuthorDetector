using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using NUnit.Framework;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.DataLayer;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Tests.Grabber;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Tests
{

    [TestFixture]
    public class HtmlExtractorTests
    {
        

        [Test]
        public void Test1()
        {
            List<HtmlNode> nodes;
            var paragrpaphs = HtmlExtractor.ExtractParagrahps(@"<p>Потому что отсутствует культура занятий спортом<i><br>
</i><i><br>
&gt;Итак, ну хоть здесь то есть спортсмены?</i></p>".ToHtmlDocument(), out nodes);
            Assert.AreEqual(1, paragrpaphs.Count);
            Assert.AreEqual(2, nodes.Count);
            Assert.That(paragrpaphs[0],Is.Not.StringContaining("br"));
            Assert.That(paragrpaphs[0], Is.Not.StringContaining("спортсмены"));
        }
    }

    public class PostMetricsTests:TestsBase
    {
        [Test]
        public void TestConstructor1()
        {
            var mustLength = 203d;
            var postMetrics = new TrivialMetric(new Post()
                {
                    DateTime = Convert.ToDateTime("2012-03-31T05:30:00+04:00"),
                    Text = @"<p>Потому что отсутствует культура занятий спортом<i><br>
</i><i><br>
&gt;Итак, ну хоть здесь то есть спортсмены?</i><br>
<br>
литрболл и спортивный троллинг считаются? Также поиск пруфлинков на время, спортивное ориентирование в выдаче гугла и скоростное конфигурирование ядра</p>"
                }
                );
            Assert.AreEqual(mustLength, postMetrics.Length);
            Assert.AreEqual(0, new DoubleComparer().Compare(postMetrics.ParagraphsShare, 1.0d / mustLength));
            Assert.AreEqual(0, new DoubleComparer().Compare(postMetrics.PunctuationShare, 2.0d / mustLength));
            Assert.AreEqual(   30d * 60,postMetrics.Time ,0.001);
            Assert.AreEqual(29d / mustLength, postMetrics.WhitespacesShare, 0.001);
        }
    }


    [TestFixture]
    public abstract class TestsBase
    {
        [SetUp]
        public void SetUp()
        {
            ContextBuilder = new DbContextBuilder
                {
                    ForumSet = new InMemoryDbSet<Forum>(
                        new[]
                            {
                                new Forum()
                                    {
                                        Id = LorStorage.LorId,
                                        ForumUrl = LorGrabber.LorUrl,
                                        Description = "LOR",
                                        
                                    }
                            })
                };

            FakeModule = new FakeModule(ContextBuilder);
            OnSetup();            
        }

        protected DbContextBuilder ContextBuilder;
        protected virtual FakeModule FakeModule { get; set; }
        protected virtual IKernel Ninject
        {
            get { return new StandardKernel(FakeModule); }
        }

        protected virtual void OnSetup()
        {

        }
    }
}