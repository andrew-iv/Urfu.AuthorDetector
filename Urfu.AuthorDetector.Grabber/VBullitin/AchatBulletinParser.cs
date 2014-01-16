using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public class UserInfo
    {
        public long IdOnForum { get; set; }
        public string Nick { get; set; }
    }


    public class AchatBulletinParser : IVBulletinParser
    {
        private readonly IVBulletinPageLoader _pageLoader;
        private static readonly Regex _pageRegex = new Regex("Страница \\d+ из (\\d+)", RegexOptions.Compiled | RegexOptions.Singleline);

        public AchatBulletinParser(IVBulletinPageLoader pageLoader)
        {
            _pageLoader = pageLoader;
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
            return ParsePostBriefs(user, doc);
        }


        public IEnumerable<PostBrief> MorePosts(int searchId, int page, int pp)
        {
            var doc = _pageLoader.MorePosts(searchId, pp, page);
            return ParsePostBriefs(null, doc);
        }

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

            
            return (doc.DocumentNode.SelectNodes("//table[starts-with(@id,'post')]")??Enumerable.Empty<HtmlNode>()).Select(node =>
                {
                    PostInfo pi;
                    try
                    {
                        pi = new PostInfo() {};
                        pi.HtmlText = node.SelectSingleNode(
                            ".//td[@valign='top'][2]/div[2]"
                            ).InnerHtml;
                        pi.PostId = int.Parse(node.GetAttributeValue("id", "post0").Substring(4));
                        pi.ThemeId = thread.Id;
                        pi.Theme = thread.Title;
                        var userNode = node.SelectSingleNode(".//a[starts-with(@href,'member')][@class]");
                        pi.UserId = int.Parse(userNode.GetAttributeValue("href", "adga?=0").Split('=').Last());
                        pi.Nick = userNode.InnerText;
                        pi.Time =
                            DateTime.Parse(
                                node.SelectSingleNode(".//td[@class='thead']/div[a[@name]]/text()[3]").InnerText
                                .Trim());

                    }
                    catch
                    {
                        return null;
                    }
                    return pi;
                }).Where(x=>x!=null);
        }

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
            return doc.DocumentNode.SelectNodes("//tr/td[starts-with(@id,'t')]")
          .Select(
              node =>
              {
                  try
                  {
                      return new ThemeInfo()
                      {
                          Id = int.Parse(node.GetAttributeValue("id","t0").Substring(1)),
                          Title = node.SelectSingleNode("./div/a").InnerText,
                          Owner = node.SelectSingleNode("./div/span[@onclick]").InnerText,

                         /*Nick = user,
                           PostId = int.Parse(
                               node.SelectSingleNode(".//a[contains(@href,'showthread.php?p')]")
                                   .GetAttributeValue("href", "")
                                   .Split('#', '=')[1]),
                           Theme = themeNode.SelectSingleNode(".//strong").InnerHtml,
                           ThemeId = int.Parse(themeNode.GetAttributeValue("href", "").Split('=').Last()),
                           Time =
                               DateTime.Parse(
                                   node.SelectSingleNode(".//td[@class='thead']/text()[3]")
                                       .InnerText.Trim()),*/
                      };
                  }
                  catch (Exception exc)
                  {
                      return null;
                  }
              }
            ).Where(x => x != null);
        }

        private static IEnumerable<PostBrief> ParsePostBriefs(string user, HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//table[@style='margin:6px 0px 6px 0px']")
                      .Select(
                          node =>
                          {
                              var themeNode = node.SelectSingleNode(".//a[contains(@href,'showthread.php?t')]");
                              try
                              {
                                  return new PostBrief()
                                      {
                                          Nick = user,
                                          PostId = int.Parse(
                                              node.SelectSingleNode(".//a[contains(@href,'showthread.php?p')]")
                                                  .GetAttributeValue("href", "")
                                                  .Split('#', '=')[1]),
                                          Theme = themeNode.SelectSingleNode(".//strong").InnerHtml,
                                          ThemeId = int.Parse(themeNode.GetAttributeValue("href", "").Split('=').Last()),
                                          Time =
                                              DateTime.Parse(
                                                  node.SelectSingleNode(".//td[@class='thead']/text()[3]")
                                                      .InnerText.Trim()),
                                      };
                              }
                              catch (Exception exc)
                              {
                                  return null;
                              }
                          }).Where(x => x != null);
        }

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

    public class ThemeInfo
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
    }
}