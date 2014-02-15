using System.Collections.Generic;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class BinaryFromMultyClassifierFactory : IBinaryClassifierFactory 
    {
        private double _errorLevel=0.1d;

        Dictionary<object,BinaryFromMultyClassifier> _cahce = new Dictionary<object, BinaryFromMultyClassifier>();

        public BinaryFromMultyClassifierFactory(IClassifierFactory multyFactory)
        {
            MultyFactory = multyFactory;
        }

        public double ErrorLevel
        {
            get { return _errorLevel; }
            set { _errorLevel = value; }
        }

        public IClassifierFactory MultyFactory { get; private set; }

        public ICommonMetricProvider[] CommonMetricProviders { get; set; }
        public IBinaryClassifier Create(IDictionary<Author, IEnumerable<string>> authors, Author author)
        {

            BinaryFromMultyClassifier cls;
            if (_cahce.TryGetValue(authors, out cls) && cls.MultyFactory == MultyFactory)
            {
                cls.Author = author;
                cls.ErrorLevel = ErrorLevel;
                return cls;
            }
            cls = new BinaryFromMultyClassifier()
                {
                    Author = author,
                    ErrorLevel = ErrorLevel,
                    MultyFactory = MultyFactory
                };
            cls.Study(authors);
            _cahce[authors] = cls;
            return cls;
        }
    }
}