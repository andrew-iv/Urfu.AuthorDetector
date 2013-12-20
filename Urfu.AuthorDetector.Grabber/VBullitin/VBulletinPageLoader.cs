using HtmlAgilityPack;
using System.Linq;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public interface IVBulletinPageLoader
    {
        HtmlDocument Members(int count);
        HtmlDocument SearchUser(string userName, int[] forumchoice, bool childForums);
        HtmlDocument MemberPosts(int id);
        HtmlDocument ShowthreadPost(int id);
        HtmlDocument Load(string url, ILoadDocumentParameters parameters = null);
    }

    public class VBulletinPageLoader:BasePageLoader, IVBulletinPageLoader
    {
        private readonly string _prefix;

        public VBulletinPageLoader(string prefix)
        {
            _prefix = prefix;
        }

        public HtmlDocument Members(int count)
        {
            return
                Load(
                    string.Format(
                        "{0}/memberlist.php?postslower=0&postsupper=0&ausername=&homepage=&icq=&aim=&yahoo=&msn=&joindateafter=&joindatebefore=&lastpostafter=&lastpostbefore=&order=DESC&sort=posts&pp={1}&ltr=",
                        _prefix, count));
        }


        public HtmlDocument SearchUser(string userName, int[] forumchoice, bool childForums)
        {
            return Load(
                string.Format(
                "{0}/search.php?s=&do=process&query=&titleonly=0&searchuser={1}&starteronly=0&exactname=1&replyless=0&replylimit=0&searchdate=0&beforeafter=after&sortby=lastpost&order=descending&showposts=0&" +
                "{2}&childforums={3}"
                , _prefix, userName,
                string.Join("&",
                forumchoice.Select(id => "forumchoice[]="+id)),childForums?'1':'0'));
        }

        public HtmlDocument MemberPosts(int id)
        {
            return
                Load(
                    string.Format(
                        "{0}/search.php?searchid={1}&pp=99900&page=1", _prefix, id));
        }

        public HtmlDocument ShowthreadPost(int id)
        {
            return
                Load(
                    string.Format(
                        "{0}//showthread.php?p={1}", _prefix, id));
        }
    }
}