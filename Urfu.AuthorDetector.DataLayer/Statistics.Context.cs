﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StatisticsContainer : DbContext, IStatisticsContext
    {
        public StatisticsContainer()
            : base("name=StatisticsContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    	public new void SaveChanges()
            {
                base.SaveChanges();
            }
    
            public void Add<T>(T entity) where T : class
            {
                this.Set<T>().Add(entity);
            }
    
    
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Theme> Themes { get; set; }
        public IDbSet<Author> Authors { get; set; }
        public IDbSet<Forum> ForumSet { get; set; }
        public IDbSet<ClassifierVersion> ClassifierVersionSet { get; set; }
        public IDbSet<ClassifierResult> ClassifierResultSet { get; set; }
        public IDbSet<ClassifierParams> ClassifierParamsSet { get; set; }
        public IDbSet<BayesClassifierTest> BayesClassifierTestSet { get; set; }
    }
}
