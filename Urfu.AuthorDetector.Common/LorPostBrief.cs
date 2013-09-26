using System;

namespace Urfu.AuthorDetector.Common
{
    public class LorPostBrief
    {
        public LorPostBrief()
        {
            
        }

        protected LorPostBrief(LorPostBrief another)
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
        public int ThemeId { get; set; }
        public int PostId { get; set; }
    }
}