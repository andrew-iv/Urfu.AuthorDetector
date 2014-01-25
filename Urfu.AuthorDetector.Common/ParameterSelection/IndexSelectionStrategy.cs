using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class IndexSelectionStrategy
    {
        public enum CollectionChangedType
        {
            Add,
            Del
        }

        public class CollectionChanged
        {
            public CollectionChangedType Type{get; internal set; }
            public int Index {get; internal set; }
            public string Name {get; internal set; }
        }

        private TimeSpan _timeOut = new TimeSpan(0,5,0);
        private IClassifierBenchmark _benchmark;
        private bool _backSteps=true;

        public bool BackSteps
        {
            get { return _backSteps; }
            set
            {
                lock (this)
                {
                    _backSteps = value;
                }
            }
        }

        public TimeSpan TimeOut
        {
            get { return _timeOut; }
            set
            {
                lock (this)
                {
                    _timeOut = value;
                }
            }
        }

        public IClassifierBenchmark Benchmark
        {
            get { return _benchmark; }
            set
            {
                lock (this)
                {
                    _benchmark = value;
                }
            }
        }


        private void CheckParams()
        {
            if (Benchmark == null) throw new InvalidOperationException("установите Benchmark");
            if (TimeOut <= new TimeSpan()) throw new InvalidOperationException("TimeOut должен быть положительным");
        }

        public event EventHandler<CollectionChanged> ArgAdded = delegate{  };




        public int[] SelectMetric(IPostMetricProviderStrategyProxy factory)
        {
            CheckParams();
            int[] best= new int[]{};
            lock (this)
            {
                var start = DateTime.Now;
                factory.Start();
                var yetUsed = new HashSet<int>();
                double maxScore = 0d;
                int maxInd = 0;
                while (DateTime.Now < start + TimeOut)
                {
                    var locMax = 0d;   
                    bool enlarged = false;
                    foreach (var ind in Enumerable.Range(0, factory.Size).Where(x => !yetUsed.Contains(x)))
                    {
                        factory.SetIndexes(yetUsed.Union(new[] {ind}).OrderBy(x => x).ToArray());
                        var score = Benchmark.Score(factory.Factory);
                        if (score > locMax)
                        {
                            maxInd = ind;
                            locMax = score;
                            if (locMax > maxScore)
                            {
                                maxScore = score;
                                enlarged = true;
                            }
                        }
                    }
                    if (enlarged)
                    {
                        best = yetUsed.ToArray();
                    }
                    yetUsed.Add(maxInd);
                    
                    ArgAdded.Invoke(this,new CollectionChanged()
                        {
                            Index = maxInd,
                            Type = CollectionChangedType.Add,
                            Name = factory.Names.ElementAt(maxInd)
                        
                        });
                    if (yetUsed.Count == factory.Size) break;

                }
                factory.End();

                return best.OrderBy(x => x).ToArray();
            }
            

            
        }
    }
}