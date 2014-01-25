using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Statistics.Analysis;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.Experiment;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.Common.StatMethods;

namespace Urfu.AuthorRecognizer.MetricSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PcaMetricTransformer pca;
            var factory = new StupidPerecentileBayesClassifierFactory()
                {
                    PostMetricProvider =
                    StaticVars.Kernel.Get<IPostMetricProvider>()
                    /* PcaMetricTransformer.CreateSimpeMetricProvider(
                    (float)double.Parse(tbTreeshold.Text), out pca,AnalysisMethod.Standardize)*/
                };
            //this.TextBlockRes.Text += string.Format("PCA-Comps ={0}", pca.Size) + Environment.NewLine;
            TextBlockRes.UpdateLayout();
            UpdateLayout();
            
            var ds = StaticVars.Kernel.Get<IDataSource>();
            var strategy = new IndexSelectionStrategy()
                {
                    Benchmark = new ForumClassifierBenchmark(ds.GetPosts(20))
                        {
                            TestsInRoundCount = 400,
                            RoundCount = 5,
                            MessageCount = int.Parse(tbMsgCount.Text)
                        },
                    TimeOut = TimeSpan.FromMinutes(int.Parse(tbMinutes.Text))
                };
            strategy.ArgAdded += (o, changed) =>
            {
                TextBlockLog.Text += changed.Index + Environment.NewLine;
                TextBlockLog.UpdateLayout();
                UpdateLayout();
            };
            var ind = strategy.SelectMetric(new MultyMetricProviderStrategyProxy()
                {
                    Factory = new StupidPerecentileBayesClassifierFactory()
                        {
                            PostMetricProvider = new SelectedPostMetricProvider(
                                StaticVars.Kernel.Get<IPostMetricProvider>()) { Indexes = new int[] { 3, 4, 6, 14, 17, 25, 29, 31, 32, 36, 38, 41, 42, 43, 44, 45, 46, 48, 50, 51, 53, 55, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 68, 69, 70, 71, 72, 73 }},
                            MultiplyMetricsProvider = PcaMetricTransformer.CreateMultiplyMetricProvider(0.995f,
                            out pca,AnalysisMethod.Standardize)
                        }
                });



            foreach (var i in ind)
            {
                this.TextBlockRes.Text += i + " = " + factory.PostMetricProvider.Names.ToArray()[i] + Environment.NewLine;
            }
        }
    }
}
