namespace Urfu.Utils
{
    public static class SqlStringHelper
    {
        public const int MaxNvarcharLength = 4000;
        public static string CutString(this string @this, int length = MaxNvarcharLength)
        {
            
            return @this.Length > length?  @this.Substring(0, length):@this;
        }
    }
}