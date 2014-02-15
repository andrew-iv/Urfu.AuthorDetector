using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common
{
    public class FromHtmlDataExtractor : IDataExtractor
    {
        private Dictionary<int, string> _cache = new Dictionary<int, string>(); 


        private string NodeText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment) return "";
            if (node.NodeType == HtmlNodeType.Text) return HttpUtility.HtmlDecode(node.InnerHtml.Replace(Environment.NewLine,""));
            if (node.NodeType == HtmlNodeType.Element && node.Name == "br") return "\n";

            var res = string.Join("",
                        node.ChildNodes.
                        Select(NodeText));
            if (node.Name == "p")
                return " " + res + " ";
            return res;

        }

        public string GetText(Post post)
        {
            try
            {
                string fromCache;
                if (_cache.TryGetValue(post.Id, out fromCache))
                    return fromCache;

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

                /*
                foreach (var delDiv in (htmlDoc.DocumentNode.SelectNodes("//br") ?? Enumerable.Empty<HtmlNode>()))
                {
                    delDiv.ParentNode.ReplaceChild(new HtmlNode(HtmlNodeType.Text,htmlDoc,0){InnerHtml = "&brrrrr;"},  delDiv)

                    delDiv.Remove();
                }
                */

                var res = NodeText(htmlDoc.DocumentNode);
                _cache[post.Id] = res;
                return res;

            }
            catch (Exception)
            {

                return post.Text;
            }
        }
    }
}