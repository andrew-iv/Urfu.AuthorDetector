using System.Text;
using HtmlAgilityPack;
using System.Linq;
using Urfu.AuthorDetector.Grabber.Common;

namespace Urfu.AuthorDetector.Grabber.VBullitin
{
    public interface IVBulletinPageLoader
    {
        HtmlDocument Members(int count, int page);
        HtmlDocument MorePosts(int searchId, int count, int page);

        HtmlDocument SearchUser(string userName, int[] forumchoice, bool childForums);
        HtmlDocument MemberPosts(int id);
        HtmlDocument ShowthreadPost(int id);
        HtmlDocument Load(string url, ILoadDocumentParameters parameters = null);
        HtmlDocument Forumdisplay(int forumId,int page);
        HtmlDocument Showthread(long id, int page = 1);
    }

    public class VBulletinPageLoader:BasePageLoader, IVBulletinPageLoader
    {
        private readonly string _prefix;
        private readonly Encoding _encoding;

        public VBulletinPageLoader(string prefix, Encoding encoding= null)
        {
            _prefix = prefix;
            _encoding = encoding;
        }

        public HtmlDocument Forumdisplay(int forumId,int page)
        {
            return
                Load(
                    string.Format(
                        "{0}/forumdisplay.php?f={1}&order=desc&page={2}&daysprune=-1",
                        _prefix, forumId, page));
        }

        public HtmlDocument Showthread(long id,int page =1)
        {
            return
                Load(
                    string.Format(
                        "{0}/showthread.php?t={1}&pp=40&page={2}",
                        _prefix, id,page));
        }


        protected override ILoadDocumentParameters DefaultParameters
        {
            get
            {
                var def = new LoadDocumentParameters(base.DefaultParameters);
                if (_encoding != null)
                    def.Encoding = _encoding;
                return def;
            }
        }

        public HtmlDocument Members(int count, int page)
        {
            return
                Load(
                    string.Format(
                        "{0}/memberlist.php?order=DESC&sort=posts&pp={1}&page={2}",
                        _prefix, count,page));
        }

        public HtmlDocument MorePosts(int searchId, int count, int page)
        {
            return Load(string.Format("{0}/search.php?searchid={1}&pp={2}&page={3}",_prefix, searchId, count, page));
        }


        public HtmlDocument SearchUser(string userName, int[] forumchoice, bool childForums)
        {
            return Load(
                string.Format(
                "{0}/search.php?s=&do=process&query=&titleonly=0&searchuser={1}&starteronly=0&exactname=1&replyless=0&replylimit=0&searchdate=0&beforeafter=after&sortby=lastpost&order=descending&showposts=1&" +
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