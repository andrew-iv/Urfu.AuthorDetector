using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.DataLayer;
using Urfu.AuthorDetector.Grabber;
using Urfu.AuthorDetector.Grabber.Flamp;
using Urfu.AuthorDetector.Grabber.VBullitin;

namespace Urfu.AuthorDetector.TalksGrabber
{
    class Program
    {
        private static readonly DateTime Start = new DateTime(2012, 11, 1);
        private static readonly DateTime End = new DateTime(2012, 12, 1);
        //private static readonly string forum = "http://hpc.name/";

        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new RealModule());

            
                using (var storage = new ForumIdStorage(kernel.Get<IStatisticsContext>(), 3))
                {
                    var grabber =
                        new VBulletinGrabber(new AchatBulletinParser(new VBulletinPageLoader(storage.Forum.ForumUrl, Encoding.GetEncoding(1251))), new VBulletinLog());
                    storage.SavePosts(grabber.LoadForum(46,maxPage:300,ignoreThemes:
                        new SortedSet<long>(
                            new long[]{104005,139152 }
                        )));
                }
        }
    }
}
