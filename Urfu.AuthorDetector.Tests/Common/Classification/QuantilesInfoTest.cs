using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Tests.Common.Classification
{
    [TestFixture]
    public class QuantilesInfoTest
    {
        [Test]
        public void Test1()
        {
            var qInfo = new QuantilesInfo(3, Enumerable.Range(1, 100).Select(x => new[] { x * 0.5d, x * 3.0d, x * x * 1.0d }).ToArray());
            Assert.AreEqual(qInfo.Size, 3);

            Assert.That(qInfo.GetQuantiles(new[] { 1d, 4d, 5d }), Is.All.EqualTo(1));

            CollectionAssert.AreEqual(new int[] { 4, 2, 1 }, qInfo.GetQuantiles(new[] { 40d, 90d, 100d }));

            CollectionAssert.AreEqual(new int[] { -1, 2, 1 }, qInfo.GetQuantiles(new[] { 140d, 90d, 100d }));

        }
    }
}