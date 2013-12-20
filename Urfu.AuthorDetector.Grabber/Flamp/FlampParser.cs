using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.Flamp
{
    public class FlampParser : IFlampParser
    {
        private readonly IFlampLoader _pageLoader;

        public FlampParser(IFlampLoader pageLoader)
        {
            _pageLoader = pageLoader;
        }

        public IEnumerable<string> Users(string city, int pages)
        {
            return Enumerable.Range(1, pages).Select(i => _pageLoader.LoadUsers(city, i))
                .SelectMany(
                doc => doc.DocumentNode.SelectNodes("//a[contains(@class,\"user-name\")]")
                    .Select(x => x.GetAttributeValue("href","").Split('/').Last())).ToArray();
        }


        public IEnumerable<PostInfo> Posts(string user)
        {
            var doc = _pageLoader.LoadUser(user);
            var reviewItems = doc.DocumentNode.SelectNodes("//li[@class='review-item']").ToArray();
            return reviewItems.Select(ri => new PostInfo
                {
                    HtmlText = ri.SelectSingleNode(".//p[@itemprop='reviewBody']").InnerHtml,
                    Time = DateTime.Parse(
                        ri.SelectSingleNode(".//meta[@itemprop='datePublished']")
                          .GetAttributeValue("content", "_")),
                    Theme = ri.SelectSingleNode(".//h2[@class='title']").InnerText,
                    ThemeId = long.Parse(
                        ri.SelectSingleNode(".//a[@class='link-short-list']")
                          .GetAttributeValue("href", "")
                          .Split('-')
                          .Last()),
                    Nick = user,
                    PostId = int.Parse(
                        ri.SelectSingleNode(".//a[@class='message-social-link message-popup-link']")
                          .GetAttributeValue("data-id", "_")
                          .Split('-')
                          .Last())
                }).ToArray();

        }
    }
}