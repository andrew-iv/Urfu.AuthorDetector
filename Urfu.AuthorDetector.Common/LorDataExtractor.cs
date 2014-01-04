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

        private string NodeText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment) return "";
            if (node.NodeType == HtmlNodeType.Text) return HttpUtility.HtmlDecode(node.InnerHtml.Replace(Environment.NewLine,""));
            if (node.NodeType == HtmlNodeType.Element && node.Name == "br") return Environment.NewLine;

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

                return NodeText(htmlDoc.DocumentNode);
            }
            catch (Exception)
            {

                return post.Text;
            }
        }
    }
}