using System.Collections.Generic;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IBinaryClassifier : IAlogorithm
    {
        Author Author { get; set; }
        bool Confirm(IEnumerable<string> items);
        
    }
}