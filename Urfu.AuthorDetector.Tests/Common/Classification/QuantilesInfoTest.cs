using System.Linq;
using NUnit.Framework;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.StatMethods;

namespace Urfu.AuthorDetector.Tests.Common.Classification
{
    [TestFixture]
    public class PCATests
    {
        [Test]
        public void TestLinearComponents()
        {
            var matr = new[]
                {
                    new[] {1d, 1d, 4d},
                    new[] {2d, 2d, 7d},
                    new[] {3d, 3d, 10d},
                    new[] {4d, 4d, 13d},
                    new[] {1d, 11d, 3d},
                    new[] {2d, 12d, 6d},
                    new[] {3d, 13d, 9d},
                    new[] {4d, 14d, 12d}
                };

            var trans = new PcaMetricTransformer(matr.Take(4));
            Assert.AreEqual(3, trans.PCA.Components.Count);
            //Assert.AreEqual(0, trans.PCA.IndependentVariables);
        }
    }


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