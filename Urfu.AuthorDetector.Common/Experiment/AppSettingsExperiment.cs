using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Urfu.AuthorDetecor.Experiment
{
    public class AppSettingsExperiment : IExperiment
    {
        public AppSettingsExperiment(NameValueCollection settings = null)
        {
            if (settings == null)
            {
                settings = ConfigurationManager.AppSettings;
            }
            StartGeneral = DateTime.Parse(settings.Get("Start"));
            EndGeneral = DateTime.Parse(settings.Get("End"));
            AuthorIds = settings.Get("Authors").Split(',');
            ForumId = int.Parse(settings.Get("ForumId"));
            PostsCount = int.Parse(settings.Get("PostsCount"));
            //  RoundsCount = int.Parse(settings.Get("RoundsCount"));
        }

        public DateTime StartGeneral { get; private set; }
        public DateTime EndGeneral { get; private set; }
        public int TopAuthors { get; private set; }
        public string[] AuthorIds { get; private set; }
        public int ForumId { get; private set; }
        public int PostsCount { get; private set; }
        //    public int RoundsCount { get; private set; }
    }
}
