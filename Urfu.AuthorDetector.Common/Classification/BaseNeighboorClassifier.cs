using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{

    public static class GrammsHelper
    {
        public static string[] GetTopGrammas(this IEnumerable<Post> posts, int grammLength = 3, int useFirst = 100)
        {
            var gramms = new Dictionary<string, int>();

            foreach (var post in posts)
            {
                var metric = new TrivialMetric(post);
                foreach (var gramm in metric.NGramms(grammLength))
                {
                    if (gramms.ContainsKey(gramm))
                    {
                        gramms[gramm]++;
                    }
                    else
                    {
                        gramms[gramm] = 1;
                    }

                }
            }

            return gramms.OrderByDescending(x => x.Value).Take(useFirst).Select(x => x.Key).ToArray();
        }
    }


    public abstract class BaseNeighboorClassifier : IClassifier
    {
        private readonly IDictionary<Author, IMetric> _authorMetrics;
        private IEnumerable<IMetric> _allMetrics;
        private IEnumerable<KeyValuePair<string, double>> _variance;
        

        private bool FilterPosts(Post post)
        {
            return post.Text.Contains("<p>");
        }

        protected virtual void AdditionalInitialization(IDictionary<Author, IEnumerable<Post>> authors)
        {
            
        }

        protected BaseNeighboorClassifier(IDictionary<Author, IEnumerable<Post>> authors)
        {
            AdditionalInitialization(authors);
            _allMetrics = authors.SelectMany(x => x.Value.Where(FilterPosts).Select(CallculateMetric)).ToArray();
            _variance = _allMetrics.CalculateVariance().ToArray();
            Authors = authors.Keys;
            _authorMetrics = new Dictionary<Author, IMetric>();
            foreach (var author in authors)
            {
                _authorMetrics.Add(author.Key, CallculateMetrics(author.Value.Where(FilterPosts)));
            }


        }

        protected abstract IMetric CallculateMetric(Post post);


        protected virtual IMetric CallculateMetrics(IEnumerable<Post> posts)
        {
            return
                new DictionaryMetric(
                    new DictionaryMetric(
                     posts.Select(CallculateMetric).CalculateAverage()).Standardizate(_variance));
        }

        protected virtual double CompareMetrics(IMetric a, IMetric b)
        {
            const double keyNotFoundPenalty = 100d;
            double res = 0d;
            return Math.Sqrt(b.MetricValues.OrderBy(x => x.Key).Zip
                (a.MetricValues.OrderBy(x=>x.Key), (aKvp, bKvp) =>
                    {
                        var diff = aKvp.Value - bKvp.Value;
                        return diff*diff;
                    }).Sum());
        }

        public IEnumerable<Author> Authors { get; private set; }
        public Author ClassificatePosts(IEnumerable<Post> posts)
        {
            var metric = CallculateMetrics(posts.Where(FilterPosts));
            var etalons = _authorMetrics.OrderBy(x => CompareMetrics(x.Value, metric)).Select(x => x.Key);
            return etalons.First();
        }

        public virtual string Description
        {
            get { return "Метод ближайших соседей"; }
        }
        public virtual string Name { get { return "Метод ближайших соседей"; } }
    }
}