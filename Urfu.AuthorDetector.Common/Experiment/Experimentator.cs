﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetecor.Experiment
{
    public class DataSource
    {
        public int? ForumId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public IDictionary<Author, string[]> GetPosts(int topAuthors)
        {
            var cont = StaticVars.Kernel.Get<IStatisticsContext>();
            var de = StaticVars.Kernel.Get<IDataExtractor>();
            var authors = ForumId.HasValue?cont.Authors.Where(x=>x.Forum.Id == ForumId) : cont.Authors;


            return authors.SelectMany(x =>
                               x.Post.Where(post => !DateEnd.HasValue || post.DateTime <= DateEnd)
                                .Where(post => !DateStart.HasValue || post.DateTime >= DateStart))
                   .GroupBy(x => x.Author)
                   .OrderByDescending(x => x.Count()).Take(topAuthors)
                   .ToDictionary(x => x.Key, x => x.Select(de.GetText).ToArray());
        }
    }

    public class Experimentator
    {
        private readonly IKernel _kernel;

        public class Result
        {
            public readonly Author Returned;
            public readonly Author Actual;

            public Result(Author returned, Author actual)
            {
                Returned = returned;
                Actual = actual;
            }

            public bool IsSuccess { get { return Returned.Identity == Actual.Identity; } }
        }

        private readonly IExperiment _experiment;
        private IClassifier _classifier;
        private IStatisticsContext _context;
        private Author[] _authors;

        public Experimentator(IKernel kernel, Func<IDictionary<Author, IEnumerable<string>>, IClassifier> constructor)
        {
            _kernel = kernel;
            var filter = kernel.Get<IPostsQueryFilter>();
            var dataExtractor = kernel.Get<IDataExtractor>();

            _context = kernel.Get<IStatisticsContext>();    
            _experiment = kernel.Get<IExperiment>();
            var posts = filter.FilterDate(
                filter.TopAuthorsMinimum(
                    _context.Posts.Where(x => x.Author.Forum.Id == _experiment.ForumId
                        )
                    ).Take(_experiment.TopAuthors).SelectMany(x => x.Author.Post)
                , _experiment.StartGeneral, _experiment.EndGeneral).GroupBy(x => x.Author).ToDictionary(x => x.Key, x => x.Select(dataExtractor.GetText).ToArray() as IEnumerable<string>);
            _authors = posts.Keys.ToArray();
            _classifier = constructor(posts);
        }

        public Result Test()
        {
            var exp = _kernel.Get<IExperiment>();
            var filter = _kernel.Get<IPostsQueryFilter>();
            var rand = new Random();
            var need = _authors[rand.Next(_authors.Length)];
            var newPosts =
                filter.FilterDate(
                    _context.Posts.Where(x => x.Author.Id == need.Id), exp.EndGeneral, null);
            var allCount = newPosts.Count();
            var possibleOffset = allCount - exp.PostsCount;
            var extractor = _kernel.Get<IDataExtractor>();

            var res = _classifier.ClassificatePosts(possibleOffset <= 0
                                                        ? newPosts.Select(extractor.GetText).ToList()
                                                        : newPosts.OrderBy(x => x.Id).Skip(rand.Next(possibleOffset)).Take(exp.PostsCount).Select(extractor.GetText).ToList()
                );
            return new Result(res,need);
        }
    }
}