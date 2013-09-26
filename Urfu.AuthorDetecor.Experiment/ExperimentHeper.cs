using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetecor.Experiment
{
    public static class ExperimentHeper
    {
        public static IQueryable<Author> GetAuthors(this IQueryable<Author> query, string[] ids)
        {
            return query.Where(x => ids.Contains(x.Identity));
        }
    }
}