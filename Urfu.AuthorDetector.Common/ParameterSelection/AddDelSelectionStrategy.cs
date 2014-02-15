using System;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.AuthorDetector.Common.ParameterSelection
{
    public class AddDelSelectionStrategy : SelectionStrategyBase
    {


        private bool _backSteps = true;

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


        public int DeltaAdd { get; set; }
        public int DeltaDel { get; set; }



        public override event EventHandler<SelectionCollectionChanged> ArgAdded = delegate { };
        public event EventHandler<SelectionCollectionChanged> ArgDel = delegate { };


        public int[] SelectMetric(IPostMetricProviderStrategyProxy proxy, int[] startFrom = null)
        {
            CheckParams();
            
            lock (this)
            {
                int[] best = startFrom ?? new int[] { };
                var start = DateTime.Now;
                proxy.Start();
                var yetUsed = new HashSet<int>(startFrom ?? new int[] { });
                double maxScore = 0d;

                while (DateTime.Now < start + TimeOut)
                {

                    //добавление
                    while (yetUsed.Count < DeltaAdd)
                    {
                        int maxInd = 0;
                        var locMax = 0d;
                        var enlarged = false;
                        foreach (var ind in Enumerable.Range(0, proxy.Size).Where(x => !yetUsed.Contains(x)))
                        {
                            proxy.SetIndexes(yetUsed.Union(new[] { ind }).OrderBy(x => x).ToArray());
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
                        yetUsed.Add(maxInd);
                        if (enlarged)
                        {
                            best = yetUsed.ToArray();
                        }
                        ArgAdded.Invoke(this, new SelectionCollectionChanged()
                            {
                                Index = maxInd,
                                Type = SelectionCollectionChangedType.Add,
                                Name = proxy.Names.ElementAt(maxInd)

                            });
                    }

                    //удаление
                    while (yetUsed.Count > DeltaDel)
                    {
                        int maxInd = 0;
                        var locMax = 0d;
                        var enlarged = false;
                        foreach (var ind in Enumerable.Range(0, proxy.Size).Where(yetUsed.Contains))
                        {
                            proxy.SetIndexes(yetUsed.Where(x=>x!= ind).OrderBy(x => x).ToArray());
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
                        yetUsed.Remove(maxInd);
                        if (enlarged)
                        {
                            best = yetUsed.ToArray();
                        }
                        ArgDel.Invoke(this, new SelectionCollectionChanged()
                        {
                            Index = maxInd,
                            Type = SelectionCollectionChangedType.Add,
                            Name = proxy.Names.ElementAt(maxInd)

                        });
                    }
                }

                proxy.End();
                return best.OrderBy(x => x).ToArray();
            }
        }
    }
}