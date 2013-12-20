using System.Collections.Generic;
using Opcorpora.Dictionary.Xsd;

namespace Opcorpora.Dictionary
{
    public interface IOpcorporaDictionary
    {
        IEnumerable<lemmaItem> GetLemma(string word);
        IEnumerable<dictionaryGrammeme> Grammemes { get; }
    }
}