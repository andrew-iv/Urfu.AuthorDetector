using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using NUnit.Framework;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.StatMethods;

namespace Urfu.AuthorDetector.Tests.Common.Classification
{
    [TestFixture]
    public class KVMStatTests
    {
        [TestCaseSource("DataSource")]
        public void Test1(Dictionary<int, double[][]> data)
        {
            var stud = data.SelectMany(x => x.Value.Select(val => new KeyValuePair<int, double[]>(x.Key, val))).ToArray();
            var stat = new MultyKSvmStatistics(4,data.Count,
                Gaussian.Estimate(stud.Select(x => x.Value).ToArray())
                //new Polynomial(3)
                );
            double[] vecs;
            stat.Study(stud);
            var res = stat.Classify(new double[]{25,26,39,46},out vecs, MulticlassComputeMethod.Elimination);
            Assert.AreEqual(1,res);

            res = stat.Classify(new double[] { 25, 36, 29, 16 }, out vecs, MulticlassComputeMethod.Elimination);
            Assert.AreEqual(2, res);
            
        }

        [TestCaseSource("DataSource")]
        public void Test2(Dictionary<int, double[][]> data)
        {
            var stud = data.SelectMany(x => x.Value.Select(val => new KeyValuePair<int, double[]>(x.Key, val))).ToArray();
            var ker =
                Gaussian.Estimate(stud.Select(x => x.Value).ToArray());
            var svm = new KernelSupportVectorMachine(ker, 4);
            var err = new SequentialMinimalOptimization(svm, stud.Select(x => x.Value).ToArray(),
                                              stud.Select(x => x.Key==0?1:-1).ToArray()
                ).Run();
            err = new ProbabilisticOutputLearning(svm, stud.Select(x => x.Value).ToArray(),
                                              stud.Select(x => x.Key == 0 ? 1 : -1).ToArray()
                ).Run();


            double vecs;
            var res = svm.Compute(new double[] {25, 26, 39, 46}, out vecs);
            Assert.AreEqual(-1, res);


        }

        public static IEnumerable<TestCaseData>  DataSource
        {
            get
            {
                yield return new TestCaseData(Data1);
            }
        }

        private static Dictionary<int, double[][]> Data1
        {
            get
            {
                return new Dictionary<int, double[][]>
                    {
                        {
                            1,
                            Enumerable.Range(0, 20).Select(i =>
                                                           new double[]
                                                               {
                                                                   i*2 + 10, i*3 + 10, i*4 + 10, i*5 + 10
                                                               }
                                ).ToArray()
                        },
                        {
                            2,
                            Enumerable.Range(0, 20).Select(i =>
                                                           new double[]
                                                               {
                                                                   i*2, i*3, i*2, i*5 - 30
                                                               }
                                ).ToArray()
                        },
                        {
                            0,
                            Enumerable.Range(0, 30).Select(i =>
                                                           new double[]
                                                               {
                                                                   i*2 - 2400, i*2, i*3 - 1000, i*5 - 1000
                                                               }
                                ).ToArray()
                        },
                        {
                            3,
                            Enumerable.Range(0, 30).Select(i =>
                                                           new double[]
                                                               {
                                                                   i*2 + 2400, i*2, i*3 + 1000, i*5 + 1000
                                                               }
                                ).ToArray()
                        },
                    };
            }
        }
    }


    [TestFixture]
    public class PCATests
    {
        [Test]
        public void TestLinearComponents()
        {
            var matr = new[]
                {
                    new[] {1d,0d, 1d, 4d},
                    new[] {2d,0d, 2d, 7d},
                    new[] {3d,0d, 3d, 10d},
                    new[] {4d,0d, 4d, 13d},
                    new[] {1d,0d, 11d, 3d},
                    new[] {2d,0d, 12d, 6d},
                    new[] {3d,0d, 13d, 9d},
                    new[] {4d,0d, 14d, 12d}
                };

            var trans = new PcaMetricTransformer(matr.Take(4));
            trans.SetComponents(0.99f);
            
            Assert.AreEqual(1, trans.Components.Count());
            var metr = trans.GetMetric(new[] {3d, 0d, 3d, 10d});
            Assert.AreEqual(1,metr.Length);
            Assert.Greater(metr[0],0);

            metr = trans.GetMetric(new[] { 1d, 0d, 1d, 4d });
            Assert.AreEqual(1, metr.Length);
            Assert.Less(metr[0], 0);


            var metr2 = trans.GetMetric(new[] { 1d, 10d, 1d, 4d });
            Assert.AreEqual(1, metr2.Length);
            Assert.AreEqual(metr[0], metr2[0]);


            //Assert.AreEqual(0, trans.PCA.IndependentVariables);
        }
    }
}