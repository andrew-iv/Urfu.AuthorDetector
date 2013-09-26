using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Grabber
{
    public abstract class PostRetriever
    {
        public virtual IEnumerable<Post> GetPosts()
        {
            return null;
        }
    }

    
}
