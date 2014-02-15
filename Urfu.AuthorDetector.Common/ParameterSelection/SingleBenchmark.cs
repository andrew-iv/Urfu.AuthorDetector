using System;
using System.Collections.Generic;
using System.Linq;
using Accord;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class BinaryBayesStatisticsMaker : BenchmarkBase
    {
        private readonly ICommonMetricProvider[] _providers;
        private double _falseAuthorProbability = 0.75d;
        private int _changeAuthorCount = 3;


        public int ChangeAuthorCount
        {
            get { return _changeAuthorCount; }
            set { _changeAuthorCount = value; }
        }

        public BinaryBayesStatisticsMaker(IDictionary<Author, string[]> authors, params ICommonMetricProvider[] providers)
            : base(authors)
        {
            _providers = providers;
        }


        private BinaryBayesStatistics CreateClassifier( out Dictionary<Author, string[]> testCorpus, out Author selectedAuthor)
        {
            var ranSubArr = OnlyCanCheck.RandomSubArray(AuthorsCount, _random);
            testCorpus = new Dictionary<Author, string[]>();
            var learnCorpus = new Dictionary<Author, IEnumerable<string>>();

            foreach (var kvp in ranSubArr)
            {
                int skip = _random.Next((kvp.Value.Length - MessageCount - LearningCount) / 3 + 1);
                learnCorpus[kvp.Key] = kvp.Value.Skip(skip).Take(LearningCount).ToArray();
                int skipAfter = skip + LearningCount;
                testCorpus[kvp.Key] =
                    kvp.Value.Skip(skipAfter + _random.Next(kvp.Value.Length - skipAfter - MessageCount + 1))
                       .Take(MessageCount)
                       .ToArray();
            }
            var authors = learnCorpus.ToArray();
            var randAuthor = authors[_random.Next(authors.Length - 1)];
            selectedAuthor = randAuthor.Key;
            return 
                    new BinaryBayesStatistics(randAuthor.Value,
                        authors.Where(x => x.Key != randAuthor.Key).SelectMany(x => x.Value), providers: _providers);
                
        }

        private IEnumerable<KeyValuePair<Author, string[]>> HaveAnothMessagesToCheck
        {
            get { return _authors.Where(x => x.Value.Length > MessageCount); }
        }


        public void Score( int seed = 0)
        {
            lock (this)
            {
                try
                {
                    var falseCount = (int) (TestsInRoundCount*FalseAuthorProbability);
                    int successFalse = 0;
                    int successTrue = 0;
                    _random = new Random(seed);
                    for (int i = 0; i < RoundCount; i++)
                    {
                        Dictionary<Author, string[]> testCorpus;
                        Author author;
                        var cls = CreateClassifier(out testCorpus, out author);
                        var testCorpusAsArray = HaveAnothMessagesToCheck.ToArray();
                        KeyValuePair<Author, string[]> randAuthor;
                        for (int j = 0; j < TestsInRoundCount; j++)
                        {
                            bool isMy = j >= falseCount;
                            if (isMy)
                            {
                                randAuthor = testCorpusAsArray.First(x => x.Key == author);
                            }
                            else
                            {
                                do
                                {
                                    randAuthor = testCorpusAsArray[_random.Next(testCorpusAsArray.Length)];
                                } while (randAuthor.Key == author);
                            }
                            var skip = _random.Next(randAuthor.Value.Length - MessageCount + 1);
                            cls.LogResult(isMy, randAuthor.Value.Skip(skip).Take(MessageCount).ToArray());
                        }



                    }
                }
                catch
                {
                }
            }
        }

        public double FalseAuthorProbability
        {
            get { return _falseAuthorProbability; }
            set { _falseAuthorProbability = value; }
        }
    }


    public class SingleBenchmark : BenchmarkBase, IBinaryClassifierBenchmark
    {
        private double _falseAuthorProbability = 0.75d;
        private int _changeAuthorCount = 3;


        public int ChangeAuthorCount
        {
            get { return _changeAuthorCount; }
            set { _changeAuthorCount = value; }
        }

        public SingleBenchmark(IDictionary<Author, string[]> authors)
            : base(authors)
        {
        }




        private IEnumerable<IBinaryClassifier> CreateClassifiers(IBinaryClassifierFactory factory, out Dictionary<Author, IEnumerable<string>> testCorpus, int size)
        {
            var learnCorpus = GetLearnCorpus<IEnumerable<string>>(out testCorpus);
            var authors = learnCorpus.Keys.ToArray();
            return
                Enumerable.Range(0, size).Select(i =>
                factory.Create(learnCorpus, authors[_random.Next(authors.Length - 1)]));
        }

        private IEnumerable<KeyValuePair<Author, string[]>> HaveAnothMessagesToCheck
        {
            get { return _authors.Where(x => x.Value.Length > MessageCount); }
        }


        public Tuple<double, double> Score(IBinaryClassifierFactory factory, int seed = 0)
        {
            lock (this)
            {
                try
                {
                    var falseCount = (int)(TestsInRoundCount * FalseAuthorProbability);
                    int successFalse = 0;
                    int successTrue = 0;
                    _random = new Random(seed);
                    for (int i = 0; i < RoundCount; i++)
                    {
                        Dictionary<Author, IEnumerable<string>> testCorpus;
                        foreach (var cls in
                        CreateClassifiers(factory, out testCorpus, ChangeAuthorCount))
                        {
                            var haveAnothMessagesToCheckArray = HaveAnothMessagesToCheck.ToArray();
                            for (int j = 0; j < falseCount; j++)
                            {
                                KeyValuePair<Author, string[]> randAuthor;
                                do
                                {
                                    randAuthor = haveAnothMessagesToCheckArray[_random.Next(haveAnothMessagesToCheckArray.Length)];
                                } while (randAuthor.Key == cls.Author);
                                var skip = _random.Next(randAuthor.Value.Count() - MessageCount + 1);
                                if (!
                                    cls.Confirm(randAuthor.Value.Skip(skip).Take(MessageCount).ToArray())
                                    ) successFalse++;
                            }
                            var selectedAuthor = testCorpus.First(x => x.Key == cls.Author);
                            for (int j = 0; j < TestsInRoundCount - falseCount; j++)
                            {
                                var skip = _random.Next(selectedAuthor.Value.Count() - MessageCount + 1);
                                if (
                                    cls.Confirm(selectedAuthor.Value.Skip(skip).Take(MessageCount).ToArray())
                                    ) successTrue++;
                            }
                        }
                    }
                    return new Tuple<double, double>(
                        successTrue / ((double)(TestsInRoundCount - falseCount) * ChangeAuthorCount*RoundCount),
                        successFalse / ((double)falseCount * ChangeAuthorCount * RoundCount));
                }
                catch (ConvergenceException exc)
                {

                    return new Tuple<double, double>(-1d, -1d);
                }
                catch (Exception exc)
                {

                    return new Tuple<double, double>(-2d, -2d);
                }
            }
        }

        public double FalseAuthorProbability
        {
            get { return _falseAuthorProbability; }
            set { _falseAuthorProbability = value; }
        }
    }
}