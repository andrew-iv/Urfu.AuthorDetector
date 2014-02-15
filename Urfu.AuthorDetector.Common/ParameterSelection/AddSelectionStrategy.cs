using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public abstract class SelectionStrategyBase
    {
        private TimeSpan _timeOut = new TimeSpan(0,5,0);
        private IClassifierBenchmark _benchmark;

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

        protected void CheckParams()
        {
            if (Benchmark == null) throw new InvalidOperationException("установите Benchmark");
            if (TimeOut <= new TimeSpan()) throw new InvalidOperationException("TimeOut должен быть положительным");
        }

        public virtual event EventHandler<SelectionCollectionChanged> ArgAdded = delegate { };
    }

    public class AddSelectionStrategy : SelectionStrategyBase
    {
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


        public override event EventHandler<SelectionCollectionChanged> ArgAdded = delegate{  };




        public int[] SelectMetric(IPostMetricProviderStrategyProxy proxy)
        {
            CheckParams();
            int[] best= new int[]{};
            lock (this)
            {
                var start = DateTime.Now;
                proxy.Start();
                var yetUsed = new HashSet<int>();
                double maxScore = 0d;
                int maxInd = 0;
                while (DateTime.Now < start + TimeOut)
                {
                    var locMax = 0d;   
                    bool enlarged = false;
                    foreach (var ind in Enumerable.Range(0, proxy.Size).Where(x => !yetUsed.Contains(x)))
                    {
                        proxy.SetIndexes(yetUsed.Union(new[] {ind}).OrderBy(x => x).ToArray());
                        var score = Benchmark.Score(proxy.Factory);
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
                    
                    ArgAdded.Invoke(this,new SelectionCollectionChanged()
                        {
                            Index = maxInd,
                            Type = SelectionCollectionChangedType.Add,
                            Name = proxy.Names.ElementAt(maxInd)
                        
                        });
                    if (yetUsed.Count == proxy.Size) break;

                }
                proxy.End();

                return best.OrderBy(x => x).ToArray();
            }
        }
    }

    public enum SelectionCollectionChangedType
    {
        Add,
        Del
    }
}