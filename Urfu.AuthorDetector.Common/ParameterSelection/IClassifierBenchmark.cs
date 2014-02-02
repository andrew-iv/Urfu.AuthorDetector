using Urfu.AuthorDetector.Common.Classification;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IClassifierBenchmark
    {
        /// <summary>
        /// Сколько авторов тестировать
        /// </summary>
        int AuthorsCount { get; set; }

        
        /// <summary>
        /// Количество переобучений
        /// </summary>
        int RoundCount { get; set; }

        /// <summary>
        /// Количество тестов одного и тогоже классификатора
        /// </summary>
        int TestsInRoundCount { get; set; }

        int LearningCount { get; set; }
        int MessageCount { get; set; }
        double Score(IClassifierFactory factory, int seed=0);
    }
}