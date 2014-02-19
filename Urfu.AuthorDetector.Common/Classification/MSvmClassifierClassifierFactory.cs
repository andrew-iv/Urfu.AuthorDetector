using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public class MSvmClassifierClassifierFactory : BaseClassifierFactory
    {

        public MSvmClassifierParams Params { get; set; }

        public override IClassifier Create(IDictionary<Author, IEnumerable<string>> authors)
        {
            return new MSvmClassifier(Params, authors);
        }
    }
}