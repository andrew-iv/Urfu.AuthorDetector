using System.Collections.Generic;
using System.Linq;
using Opcorpora.Dictionary.Xsd;

namespace Opcorpora.Dictionary
{
    public static class OpencorporaDictionaryExtensions
    {
        public static IEnumerable<lemmaItemG> GetlemmaItemGs(this lemmaItem lemma)
        {
            return (lemma.Parent.l.g ?? new lemmaItemG[]{}) .Union(lemma.g ?? new lemmaItemG[] {});
        }
            
            
    }

    public interface IOpcorporaDictionary
    {
        IEnumerable<lemmaItem> GetLemma(string word);
        IEnumerable<dictionaryGrammeme> Grammemes { get; }
    }
}