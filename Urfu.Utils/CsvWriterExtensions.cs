using CsvHelper;

namespace Urfu.Utils
{
    public static class CsvWriterExtensions
    {
        public static void WriteFields(this CsvWriter writer, params object[] strings)
        {
            foreach (var item in strings)
            {
                writer.WriteField(item);
            }
            writer.NextRecord();
        }
    }
}