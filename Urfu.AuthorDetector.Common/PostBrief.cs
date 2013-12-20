using System;

namespace Urfu.AuthorDetector.Common
{
    public class PostBrief
    {
        public PostBrief()
        {
            
        }

        protected PostBrief(PostBrief another)
        {
            Theme = another.Theme;
            Time = another.Time;
            ThemeId = another.ThemeId;
            PostId = another.PostId;
            Nick = another.Nick;
        }

        public string Nick { get; set; }
        public string Theme { get; set; }
        public DateTime? Time{ get; set; }
        public long ThemeId { get; set; }
        public int PostId { get; set; }
    }
}