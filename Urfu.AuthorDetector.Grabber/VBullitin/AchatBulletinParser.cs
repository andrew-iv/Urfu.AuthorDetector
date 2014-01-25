using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Urfu.AuthorDetector.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public class UserInfo
    {
        public long IdOnForum { get; set; }
        public string Nick { get; set; }
    }


    public class AchatBulletinParser : VBulletinParserBase, IVBulletinParser
    {
        public AchatBulletinParser(IVBulletinPageLoader pageLoader) : base(pageLoader)
        {
        }

        protected override string ForumDisplayIdPrefix
        {
            get { return "t"; }
        }



        protected override IEnumerable<PostBrief> ParsePostBriefs(string user, HtmlDocument doc)
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
                              });
        }

        protected override PostInfo ShowthreadParsePostInfo(HtmlNode node)
        {
            PostInfo pi;
            try
            {
                pi = new PostInfo() { };
                var postNode = node.SelectSingleNode(".//td[@valign='top'][2]/div[2]");
                pi.HtmlText = postNode.InnerHtml;
                pi.PostId = ExtractIntFromAttr(postNode);
                pi.UserId = ExtractIntFromAttr(node.SelectSingleNode(".//a[starts-with(@href,'member')]"),"href");
                pi.Nick = node.SelectSingleNode(".//a/font").InnerText;
                pi.Time =
                    DateTime.Parse(
                        node.SelectSingleNode(".//td[@class='thead'][1]/text()").InnerText
                            .Trim());

            }
            catch
            {
                return null;
            }
            return pi;
        }
    }

    public class ThemeInfo
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
    }
}