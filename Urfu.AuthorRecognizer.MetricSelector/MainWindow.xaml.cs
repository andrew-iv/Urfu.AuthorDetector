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
                    CommonMetricProviders = new[] { SomeSelectedMetricProviders.Chi2Test4 }
                    /* PcaMetricTransformer.CreateSimpeMetricProvider(
                    (float)double.Parse(tbTreeshold.Text), out pca,AnalysisMethod.Standardize)*/
                };
            //this.TextBlockRes.Text += string.Format("PCA-Comps ={0}", pca.Size) + Environment.NewLine;
            TextBlockRes.UpdateLayout();
            UpdateLayout();
            
            var ds = StaticVars.Kernel.Get<IDataSource>();
            var strategy = new AddDelSelectionStrategy()
                {
                    Benchmark = new MultyBenchmark(ds.GetPosts(20))
                        {
                            TestsInRoundCount = 400,
                            RoundCount = 4,
                            MessageCount = int.Parse(tbMsgCount.Text)
                        },
                    TimeOut = TimeSpan.FromMinutes(int.Parse(tbMinutes.Text)),
                    DeltaAdd = 30,
                    DeltaDel = 14
                };
            strategy.ArgAdded += (o, changed) =>
            {
                TextBlockLog.Text += changed.Index + Environment.NewLine;
                TextBlockLog.UpdateLayout();
                UpdateLayout();
            };
            var ind = strategy.SelectMetric(new CommonProviderStrategyProxy()
                {
                    Factory = new StupidPerecentileBayesClassifierFactory()
                        {
                            CommonMetricProviders = new []{SomeSelectedMetricProviders.Chi2Test4}
                        }
                },new int[]{3,5,7,9,10,11,12,15,18,21,22,23,24,35,37,40,41,42});



            foreach (var i in ind)
            {
                this.TextBlockRes.Text += i + " = " + factory.CommonMetricProviders[0].Names.ToArray()[i] + Environment.NewLine;
            }
        }
    }
}
