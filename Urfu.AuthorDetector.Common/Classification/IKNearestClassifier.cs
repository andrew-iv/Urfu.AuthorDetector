namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// классификатор метода ближайших сосодей
    /// </summary>
    public interface IKNearestClassifier: IClassifier
    {
        int K { get; set; }
    }
}