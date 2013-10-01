using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Tests.Common.Classification
{
    public class SplitMetricProvider : IMetricProvider
    {
        public SplitMetricProvider(int size)
        {
            Size = size;
        }

        public IEnumerable<string> Names { get { return Enumerable.Range(1, Size).Select(x => "Name_" + x); } }
        public int Size { get; private set; }
        public IEnumerable<double> GetMetrics(string text)
        {
            return text.Split('_').Select(double.Parse).ToArray();
        }
    }


    [TestFixture]
    public class ForAuthorMetricSelectorTests
    {

        public IEnumerable<TestCaseData> Test3AuthorsData
        {
            get
            {
                yield return new TestCaseData(new Func<IEnumerable<IEnumerable<string>>,int,IMetricSelector>
                    ((authors, topMetricsCount) => new Chi2ForAuthorMetricSelector(authors, topMetricsCount)))
                    .SetName("Chi2ForAuthorMetricSelector");

                yield return new TestCaseData(new Func<IEnumerable<IEnumerable<string>>, int, IMetricSelector>
                    ((authors, topMetricsCount) => new IntersectionForAuthorMetricSelector(authors, topMetricsCount)))
                    .SetName("IntersectionForAuthorMetricSelector");
            }
        }


        [TestCaseSource("Test3AuthorsData")]
        public void Test3Authors(Func<IEnumerable<IEnumerable<string>>,int,IMetricSelector> metricSelector)
        {
            var authors = new IEnumerable<string>[]
                {
                    Enumerable.Range(0, 1000).Select(i => string.Format("{0}_{1}_{2}_{3}", i*0.5, i*0.5, i*0.5, 0)),
                    Enumerable.Range(0, 1000).Select(i => string.Format("{0}_{1}_{2}_{3}", i*0.5, i*0.5, i*1.5, i*2.4)),
                    Enumerable.Range(0, 1000).Select(i => string.Format("{0}_{1}_{2}_{3}", i*4.5, i*0.5, i*1.5, i*0)),
                };
            var seector = metricSelector(authors, 1);
            var res = seector.SelectMetrics(new SplitMetricProvider(4));
            CollectionAssert.AreEquivalent(new[]{0,3}, res);

            seector = metricSelector(authors, 2);
            res = seector.SelectMetrics(new SplitMetricProvider(4));
            CollectionAssert.AreEquivalent(new[] { 0, 2, 3 }, res);

            seector = metricSelector(authors, 3);
            res = seector.SelectMetrics(new SplitMetricProvider(4));
            CollectionAssert.AreEquivalent(new[] { 0, 2, 3,1 }, res);

            seector = metricSelector(authors, 4);
            res = seector.SelectMetrics(new SplitMetricProvider(4));
            CollectionAssert.AreEquivalent(new[] { 0, 2, 3, 1 }, res);
        }

    }

    [TestFixture]
    public class HistogramOperationTests
    {



        [Test]
        public void TestChi2DistanceEqual()
        {
            var eps = 0.0000001;
            var data = Enumerable.Range(0, 100).Select(x => x * 0.5d);
            var data2 = Enumerable.Range(0, 200).Select(x => (x / 2) * 0.5d);
            var data3 = Enumerable.Range(0, 200).Select(x => x * 0.5d);
            var data4 = Enumerable.Range(0, 200).Select(x => x * x * 0.5d);


            var histA = new Histogram(data, 100, -eps, 49.5 + eps);
            var with100 = histA.Chi2Distance(new Histogram(data, 100, -eps, 49.5 + eps));

            Assert.AreEqual(0d, with100, 0.005);

            Assert.AreEqual(new Histogram(data, 100, -eps, 49.5 + eps).Chi2Distance(histA), with100, 0.005);
            Assert.AreEqual(0d, histA.Chi2Distance(new Histogram(data, 50, -eps, 49.5 + eps)), 0.01);

            

            Assert.AreEqual(0d, histA.Chi2Distance(new Histogram(data2, 100, -eps, 49.5 + eps)), 0.01);
            Assert.Greater(new Histogram(data, 100, -eps, 100 + eps).Chi2Distance(new Histogram(data3, 200, -eps, 100 + eps)) * 2, 0.5);


            Assert.AreEqual(with100, new Histogram(data.Select(x => x * 4), 100).Chi2Distance(new Histogram(data.Select(x => x * 4), 100)));
            Assert.AreEqual(with100, new Histogram(data4.Select(x => x * 4), 33).Chi2Distance(new Histogram(data4.Select(x => x * 4), 33)), 0.025);


            var cuttedHist = new Histogram(data, 100, -eps, 50 + eps);
        }

        [Test]
        public void ToHistogrammTest()
        {
            var data = Enumerable.Range(0, 100).Select(x => x * 0.5d);
            var eps = 0.0000001;
            var hist = data.ToHistogramm(40, 5, 44.5);
            Assert.That(hist[0].Count,Is.GreaterThan(4));
            Assert.That(hist[hist.BucketCount-1].Count, Is.GreaterThan(4));
        }

        [Test]
        public void TestIntersectionSquareEqual()
        {
            var eps = 0.0000001;
            var data0 = Enumerable.Range(0, 100).Select(x => 0d);
            var data = Enumerable.Range(0, 100).Select(x => x * 0.5d);
            var data2 = Enumerable.Range(0, 200).Select(x => (x / 2) * 0.5d);
            var data3 = Enumerable.Range(0, 200).Select(x => x * 0.5d);
            var data4 = Enumerable.Range(0, 200).Select(x => x * x * 0.5d);

            


            var histA = new Histogram(data, 100, -eps, 50 + eps);

            Assert.AreEqual(0,histA.IntersectionSquare(new Histogram(data0, 100, -eps, 50 + eps)), 0.02);
            var with100 = histA.IntersectionSquare(new Histogram(data, 100,-eps,50+eps));

            Assert.AreEqual(with100, 1d,0.05);
            Assert.AreEqual(with100, histA.IntersectionSquare(new Histogram(data, 50, -eps, 50 + eps)), 0.05);
            Assert.AreEqual(with100, histA.IntersectionSquare(new Histogram(data2, 100, -eps, 50 + eps)), 0.05);
            Assert.AreEqual(with100, histA.IntersectionSquare(new Histogram(data3, 200, - eps, 100 + eps)) * 2, 0.025);


            Assert.AreEqual(with100, new Histogram(data.Select(x => x * 4), 100).IntersectionSquare(new Histogram(data.Select(x => x * 4), 100)));
            Assert.AreEqual(with100, new Histogram(data4.Select(x => x * 4), 33).IntersectionSquare(new Histogram(data4.Select(x => x * 4), 33)),0.025);

            //Assert.AreEqual(0d, histA.IntersectionSquare(data.ToHistogramm(50, -eps, 45 + eps)) - 0.77, 0.025);

        }

    }


    [TestFixture]
    public class AllMetricProviderTests
    {
        private AllMetricProvider _provider = new AllMetricProvider();

        [SetUp]
        public void SetUp()
        {
            StaticVars.InitializeTops(new string[] { "Аааааа Ееееееззз", "Аааааа Ааа Ееееееззз" }, 2);
        }

        [Test]
        public void Test1()
        {
            const string tString = "Аааааа еееееееееееее,";
            Assert.AreEqual(_provider.Size, _provider.Names.Count());
            var metric = _provider.GetMetrics(tString).ToArray();
            Assert.AreEqual(_provider.Size, metric.Length);
            Assert.AreEqual(tString.Length, metric[0], 0.001);
            Assert.AreEqual(1.0 / tString.Length, metric[1], 0.001);
            Assert.AreEqual((tString.Length - 2.0d) / tString.Length, metric[5], 0.001);

            Assert.Greater(metric[7], 0.01);
            Assert.Greater(metric[8], 0.01);
            Assert.Greater(metric[9], 0.01);
            Assert.AreEqual(0,metric[10]);
        }

        [Test]
        public void TestEmpty()
        {
            const string tString = "";
            Assert.AreEqual(_provider.Size, _provider.Names.Count());
            var metric = _provider.GetMetrics(tString).ToArray();
            Assert.AreEqual(_provider.Size, metric.Length);
            Assert.AreEqual(tString.Length, metric[0], 0.001);
            Assert.AreEqual(0d, metric[1], 0.001);
            Assert.AreEqual(0d, metric[5], 0.001);
            Assert.AreEqual(0d, metric[8], 0.001);
        }



    }

    public class MetricTests
    {
        public class MockMetric1 : BaseMetric, IFillableMetric
        {
            private IDictionary<string, double> _dict;

            public override IEnumerable<KeyValuePair<string, double>> MetricValues
            {
                get { return _dict; }
            }

            public virtual void FillFromPost(string post)
            {
                //  _dict = new Dictionary<string, double>() { { "Id", post.Id }, { "IdOnForum", post.IdOnForum } };
            }
        }

        public IEnumerable<TestCaseData> TestClassifier1Source
        {
            get
            {
                yield break;
                var nameCount = 1;
                yield return new TestCaseData(Enumerable.Range(1, 20).Select(i => new Post() { IdOnForum = -200, Text = "<p>", Id = i - 10 }))
                    .Returns("author3").SetName((nameCount++).ToString());

                yield return new TestCaseData(Enumerable.Range(1, 30).Select(i => new Post() { IdOnForum = 6000, Text = "<p>", Id = i - 10 }))
                    .Returns("author1").SetName((nameCount++).ToString());

                yield return new TestCaseData(Enumerable.Range(1, 30).Select(i => new Post() { IdOnForum = -100, Text = "<p>", Id = 135 }))
                    .Returns("author3").SetName((nameCount++).ToString());

                yield return new TestCaseData(Enumerable.Range(1, 30).Select(i => new Post() { IdOnForum = 6, Text = "<p>", Id = 1000000 }))
                    .Returns("author2").SetName((nameCount++).ToString());
            }
        }

        [TestCaseSource("TestClassifier1Source")]
        public string TestClassifier1(IEnumerable<Post> example)
        {
            return null;
            /*var author1 = Enumerable.Range(1, 400).Select(i => new Post() { Id = i, IdOnForum = i * 4 + 30 , Text = "<p>"}).ToArray();
            var author2 = Enumerable.Range(1, 490).Select(i => new Post() { Id = i * 4 + 40, IdOnForum = i * 2 + 20, Text = "<p>" }).ToArray();
            var author3 = Enumerable.Range(1, 290).Select(i => string { Id = i, IdOnForum = i - 250, Text = "<p>" }).ToArray();
            var classifier = new MetricNeighboorClassifier<MockMetric1>(new Dictionary<Author, IEnumerable<string>>()
                {
                    {new Author(){Identity = "author1"},author1},
                    {new Author(){Identity = "author2"},author2},
                    {new Author(){Identity = "author3"},author3}
                });
            return classifier.ClassificatePosts(example).Identity;*/
        }


    }
}