using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class BenchmarkBase : IBenchmark
    {
        protected readonly IDictionary<Author, string[]> _authors;
        private int _messageCount = 10;
        private int _learningCount = 300;
        private int _roundCount = 10;
        private int _testsInRoundCount = 50;
        private int _authorsCount = 10;
        public int AuthorsCount
        {
            get
            {
                lock (this)
                {
                    return _authorsCount;
                }
            }
            set { _authorsCount = value; }
        }

        public double FalseAuthorProbability { get; set; }


        protected Random _random = new Random();

        public BenchmarkBase(IDictionary<Author,string[]> authors)
        {
            _authors = authors;
        }

        public int LearningCount
        {
            get
            {
                lock (this)
                {
                    return _learningCount;
                }
            }
            set { _learningCount = value; }
        }

        public int MessageCount
        {   
            get
            {
                lock (this)
                {
                    return _messageCount;
                }
            }
            set { _messageCount = value; }
        }

        public int RoundCount
        {
            get
            {
                lock (this)
                {
                    return _roundCount;
                }
            }
            set { _roundCount = value; }
        }

        public int TestsInRoundCount
        {
            get
            {
                lock (this)
                {
                    return _testsInRoundCount;
                }
            }
            set { _testsInRoundCount = value; }
        }

        protected IEnumerable<KeyValuePair<Author,string[]>> OnlyCanCheck
        {
            get { return _authors.Where(x => x.Value.Length >= MessageCount + LearningCount); }
        }

        protected Dictionary<Author, TEnumerable> GetLearnCorpus<TEnumerable>(out Dictionary<Author, TEnumerable> testCorpus) where TEnumerable : class, IEnumerable<string>
        {
            var ranSubArr = OnlyCanCheck.RandomSubArray(AuthorsCount, _random);
            testCorpus = new Dictionary<Author, TEnumerable>();
            var learnCorpus = new Dictionary<Author, TEnumerable>();

            foreach (var kvp in ranSubArr)
            {
                int skip = _random.Next((kvp.Value.Length - MessageCount - LearningCount) / 3 + 1);
                learnCorpus[kvp.Key] = kvp.Value.Skip(skip).Take(LearningCount).ToArray() as TEnumerable;
                int skipAfter = skip + LearningCount;
                testCorpus[kvp.Key] =
                    kvp.Value.Skip(skipAfter)
                       .ToArray() as TEnumerable;
            }
            return learnCorpus;
        }
    }
}