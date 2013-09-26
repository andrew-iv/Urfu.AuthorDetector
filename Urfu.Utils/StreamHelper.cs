using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.Utils
{
    public static class StreamHelper
    {
        public static byte[] ReadBytes(this Stream stream,int size )
        {
            var buff = new byte[size];
            stream.Read(buff, 0, size);
            return buff;
        }

        public static byte[] ReadBytes(this Stream stream)
        {
            return ReadBytes(stream,(int)(stream.Length-stream.Position));
        }

        public static string ReadStringToEnd(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            var sr = new StreamReader(stream,encoding);
            return sr.ReadToEnd();
        }
    }
}
