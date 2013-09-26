using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Tests.Common.Classification
{
    public class MetricTests
    {
        public class MockMetric1 : BaseMetric, IFillableMetric
        {
            private IDictionary<string, double> _dict;

            public override IEnumerable<KeyValuePair<string, double>> MetricValues
            {
                get { return _dict; }
            }

            public virtual void FillFromPost(Post post)
            {
                _dict = new Dictionary<string, double>() { { "Id", post.Id }, { "IdOnForum", post.IdOnForum } };
            }
        }

        public IEnumerable<TestCaseData> TestClassifier1Source
        {
            get
            {
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
            var author1 = Enumerable.Range(1, 400).Select(i => new Post() { Id = i, IdOnForum = i * 4 + 30 , Text = "<p>"}).ToArray();
            var author2 = Enumerable.Range(1, 490).Select(i => new Post() { Id = i * 4 + 40, IdOnForum = i * 2 + 20, Text = "<p>" }).ToArray();
            var author3 = Enumerable.Range(1, 290).Select(i => new Post() { Id = i, IdOnForum = i - 250, Text = "<p>" }).ToArray();
            var classifier = new MetricNeighboorClassifier<MockMetric1>(new Dictionary<Author, IEnumerable<Post>>()
                {
                    {new Author(){Identity = "author1"},author1},
                    {new Author(){Identity = "author2"},author2},
                    {new Author(){Identity = "author3"},author3}
                });
            return classifier.ClassificatePosts(example).Identity;
        }


    }
}