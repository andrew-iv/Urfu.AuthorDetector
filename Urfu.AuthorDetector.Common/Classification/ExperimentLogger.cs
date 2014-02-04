using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Urfu.AuthorDetector.Common.ParameterSelection;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
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