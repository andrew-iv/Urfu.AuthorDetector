namespace Urfu.AuthorDetector.Common
{
    public class PostInfo:PostBrief
    {
        public PostInfo()
        {
            
        }

        public PostInfo(PostBrief another) : base(another)
        {
        }

        public string HtmlText { get; set; }
    }
}