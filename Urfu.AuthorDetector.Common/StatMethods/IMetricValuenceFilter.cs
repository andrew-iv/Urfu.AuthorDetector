using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Testing;

/*
namespace Urfu.AuthorDetector.Common.StatMethods
{
    public interface IMetricValuenceFilter
    {
        /// <summary>
        /// Статистика по авторам
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        bool TestByOtherAuthors(IEnumerable<IEnumerable<double>> items);

        /// <summary>
        /// общий фильтр
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        bool TestAll(IEnumerable<double> items);
    }

    public abstract class TwoSampleTestMetricValuenceFilter : IMetricValuenceFilter
    {

        protected TwoSampleTestMetricValuenceFilter(double pvalue, double totalShare = 0.25)
        {
            _pvalue = pvalue;
            _totalShare = totalShare;
        }

        protected abstract TwoSampleTest CreateTest();

        private double _pvalue;
        private double _totalShare;

        public bool TestByOtherAuthors(IEnumerable<IEnumerable<double>> items)
        {
            var array = items.Select(x => x.ToArray()).ToArray();
            var test = CreateTest();
            int testFails = 0;

            foreach (var i in Enumerable.Range(0,array.Length-1))
            {
                foreach (var j in Enumerable.Range(i, array.Length-i-1))
                {
                    test.Sample1 = new NumericalVariable(array[i]);
                    test.Sample2 = new NumericalVariable(array[j]);
                    if (test.PValue < _pvalue)
                        testFails++;
                }
            }
            return array.Length*(array.Length - 1)*_totalShare >= 2*testFails;
        }

        public bool TestAll(IEnumerable<double> items)
        {
            return true;
        }
    }

     public class KSTwoSampleTestMetricValuenceFilterImpl : TwoSampleTestMetricValuenceFilter
    {
        public KSTwoSampleTestMetricValuenceFilterImpl(double pvalue=0.01, double totalShare = 0.25)
            : base(pvalue, totalShare)
        {
        }

        protected override TwoSampleTest CreateTest()
        {
            return new TwoSampleKolmogorovSmirnovTest();
        }
    }
}*/