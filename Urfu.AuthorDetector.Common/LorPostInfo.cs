namespace Urfu.AuthorDetector.Common
{
    public class LorPostInfo:LorPostBrief
    {
        public LorPostInfo(LorPostBrief another) : base(another)
        {
        }

        public string HtmlText { get; set; }
    }
}