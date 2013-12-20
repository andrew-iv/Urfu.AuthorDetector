using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public class VBulletinParser
    {
        private readonly IVBulletinPageLoader _pageLoader;

        public VBulletinParser(IVBulletinPageLoader pageLoader)
        {
            _pageLoader = pageLoader;
        }

        public IEnumerable<string> Users(string )
        {
            return Enumerable.Range(1, pages).Select(i => _pageLoader.LoadUsers(city, i))
                .SelectMany(
                doc => doc.DocumentNode.SelectNodes("//a[contains(@class,\"user-name\")]")
                    .Select(x => x.GetAttributeValue("href", "").Split('/').Last())).ToArray();
        }
    }
}