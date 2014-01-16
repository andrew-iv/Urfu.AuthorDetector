using System;

namespace Urfu.AuthorDetecor.Experiment
{
    public interface IExperiment
    {
        DateTime StartGeneral { get; }
        DateTime EndGeneral { get; }
        int TopAuthors { get; }
        int ForumId { get; }
        int PostsCount { get; }
        //int RoundsCount { get; }
    }
}