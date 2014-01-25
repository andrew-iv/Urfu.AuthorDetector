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

        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new RealModule());
            //kernel.Rebind<IVBulletinParser>().To<XakeNameBulletinParser>();


            using (var storage = new ForumIdStorage(kernel.Get<IStatisticsContext>(), 1004))
                {
                    var grabber =
                        new VBulletinGrabber(new XakeNameBulletinParser(new VBulletinPageLoader(storage.Forum.ForumUrl, Encoding.GetEncoding(1251))), new VBulletinLog());
                    storage.SavePosts(grabber.LoadForum(41, maxPage: 300, ignoreThemes:
                        new SortedSet<long>(
                            new long[] { 44825 }
                        )));
                }
        }
    }
}
