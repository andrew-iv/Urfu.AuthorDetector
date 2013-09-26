using System;
using System.Collections.Generic;

namespace Urfu.Utils
{
    public class DoubleComparer:IComparer<double>
    {
        private readonly double _precision;

        public DoubleComparer(double precision=0.001d)
        {
            if (precision <= 0)
                throw new ArgumentOutOfRangeException("precision");
            _precision = precision;
        }

        public int Compare(double x, double y)
        {
            if (Math.Abs(x - y) < _precision) return 0;
            if (x < y)
                return -1;       
            else
                return 1;        
        }
    }
}