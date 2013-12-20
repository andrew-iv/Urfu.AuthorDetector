using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Opcorpora.Dictionary.Xsd;
using Wintellect.PowerCollections;

namespace Opcorpora.Dictionary
{
    public class OpcorporaDictionary : IOpcorporaDictionary
    {
        

        private dictionary _xsdType;
        private Dictionary<string, List<lemmaItem>> _lemmaDict;

        private void Init(Stream stream)
        {
            var ser = new XmlSerializer(typeof (Xsd.dictionary));
            _xsdType = ser.Deserialize(stream) as Xsd.dictionary;
            _lemmaDict = new Dictionary<string, List<lemmaItem>>();
            foreach (var lemma in _xsdType.lemmata)
            {
                foreach (var f in lemma.f)
                {
                    if (!_lemmaDict.ContainsKey(f.t))
                    {
                        _lemmaDict[f.t] = new List<lemmaItem>();
                    }
                    f.Parent = lemma;
                    _lemmaDict[f.t].Add(f);
                }
            }
        }

        public IEnumerable<lemmaItem> GetLemma(string word)
        {
            List<lemmaItem> res;
             return _lemmaDict.TryGetValue(word, out  res)?res:Enumerable.Empty<lemmaItem>() ;
        }

        public IEnumerable<dictionaryGrammeme> Grammemes { get { return _xsdType.grammemes; } }

        public OpcorporaDictionary(string fileName)
        {
            using (var str = File.OpenRead(fileName))
            {
                Init(str);
            }
        }

        public OpcorporaDictionary(Stream stream)
        {
            Init(stream);
        }
    }
}
