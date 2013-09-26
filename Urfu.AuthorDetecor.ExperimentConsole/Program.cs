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

        private const int TopAuthors = 50;

        private static StandardKernel _kernel;

        private static void Test(Func<IDictionary<Author, IEnumerable<Post>>, IClassifier> constructor, string filePrefix)
        {
            _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
            {
                TopAuthors = TopAuthors,
                ForumId = LorStorage.LorId,
                EndGeneral = new DateTime(2013, 3, 1),
                PostsCount = 100,
                StartGeneral = new DateTime(2012, 1, 1)
            });
            var exp = new Experimentator(_kernel, constructor);
            _kernel.Unbind<IExperiment>();

            foreach (var postsCount in Enumerable.Range(1,3).Select(i=>i*100))
            {
                using (var file = File.Open(
    string.Format("{2}_{1}_{0}.csv",postsCount, DateTime.Now.Ticks, filePrefix)
    , FileMode.Create, FileAccess.Write))
                using (var writer = new CsvWriter(new StreamWriter(file)))
                {
                    const int cnt1 = 1;
                    const int cnt2 = 100;
                    int success = 0;


                    foreach (var endMonth in Enumerable.Range(3, cnt1).Select(x => new DateTime(2013, x, 1)))
                    {
                        _kernel.Bind<IExperiment>().ToConstant(new Experiment.Experiment()
                        {
                            TopAuthors = TopAuthors,
                            EndGeneral = endMonth,
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
                    }
                    Console.WriteLine("{0}% - {1}", success * 100d / (cnt1 * cnt2), postsCount);
                }
            }
            
        }

        static void Main(string[] args)
        {
            _kernel = new StandardKernel(new CommonModule());
            
            Test(x => new GrammsMetricNeighboorClassifier<TrigrammsTrivialMetric>(x), "TrivialMetricGrammas");
            Test(x => new MetricNeighboorClassifier<TrivialMetric>(x), "TrivialMetric");

        }
    }
}
