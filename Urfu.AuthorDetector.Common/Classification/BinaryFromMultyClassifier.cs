using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class BinaryFromMultyClassifier : IBinaryClassifier
    {
        public Author Author { get; set; }

        public IClassifierFactory MultyFactory { get; set; }

        private IClassifier _classifier;

        public bool Confirm(IEnumerable<string> items)
        {
            if (!_classifier.Authors.Contains(Author)) 
                throw new PropertyConstraintException("Author должен быть среди обученных");

            var count = (int)Math.Floor(_classifier.Authors.Count() * ErrorLevel + 0.0001);
            if (count <= 0) count = 1;
            return _classifier.ClassificatePosts(items, count).Any(x => x == Author);
        }

        public double ErrorLevel { get; set; }
        

        public void Study(IDictionary<Author, IEnumerable<string>> items)
        {
            _classifier = MultyFactory.Create(items);
        }


        public string Description { get; private set; }
        public string Name { get; private set; }
    }


}