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


    public interface IVBulletinParser
    {
        IEnumerable<UserInfo> Users(int count, int page = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="themIds"> Для Ачата - 6, 35 </param>
        /// <param name="searchId"></param>
        /// <param name="childForums">Включить дочерние форумы </param>
        /// <returns></returns>
        IEnumerable<PostBrief> Posts(string user, int[] themIds, out int searchId, out int total, bool childForums = true);

        IEnumerable<PostBrief> MorePosts(int searchId, int page, int pp);
        PostInfo FillPostInfo(PostBrief brief);
    }

    public class VBulletinParser : IVBulletinParser
    {
        private readonly IVBulletinPageLoader _pageLoader;

        public VBulletinParser(IVBulletinPageLoader pageLoader)
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
                          }
                ).Where(x=>x!= null);
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
}