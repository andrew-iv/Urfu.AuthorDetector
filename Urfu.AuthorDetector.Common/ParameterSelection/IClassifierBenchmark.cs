using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IClassifierBenchmark : IBenchmark
    {
        /// <summary>
        /// Оценить точность
        /// </summary>
        /// <param name="factory">фабрика классификаторов</param>
        /// <param name="seed">Начальное значение ГСЧ</param>
        /// <returns></returns>
        double Score(IClassifierFactory factory, int seed=0);

        /// <summary>
        /// Оценить точность
        /// </summary>
        /// <param name="factory">фабрика классификаторов</param>
        /// <param name="topN">Сколько авторов</param>
        /// <param name="seed">Начальное значение ГСЧ</param>
        /// <returns></returns>
        double ScoreTopN(IClassifierFactory factory,int topN, int seed = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory">фабрика классификаторов</param>
        /// <param name="seed">Начальное значение ГСЧ</param>
        /// <param name="reliableFalse">Доля ответов, помеченных как достоверные при условии неверности ответа</param>
        /// <param name="reliableTrue">Доля ответов, помеченных как достоверные при условии верности ответа</param>
        /// <returns></returns>
        double Score(IClassifierFactory factory, out double reliableFalse, out double reliableTrue, int seed = 0);
    }
}