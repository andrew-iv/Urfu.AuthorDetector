using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    public interface IBayesStatisticsProvider
    {
        double GetFalseErrorLevel(double errorLevel, int messageCount);
        bool IsReliable();
    }


    public interface IBayesResultLogger
    {
        
        void Log(BayesClassifierTest result);
    }

    public class BayesResultLogger : IBayesResultLogger
    {
        private readonly IStatisticsContext _context;
        private readonly IClassifierBenchmark _benchmark;

        [Named("BayesClsVersionId")]
        [Inject]
        public int VersionId { get; set; }

        public BayesResultLogger(IStatisticsContext context)
        {
            _context = context;
        }

        public void Log(BayesClassifierTest result)
        {
            result.ClassifierVersion = _context.ClassifierVersionSet.FirstOrDefault(x => x.Id == VersionId);
            _context.BayesClassifierTestSet.Add(
                result
                );
            _context.SaveChanges();
        }
    }

    /// <summary>
    /// Заглушка
    /// </summary>
    public class DummyBayesResultLogger : IBayesResultLogger
    {
        public void Log(BayesClassifierTest result)
        {
            
        }
    }

    public class ExperimentLogger : IExperimentLogger
    {
        private readonly IStatisticsContext _context;
        private readonly IClassifierBenchmark _benchmark;

        [Named("ClsVersionId")]
        [Inject]
        public int VersionId { get; set; }

        public ExperimentLogger(IStatisticsContext context, IClassifierBenchmark benchmark)
        {
            _context = context;
            _benchmark = benchmark;
        }

        public void LogBenchmark(IClassifierFactory factory, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _context.ClassifierResultSet.Add(
                new ClassifierResult()
                    {
                        ClassifierVersion = _context.ClassifierVersionSet.FirstOrDefault(x => x.Id == VersionId),
                        MessageCount = _benchmark.MessageCount,
                        RoundCount = _benchmark.RoundCount,
                        Result = _benchmark.Score(factory),
                        DateTime = DateTime.Now,
                        LearningCount = _benchmark.LearningCount,
                        TestsPerRound = _benchmark.TestsInRoundCount,
                        ClassifierParams = parameters.Select(x => new ClassifierParams()
                            {
                                Key = x.Key,
                                Value = x.Value
                            }).ToList()
                    });
            _context.SaveChanges();
        }
    }
}