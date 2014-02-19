namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IAlogorithm
    {
        /// <summary>
        /// Описание алгоритма
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Название
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Уровень ошибки первого рода
        /// </summary>
        double ErrorLevel { get; set; }
    }
}