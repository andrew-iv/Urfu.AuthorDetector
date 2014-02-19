using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{

    

    public interface IClassifierFactory
    {
        /// <summary>
        /// Параметризация сообщений
        /// </summary>
        ICommonMetricProvider[] CommonMetricProviders { get; set; }

        /// <summary>
        /// метод создания
        /// </summary>
        /// <param name="authors"></param>
        /// <returns></returns>
        IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }


    public interface IExperimentLogger
    {
        void LogBenchmark(IClassifierFactory factory, IEnumerable<KeyValuePair<string, string>> parameters);
    }
}
