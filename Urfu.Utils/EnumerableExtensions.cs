using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.Utils
{
    public static class EnumerableExtensions
    {
        public static T[] RandomSubArray<T>(this IEnumerable<T> array, int k, Random rand)
        {
            var asArray = array as T[] ?? array.ToArray();
            int n = asArray.Length;

            if(k > n) throw new ArgumentOutOfRangeException("k","k долно быть меньше размера массива");

            T[] ret = new T[k];
            bool[] keys = new bool[n];

            bool flag = k <= n / 2;

            for (int i = 0, stop = flag ? k : n - k; i < stop; i++)
            {
                while (true)
                {
                    int key = rand.Next(0, n);
                    if (!keys[key])
                    {
                        keys[key] = true;
                        break;
                    }
                }
            }

            int p = 0;
            for (int i = 0; i < k; i++)
            {
                while (keys[p] ^ flag)
                {
                    p++;
                }
                ret[i] = asArray[p++];
            }
            return ret;
        }
    }
}