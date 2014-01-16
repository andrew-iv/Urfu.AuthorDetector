using System;
using System.Collections.Generic;
using System.Linq;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public interface IClassifierBenchmark
    {
        /// <summary>
        /// Сколько авторов тестировать
        /// </summary>
        int AuthorsCount { get; set; }

        /// <summary>
        /// Сколько постов тестировать
        /// </summary>
        int MessageCount { get; set; }

        /// <summary>
        /// Количество переобучений
        /// </summary>
        int RoundCount { get; set; }

        /// <summary>
        /// Количество тестов одного и тогоже классификатора
        /// </summary>
        int TestsInRoundCount { get; set; }
        double Score(IClassifierFactory factory);
    }

    public class ForumClassifierBenchmark : IClassifierBenchmark
    {
        private readonly Dictionary<Author, string[]> _authors;
        private DateTime _startLearning = new DateTime(2012,1,1);
        private DateTime _endLearning = new DateTime(2013,3,1);
        private int _authorsCount = 10;
        private int _messageCount = 10;
        private int _learningCount = 300;
        private int _roundCount = 100;
        private int _testsInRoundCount = 10;
        private Random _random = new Random();

        public ForumClassifierBenchmark(Dictionary<Author,string[]> authors)
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


        private IEnumerable<KeyValuePair<Author,string[]>> OnlyCanCheck
        {
            get { return _authors.Where(x => x.Value.Length >= MessageCount + LearningCount); }
        }


        private IClassifier CreateClassifier(IClassifierFactory factory, out Dictionary<Author,string[]> testCorpus)
        {
            var ranSubArr = OnlyCanCheck.RandomSubArray(AuthorsCount, _random);
            testCorpus = new Dictionary<Author, string[]>();
            var learnCorpus = new Dictionary<Author, IEnumerable<string>>();

            foreach (var kvp in ranSubArr)
            {
                int skip = _random.Next((kvp.Value.Length - MessageCount + LearningCount)/2);
                learnCorpus[kvp.Key] = kvp.Value.Skip(skip).Take(LearningCount).ToArray();
                int skipAfter = skip + LearningCount;
                testCorpus[kvp.Key] =
                    kvp.Value.Skip(skipAfter + _random.Next(kvp.Value.Length - skipAfter - MessageCount))
                       .Take(MessageCount)
                       .ToArray();
            }

            return factory.Create(learnCorpus);
        }

        public double Score(IClassifierFactory factory)
        {
            lock (this)
            {
                int success = 0;
                _random = new Random(0);
                for (int i = 0; i < RoundCount; i++)
                {
                    Dictionary<Author, string[]> testCorpus;
                    var cls = CreateClassifier(factory, out testCorpus);
                    var testCorpusAsArray = testCorpus.ToArray();
                    for (int j = 0; j < TestsInRoundCount; j++)
                    {
                        var randAuthor = testCorpusAsArray[_random.Next(testCorpusAsArray.Length)];
                        if (cls.ClassificatePosts(randAuthor.Value) == randAuthor.Key) success++;
                    }
                }
                return ((double) success)/TestsInRoundCount/RoundCount;
            }
        }
    }
}