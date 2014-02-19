using System;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.MetricProvider;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// Параметры SVM
    /// </summary>
    public class MSvmClassifierParams
    {
        public enum LearningAlgorithm
        {
            SMO,
            LS_SVM
        }

        

        public LearningAlgorithm Algorithm { get; set; }

        public MSvmClassifierParams()
        {
           Algorithm = LearningAlgorithm.SMO;
           Probalistic = false;
           Kernel = new Polynomial(1);
           Transform = doubles => GroupMetricFunctions.GetAverages(doubles, 5);
        }

        public ICommonMetricProvider CommonProvider { get; set; }

        public bool Probalistic
        { get; set; }

        public IKernel Kernel
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<double[][], double[][]> Transform { get; set; }
    }
}