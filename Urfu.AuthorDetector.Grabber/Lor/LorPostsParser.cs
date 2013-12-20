using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Grabber.Lor;

namespace Urfu.AuthorDetector.Grabber
{
    public class LorPostsParser : ILorPostsParser
    {
        private readonly ILorPageLoader _pageLoader;

        private readonly Regex _listItemRegex = new Regex("<td><a\\s*href=\"jump-message.jsp\\?msgid=(\\d+)&amp;cid=(\\d+)\"" +
                                                          ".*?>(.*?)</a></td>" +
                                                          ".*?<time datetime=\"(.*?)\">.*?</time>", RegexOptions.Compiled | RegexOptions.Singleline);

        public LorPostsParser(ILorPageLoader pageLoader)
        {
            _pageLoader = pageLoader;
        }



        public IEnumerable<PostBrief> GetPostsList(string nick, int offset = 0)
        {
            var str = new StringWriter();
            _pageLoader.LoadPostsList(nick, offset).Save(str);
            var page = str.ToString();
            var matches = _listItemRegex.Matches(page);
            return from Match match in matches
                   select new PostBrief()
                       {
                           ThemeId = int.Parse(match.Groups[1].Value),
                           PostId = int.Parse(match.Groups[2].Value),
                           Theme = match.Groups[3].Value,
                           Time = DateTime.Parse(match.Groups[4].Value),
                           Nick = nick
                       };
        }

        public IEnumerable<string> ParseThemeLinks(int offset, int year, int month, string category = "talks")
        {
            var doc = _pageLoader.LoadArchive(year, month, offset, category);
            var nodes = doc.DocumentNode.SelectNodes("//table[contains(@class,\"message-table\")]/tbody/tr//a[@href]");
            if (nodes == null)
                return Enumerable.Empty<string>();
            return nodes
               .Select(x => LorGrabber.LorUrl+ x.Attributes.First(attr => attr.Name == "href").Value);
        }



        public IEnumerable<PostInfo> ParseComments(string url)
        {

            var doc = _pageLoader.Load(url);
            if (doc == null)
                throw new ArgumentException("Не удалось загрузить документ", "url");
            return ParsePosts(doc);

        }


        private IEnumerable<PostInfo> ParsePosts(HtmlDocument doc)
        {
            try
            {
                const string topicIdStarts = "topic-";
                var theme = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                var themeArticle = doc.DocumentNode.SelectSingleNode("//article[contains(@id,'" + topicIdStarts + "')]");
                var themeId = int.Parse(themeArticle.Id.Substring(topicIdStarts.Length));
                return (doc.DocumentNode.SelectNodes("//article[starts-with(@id, \"comment-\")]")??
                Enumerable.Empty<HtmlNode>()
                ).Select(article =>
                {
                    try
                    {
                        if (article == null)
                            return null;
                        var msgBody = article.SelectSingleNode(".//div[contains(@class,\"msg_body\")]");
                        if (msgBody == null)
                            return null;
                        var nickNode = msgBody.SelectSingleNode(".//div[@class=\"sign\"]/a");
                        if (nickNode == null)
                            return null;
                        var timeNode = msgBody.SelectSingleNode(".//time[@datetime]");


                        var nick = nickNode.InnerText.Trim();
                        foreach (var delDiv in msgBody.ChildNodes.Where(x => x.Name == "div").ToArray())
                        {
                            msgBody.RemoveChild(delDiv);
                        }
                        return new PostInfo(new PostBrief()
                        {
                            Nick = nick,
                            PostId = int.Parse(article.Attributes.First(x => x.Name == "id").Value.Substring(8)),
                            Theme = theme,
                            ThemeId = themeId,
                            Time =
                                timeNode != null
                                    ? DateTime.Parse(timeNode.Attributes.First(x => x.Name == "datetime").Value)
                                    : (DateTime?)null
                        })
                        {
                            HtmlText = msgBody.InnerHtml
                        };
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(x => x != null);
            }
            catch (Exception)
            {
                return Enumerable.Empty<PostInfo>();
            }
        }


        public IEnumerable<PostInfo> ParsePostsInTheme(int themeId, int page, out bool haveNextPage)
        {
            haveNextPage = false;
            var doc = _pageLoader.LoadTheme(themeId, page);
            if (doc == null)
                throw new Exception("Не удалось загрузить документ");
            haveNextPage = doc.DocumentNode.SelectSingleNode(
                string.Format("//a[contains(@href,\"/{0}/page{1}\")]", themeId, page + 1)) != null;
            return ParsePosts(doc);

        }


        public PostInfo FillPost(PostBrief postBrief)
        {
            var doc = _pageLoader.LoadThemeByPostId(Convert.ToInt32(postBrief.ThemeId), postBrief.PostId);
            if (doc == null)
                throw new Exception("Не удалось загрузить документ");
            var node = doc.DocumentNode.SelectSingleNode
                (string.Format("//*[@id='comment-{0}']/div[@class='msg-container']/div[contains(@class,'msg_body')]", postBrief.PostId));
            if (node == null)
                return null;
            foreach (var delDiv in node.ChildNodes.Where(x => x.Name == "div").ToArray())
            {
                node.RemoveChild(delDiv);
            }
            return new PostInfo(postBrief) { HtmlText = node.InnerHtml };
        }
    }
}