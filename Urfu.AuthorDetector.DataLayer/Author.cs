//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Urfu.AuthorDetector.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Author
    {
        public Author()
        {
            this.Post = new HashSet<Post>();
        }
    
        public int Id { get; set; }
        public string Identity { get; set; }
        public string DisplayName { get; set; }
    
        public virtual ICollection<Post> Post { get; set; }
        public virtual Forum Forum { get; set; }
    }
}