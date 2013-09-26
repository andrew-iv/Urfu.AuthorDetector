using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Grabber;

namespace Urfu.AuthorDetector.TalksGrabber
{
    class Program
    {
        private static readonly DateTime Start = new DateTime(2011, 1, 1);
        private static readonly DateTime End = new DateTime(2013, 8, 1);

        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new RealModule());
            var posts = new List<LorPostInfo>();
            for (var date = Start;date<End;date= date.AddMonths(1))
            {
                using (var storage = kernel.Get<ILorStorage>())
                {
                    storage.SavePosts(kernel.Get<ILorGrabber>().LoadAllArchive(date.Year, date.Month));
                }
                Console.WriteLine("{0:MMMM yyyy} - stored",date);
            }
            
        }
    }
}
