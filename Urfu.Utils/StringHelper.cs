using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Urfu.Utils
{
    public static class StringHelper
    {
        private static Regex _resWordRegex = new Regex("[а-я]+",RegexOptions.Singleline|RegexOptions.IgnoreCase);

        public static IEnumerable<string> RussianWords(this string @str)
        {
            return _resWordRegex.Matches(@str).Cast<Capture>().Select(x=>x.Value);
        }
        static readonly char[] _vowels = new char[] { 'а', 'и', 'е', 'ё', 'о', 'у', 'ы', 'э', 'ю', 'я' };

        public static int VowelCount(this string @str)
        {
            return str.Count(_vowels.Contains);
        }
            
    }
}