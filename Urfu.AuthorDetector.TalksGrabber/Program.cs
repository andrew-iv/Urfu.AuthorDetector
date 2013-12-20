using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Grabber.Flamp;

namespace Urfu.AuthorDetector.TalksGrabber
{
    class Program
    {
        private static readonly DateTime Start = new DateTime(2012, 11, 1);
        private static readonly DateTime End = new DateTime(2012, 12, 1);

        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new RealModule());
            var posts = new List<PostInfo>();
            foreach (var city in new Dictionary<string, int>() { { "novosibirsk", 3 }, { "ekaterinburg", 2 }, { "moscow",1 } })
            {
                using (var storage = kernel.Get<IForumStorage>())
                {
                    storage.SavePosts(kernel.Get<IFlampGrabber>().LoadAllArchive(city.Key, city.Value));
                }
                Console.WriteLine("{0} - stored",city);
            }
        }
    }
}
