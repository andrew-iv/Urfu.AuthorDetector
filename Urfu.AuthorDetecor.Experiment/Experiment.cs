using System;

namespace Urfu.AuthorDetecor.Experiment
{
    public class Experiment : IExperiment
    {
        public DateTime StartGeneral { get; set; }
        public DateTime EndGeneral { get; set; }
        public int TopAuthors { get;set; }
        public int ForumId { get; set; }
        public int PostsCount { get; set; }
    }
}