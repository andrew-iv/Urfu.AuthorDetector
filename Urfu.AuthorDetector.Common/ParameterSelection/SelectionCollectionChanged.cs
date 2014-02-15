namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class SelectionCollectionChanged
    {
        public SelectionCollectionChangedType Type{get; internal set; }
        public int Index {get; internal set; }
        public string Name {get; internal set; }
    }
}