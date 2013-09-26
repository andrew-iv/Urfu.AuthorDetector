﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetecor.Experiment
{
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

        public Experimentator(IKernel kernel, Func<IDictionary<Author, IEnumerable<Post>>, IClassifier> constructor)
        {
            _kernel = kernel;
            var filter = kernel.Get<IPostsQueryFilter>();

            _context = kernel.Get<IStatisticsContext>();
            _experiment = kernel.Get<IExperiment>();
            IDictionary<Author, IEnumerable<Post>> posts = filter.FilterDate(
                filter.TopAuthorsMinimum(
                    _context.Posts.Where(x => x.Author.Forum.Id == _experiment.ForumId
                        )
                    ).Take(_experiment.TopAuthors).SelectMany(x => x.Author.Post)
                , _experiment.StartGeneral, _experiment.EndGeneral).GroupBy(x => x.Author).ToDictionary(x => x.Key, x => x.ToArray() as IEnumerable<Post>);
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


            var res = _classifier.ClassificatePosts(possibleOffset <= 0
                                                        ? newPosts.ToList()
                                                        : newPosts.OrderBy(x => x.Id).Skip(rand.Next(possibleOffset)).Take(exp.PostsCount).ToList()
                );
            return new Result(res,need);
        }
    }
}