using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Urfu.Utils
{
    public static class StringHelper
    {
        private static readonly Regex _resWordRegex = new Regex("[а-я]{3}[а-я]+", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public static IEnumerable<string> RussianWords(this string @str)
        {
            return _resWordRegex.Matches(@str).Cast<Capture>().Select(x=>x.Value);
        }
        static readonly char[] _vowels = new char[] { 'а', 'и', 'е', 'ё', 'о', 'у', 'ы', 'э', 'ю', 'я' };

        public static int VowelCount(this string @str)
        {
            return str.ToLower().Count(_vowels.Contains);
        }

        public static IEnumerable<string> NGramms(this string text, int n)
        {
            if (text.Length < n)
                return Enumerable.Empty<string>();
            return Enumerable.Range(0, text.Length - n + 1).Select(i => text.Substring(i, n));
        }

    }
}