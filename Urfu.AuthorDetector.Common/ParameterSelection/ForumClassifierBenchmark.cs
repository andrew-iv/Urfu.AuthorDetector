using System;
using System.Collections.Generic;
using System.Linq;
using Accord;
using Urfu.AuthorDetector.Common.Classification;
using Urfu.AuthorDetector.DataLayer;
using Urfu.Utils;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class MultyBenchmark : BenchmarkBase, IClassifierBenchmark
    {


        public MultyBenchmark(IDictionary<Author, string[]> authors)
            : base(authors)
        {
        }




        private IClassifier CreateClassifier(IClassifierFactory factory, out Dictionary<Author, IEnumerable<string>> testCorpus)
        {
            Dictionary<Author, IEnumerable<string>> learnCorpus;
            testCorpus = GetLearnCorpus<IEnumerable<string>>(out learnCorpus);
            return factory.Create(learnCorpus);
        }

        public double Score(IClassifierFactory factory, int seed = 0)
        {
            double reliebleFalse;
            double reliebleTrue;
            return Score(factory, out reliebleFalse, out reliebleTrue, seed);
        }

        public double ScoreTopN(IClassifierFactory factory, int topN, int seed = 0)
        {
            lock (this)
            {
                
                try
                {
                    int total = TestsInRoundCount * RoundCount;
                    int success = 0;
                    _random = new Random(seed);
                    for (int i = 0; i < RoundCount; i++)
                    {
                        Dictionary<Author, IEnumerable<string>> testCorpus;
                        var cls = CreateClassifier(factory, out testCorpus);
                        var testCorpusAsArray = testCorpus.ToArray();
                        for (int j = 0; j < TestsInRoundCount; j++)
                        {
                            var randAuthor = testCorpusAsArray[_random.Next(testCorpusAsArray.Length)];
                            var skip = _random.Next(randAuthor.Value.Count() - MessageCount + 1);
                            if (cls.ClassificatePosts(randAuthor.Value.Skip(skip).Take(MessageCount).ToArray(),topN).Any(x=>x==
                                                                                                                            randAuthor.Key))
                                success++;
                        }
                    }
                    return ((double)success) / TestsInRoundCount / RoundCount;
                }
                catch (ConvergenceException exc)
                {

                    return -1d;
                }
                catch (Exception exc)
                {

                    return -2d;
                }
            }
        }

        public double Score(IClassifierFactory factory, out double reliableFalse, out double reliableTrue, int seed = 0)
        {
            lock (this)
            {
                reliableFalse = 0d;
                reliableTrue = 0d;
                try
                {
                    int total = TestsInRoundCount * RoundCount;
                    int success = 0;
                    _random = new Random(seed);
                    for (int i = 0; i < RoundCount; i++)
                    {
                        Dictionary<Author, IEnumerable<string>> testCorpus;
                        var cls = CreateClassifier(factory, out testCorpus);
                        var testCorpusAsArray = testCorpus.ToArray();
                        for (int j = 0; j < TestsInRoundCount; j++)
                        {
                            var randAuthor = testCorpusAsArray[_random.Next(testCorpusAsArray.Length)];
                            var skip = _random.Next(randAuthor.Value.Count() - MessageCount + 1);
                            bool reliable;
                            bool isSuccess =
                                cls.ClassificatePosts(randAuthor.Value.Skip(skip).Take(MessageCount).ToArray(), out reliable) ==
                                randAuthor.Key;
                            if (isSuccess)
                            {
                                success++;
                                if (reliable)
                                    reliableTrue += 1d;
                            }
                            if (reliable)
                                reliableFalse += 1d;
                            cls.LogResult(isSuccess);
                        }
                    }
                    reliableTrue = success == 0 ? 0d : reliableTrue / total / success;
                    reliableFalse = success == total ? 0d : reliableTrue / total / (1 - success);
                    return ((double)success) / TestsInRoundCount / RoundCount;
                }
                catch (ConvergenceException exc)
                {

                    return -1d;
                }
                catch (Exception exc)
                {

                    return -2d;
                }
            }
        }
    }
}