using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Ninject;
using Urfu.AuthorDetecor.Experiment;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetecor.ExperimentConsole
{
    public class Program
    {
        private static readonly string[] Authors = new string[]{
"xtraeft",
"DNA_Seq",
"Quasar",
"Axon",
"Sadler",
"stevejobs",
"tailgunner",
"vurdalak",
"lazyklimm",
"Mystra_x64"}
;


        private static StandardKernel _kernel;

        private static void Test(Func<IDictionary<Author, IEnumerable<string>>, IClassifier> constructor,
                                 string filePrefix,int topAuthors = 10)
        {
            const int cnt1 = 1;
            const int cnt2 = 1000;
            _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
                {
                    TopAuthors = topAuthors,
                    ForumId = LorStorage.LorId,
                    EndGeneral = new DateTime(2013, 3, 1),
                    PostsCount = 1000,
                    StartGeneral = new DateTime(2012, 1, 1)
                });
            var exp = new Experimentator(_kernel, constructor);
            _kernel.Unbind<IExperiment>();
            
            using (var fileStat = File.Open(
                string.Format("{1}_{0}_Total.csv", DateTime.Now.Ticks, filePrefix)
                , FileMode.Create, FileAccess.Write))
            using (var writerStat = new CsvWriter(new StreamWriter(fileStat)))
            {

                foreach (var postsCount in Enumerable.Range(1, 20).Select(i => i*5).Concat(Enumerable.Range(1, 20).Select(i => i*10+100)))
                {
                    using (var file = File.Open(
                        string.Format("{2}_{1}_{0}.csv", postsCount, DateTime.Now.Ticks, filePrefix)
                        , FileMode.Create, FileAccess.Write))
                    using (var writer = new CsvWriter(new StreamWriter(file)))
                    {

                        int success = 0;


                        
                            _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
                                {
                                    TopAuthors = topAuthors,
                                    EndGeneral = new DateTime(2013, 3, 2),
                                    ForumId = LorStorage.LorId,
                                    PostsCount = postsCount,
                                    StartGeneral = new DateTime(2012, 1, 1)
                                });

                            foreach (var res in Enumerable.Range(1, cnt2).Select(i =>
                                                                                 exp.Test()))
                            {
                                writer.WriteFields(res.Actual.Identity, res.Returned.Identity, res.IsSuccess);
                                if (res.IsSuccess)
                                    success++;
                            }
                            _kernel.Unbind<IExperiment>();

                        writerStat.WriteFields(success*100d/(cnt1*cnt2), postsCount);
                        Console.WriteLine("{0}% - {1}", success*100d/(cnt1*cnt2), postsCount);
                    }
                }
            }
        }

    

        static void Main(string[] args)
        {
            _kernel = new StandardKernel(new CommonModule());
            StaticVars.Kernel = _kernel;
            Test(x => new BayesClassifier(x, new SelectedMetricProvider()), "BayesClassifier");
            Test(x => new NeighboorClassifier(x, new SelectedMetricProvider()), "NeighboorClassifier");

            Test(x => new BayesClassifier(x, new SelectedMetricProvider()), "BayesClassifier",50);
            Test(x => new NeighboorClassifier(x, new SelectedMetricProvider()), "NeighboorClassifier",50);

            //Test(x => new MetricNeighboorClassifier<TrivialMetric>(x), "TrivialMetric");

        }
    }
}
