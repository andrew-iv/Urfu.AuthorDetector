namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IBenchmark
    {
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

        /// <summary>
        /// Сколько авторов тестировать
        /// </summary>
        int AuthorsCount { get; set; }

        double FalseAuthorProbability { get; set; }
    }
}