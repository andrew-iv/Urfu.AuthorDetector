using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.Common.MetricProvider.Sentance;
using Urfu.AuthorDetector.Common.Sentance;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{

    

    public interface IClassifierFactory
    {
        ICommonMetricProvider[] CommonMetricProviders { get; set; }
        IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    public interface IKNearestClassifierFactory : IClassifierFactory
    {
        int K { get; set; }
    }

    public abstract class BaseClassifierFactory : IClassifierFactory
    {

        public ICommonMetricProvider[] CommonMetricProviders { get; set; }
        public abstract IClassifier Create(IDictionary<Author, IEnumerable<string>> authors);
    }

    public class PerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new PerecentileBayesClassifier(authors, CommonMetricProviders);
        }
    }

    public class StupidPerecentileBayesClassifierFactory : BaseClassifierFactory
    {
        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new StupidPerecentileBayesClassifier(authors, CommonMetricProviders);
        }
    }


    public interface IExperimentLogger
    {
        void LogBenchmark(IClassifierFactory factory, IEnumerable<KeyValuePair<string, string>> parameters);
    }


    public class MSvmClassifierClassifierFactory : BaseClassifierFactory
    {

        public MSvmClassifierParams Params { get; set; }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new MSvmClassifier(Params, authors);
        }
    }

    public class KNearestBayesClassifierFactory : BaseClassifierFactory, IKNearestClassifierFactory
    {
        private int _k=25;

        public int K
        {
            get { return _k; }
            set { _k = value; }
        }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            var cls = new KNearestBayesClassifier() {K = K};
            cls.Init(authors,CommonMetricProviders.First());
            return cls;
        }
    }

    public class KNearestSumClassifierFactory : BaseClassifierFactory, IKNearestClassifierFactory
    {
        private int _k=25;

        public int K
        {
            get { return _k; }
            set { _k = value; }
        }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            var cls = new KNearestSumClassifier() { K = K };
            cls.Init(authors, CommonMetricProviders.First());
            return cls;
        }
    }


    
}
