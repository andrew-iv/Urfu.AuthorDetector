using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorRecognizer.MetricSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        protected override void OnStartup(StartupEventArgs e)
        {
            StaticVars.Kernel = new StandardKernel(new CommonModule() { NeedCreateDictionary = true } , new LorModule(), new ExperimentModule());
            /*StaticVars.Kernel.Unbind<ICommonMetricProvider>();
            StaticVars.Kernel.Bind <ICommonMetricProvider>();*/
            base.OnStartup(e);
            
        }

    }
}

