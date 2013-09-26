using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Urfu.Utils
{
    public static class HtmlExtractor
    {
        public static HtmlDocument ToHtmlDocument(this string str)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(str);
            return doc;
        }

        public static List<string> ExtractParagrahps(HtmlDocument document, out List<HtmlNode> otherNodes)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();
            try
            {
                nodes = document.DocumentNode.SelectNodes("//p").ToList();
            }
            catch
            {
                nodes = new List<HtmlNode>();
            }
            
            otherNodes = nodes.SelectMany(x => x.ChildNodes.Where(xx=>xx.NodeType != HtmlNodeType.Text)).ToList();
            return nodes.Select(x =>string.Join(" ", x.ChildNodes.Where(xx => xx.NodeType == HtmlNodeType.Text).Select(xx => xx.InnerHtml))).ToList();
        }
    }
}