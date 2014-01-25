using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public abstract class VBulletinParserBase
    {
        protected IVBulletinPageLoader _pageLoader;
        protected static readonly Regex _pageRegex = new Regex("Страница \\d+ из (\\d+)", RegexOptions.Compiled | RegexOptions.Singleline);
        protected static readonly Regex _digtsRegex = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.Singleline);

        protected VBulletinParserBase(IVBulletinPageLoader pageLoader)
        {
            _pageLoader = pageLoader;
        }

        public IEnumerable<PostBrief> MorePosts(int searchId, int page, int pp)
        {
            var doc = _pageLoader.MorePosts(searchId, pp, page);
            return ParsePostBriefs(null, doc).Where(x => x != null);
        }

        public IEnumerable<UserInfo> Users(int count, int page = 1)
        {
            var doc = _pageLoader.Members(count, page);
            return doc.DocumentNode.SelectNodes("//table[@cellspacing='1'][3]/tr/td[@id]/a").Select(
                node =>
                new UserInfo()
                    {
                        IdOnForum = long.Parse(node.GetAttributeValue("href", "").Split('=').Last()),
                        Nick = node.InnerText
                    }
                );

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="themIds"> Для Ачата - 6, 35 </param>
        /// <param name="searchId"></param>
        /// <param name="childForums">Включить дочерние форумы </param>
        /// <returns></returns>
        public IEnumerable<PostBrief> Posts(string user, int[] themIds, out int searchId, out int total, bool childForums = true)
        {
            var re = new Regex("Показано с \\d+ по \\d+ из (\\d+)", RegexOptions.Compiled | RegexOptions.Singleline);

            var doc = _pageLoader.SearchUser(user, themIds, childForums);

            try
            {
                total = int.Parse(re.Match(doc.DocumentNode.InnerText).Groups[1].Value);
            }
            catch (Exception exc)
            {
                searchId = 0;
                total = 0;
                return Enumerable.Empty<PostBrief>();
            }

            searchId =
                int.Parse(
                    doc.DocumentNode.SelectNodes("//a[contains(@href,'searchid')]")
                       .First()
                       .GetAttributeValue("href", "")
                       .Split('=')
                       .Last());
            return ParsePostBriefs(user, doc).Where(x => x != null);
        }

        protected abstract PostInfo ShowthreadParsePostInfo(HtmlNode node);

        public IEnumerable<PostInfo> Showthread(ThemeInfo thread,int page, out int pageCount)
        {
            var doc = _pageLoader.Showthread(thread.Id, page);
            try
            {
                pageCount =
                    int.Parse(
                        _pageRegex.Match(doc.DocumentNode.SelectSingleNode("//td[@class='vbmenu_control'][1]").InnerText)
                                  .Groups[1].Value);
            }
            catch
            {
                pageCount = 1;
            }
            
            return (doc.DocumentNode.SelectNodes("//table[starts-with(@id,'post')]")??Enumerable.Empty<HtmlNode>()).Select(ShowthreadParsePostInfo
                ).Where(x=>x!=null).Select(pi =>
                    {
                        pi.ThemeId = thread.Id;
                        pi.Theme = thread.Title;
                        return pi;
                    }).ToArray();
        }

        protected abstract string ForumDisplayIdPrefix { get; }

        public IEnumerable<ThemeInfo> ForumDisplay(int forumId, int page, out int pageCount)
        {
            var doc = _pageLoader.Forumdisplay(forumId, page);
            try
            {
                pageCount =
                    int.Parse(
                        _pageRegex.Match(doc.DocumentNode.SelectSingleNode("//td[@class='vbmenu_control'][1]").InnerText)
                                  .Groups[1].Value);
            }
            catch
            {
                pageCount = 1;
            }
            return doc.DocumentNode.SelectNodes("//tr/td[starts-with(@id,'" + ForumDisplayIdPrefix + "')]")
                      .Select(
                          ParseThemeInfo
                ).Where(x => x != null);
        }

        protected virtual ThemeInfo ParseThemeInfo(HtmlNode node)
        {
            try
            {
                return new ThemeInfo()
                    {
                        Id = ExtractIntFromAttr(node),
                        Title = node.SelectSingleNode("./div/a").InnerText,
                        Owner = node.SelectSingleNode("./div/span[@onclick]").InnerText,
                    };
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        protected static int ExtractIntFromAttr(HtmlNode node, string attr = "id")
        {
            return int.Parse(_digtsRegex.Match(node.GetAttributeValue(attr, "t0")).Captures[0].Value);
        }

        protected abstract IEnumerable<PostBrief> ParsePostBriefs(string user, HtmlDocument doc);

        public PostInfo FillPostInfo(PostBrief brief)
        {
            return new PostInfo(brief)
                {
                    HtmlText = _pageLoader.ShowthreadPost(brief.PostId).DocumentNode
                                          .SelectSingleNode(
                                              string.Format("//table[@id='post{0}']//td[@valign='top'][2]/div[2]",
                                                            brief.PostId)).InnerHtml
                };
        }
    }
}