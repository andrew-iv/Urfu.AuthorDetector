using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Kernels;
using Ninject;
using Ninject.Modules;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.DataLayer;
using IKernel = Accord.Statistics.Kernels.IKernel;

namespace Bruter
{
    public class ExperimentModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDataSource>().ToMethod(x => new DataSource()
            {
                ForumId = 3,
                DateStart = new DateTime(2012, 1, 1)
            }).InTransientScope();

            Kernel.Bind<int>().ToConstant(1).Named("ClsVersionId");
            Kernel.Bind<IExperimentLogger>().To<ExperimentLogger>().InSingletonScope();
            Kernel.Bind<IClassifierBenchmark>().ToMethod(x =>
                {
                    var ds = StaticVars.Kernel.Get<IDataSource>().GetPosts(30);
                    return new ForumClassifierBenchmark(ds)
                        {
                            AuthorsCount = 10,
                            LearningCount = 300,
                            RoundCount = 5,
                            TestsInRoundCount = 1000

                        };
                }
                ).InSingletonScope();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StaticVars.Kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary = false }, new LorModule(), new ExperimentModule());
            StaticVars.InitializeTops(StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x => x.Value));
            var topPosts = StaticVars.Kernel.Get<IDataSource>().GetPosts(30).SelectMany(x => x.Value).ToArray();
            StaticVars.InitializeTops(topPosts);
            var provider = new UseNgramsMetricProvider();
            var allMetrics = provider.GetMetrics(topPosts);
            var bruter = new MSvmClassifierBruter();
            bruter.K = Enumerable.Range(1, 4).Select(i => i*5).ToArray();
            bruter.Providers = new[] {provider};
            bruter.Kernels =
                Enumerable.Range(1, 5).SelectMany(x => new[]
                    {
                        new KeyValuePair<IKernel, string>(new Polynomial(x), string.Format("Polynominal_{0}_{1}", x, 1))
                        ,
                        new KeyValuePair<IKernel, string>(new Polynomial(x, 2),
                                                          string.Format("Polynominal_{0}_{1}", x, 2)),
                        new KeyValuePair<IKernel, string>(new Polynomial(x, 0.5),
                                                          string.Format("Polynominal_{0}_{1}", x, 0.5))
                    }).Union(new[]
                        {
                            new KeyValuePair<IKernel, string>(new Gaussian(), "Gaussian"),
                            new KeyValuePair<IKernel, string>(new Laplacian(), "Laplacian"),


                        }).ToArray();
            bruter.N = 20;
            bruter.Algorithms = new MSvmClassifierParams.LearningAlgorithm[]{MSvmClassifierParams.LearningAlgorithm.LS_SVM,MSvmClassifierParams.LearningAlgorithm.SMO};
            bruter.Brute();
            

        }
    }
}
