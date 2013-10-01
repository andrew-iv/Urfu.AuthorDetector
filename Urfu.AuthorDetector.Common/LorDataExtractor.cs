using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public class LorDataExtractor : IDataExtractor
    {
        public string GetText(Post post)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(post.Text);
                foreach (var delDiv in (htmlDoc.DocumentNode.SelectNodes("//div")?? Enumerable.Empty<HtmlNode>()))
                {
                    delDiv.Remove();
                }
                foreach (var delDiv in (htmlDoc.DocumentNode.SelectNodes("//i") ?? Enumerable.Empty<HtmlNode>()))
                {
                    delDiv.Remove();
                }

                return HttpUtility.HtmlDecode(htmlDoc.DocumentNode.InnerText);
            }
            catch (Exception)
            {

                return post.Text;
            }
        }
    }
}