namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IKNearestClassifierFactory : IClassifierFactory
    {
        /// <summary>
        /// кол-во ближайших
        /// </summary>
        int K { get; set; }
    }
}