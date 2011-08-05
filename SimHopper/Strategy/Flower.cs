using System;
using System.Collections.Generic;
using System.Linq;
using MTRand;

namespace SimHopper
{
    public class Flower : IHopStrategy
    {
        private readonly int _difficulty;

        public double SliceSize { get; set; }
        public double EarliHopFactor { get; set; }
        public double PPLNSFactor { get; set; }
        public double ScoreFactor { get; set; }
        public double Threshold { get; set; }
        public double PowerValue { get; set; }
        public int SelectType { get; set; }         // 0 : first one    1 : biggest one     2 : smallest one    3 : fastest pool

        private MersenneTwister _rnd;

        private Dictionary<string, double> _slicedShare;
        private Dictionary<string, int> _slice;

        public Flower(int difficulty)
        {
            // Default values
            SliceSize = 1200;
            EarliHopFactor = 4.0f;
            PPLNSFactor = 4.0f;
            ScoreFactor = 4.0f;
            Threshold = 0.43f;
            PowerValue = 2.6f;
            SelectType = 0;

            _difficulty = difficulty;
            _rnd = new MersenneTwister((uint)DateTime.Now.Ticks);
            _slicedShare = new Dictionary<string, double>();
            _slice = new Dictionary<string, int>();
        }
        
        public void Reslice(Dictionary<string, PoolServer> pools)
        {
            _slicedShare.Clear();
            _slice.Clear();

            double totalWeight = 0;
            foreach (var pool in pools)
            {
                if (pool.Value.Type == PoolType.Smpps)
                {
                    continue;
                }

                _slicedShare.Add(pool.Key, pool.Value.CurrentShare);

                var modShare = pool.Value.CurrentShare + 1;
                switch (pool.Value.Type)
                {
                    case PoolType.PropEarlyHop:
                        modShare *= EarliHopFactor;
                        break;
                    case PoolType.Pplns:
                        modShare *= PPLNSFactor;
                        break;
                    case PoolType.Score:
                        modShare *= ScoreFactor;
                        break;
                }

                if (modShare > _difficulty * Threshold)
                {
                    continue;
                }

                var w = 1.0 / modShare;
                totalWeight += Math.Pow(w, PowerValue);
            }

            if (totalWeight>0)
            {
                var weight = SliceSize / totalWeight;
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.Smpps)
                    {
                        continue;
                    }

                    var modShare = pool.Value.CurrentShare + 1;
                    switch (pool.Value.Type)
                    {
                        case PoolType.PropEarlyHop:
                            modShare *= EarliHopFactor;
                            break;
                        case PoolType.Pplns:
                            modShare *= PPLNSFactor;
                            break;
                        case PoolType.Score:
                            modShare *= ScoreFactor;
                            break;
                    }

                    if (modShare > _difficulty * Threshold)
                    {
                        continue;
                    }

                    var w = Math.Pow(1.0 / modShare, PowerValue) * weight;
                    w = w < 60.0 ? -1 : w;
                    _slice.Add(pool.Key, (int)w);
                }
            }            
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            if (_slice.ContainsKey(currentPool))
            {
                _slice[currentPool] = _slice[currentPool] - advancedSeconds;
            }

            if (_slicedShare.Count == 0 || _slice.Count == 0 || _slice.Where(s => s.Value > 0).Count() == 0)
            {
                Reslice(pools);
            }
            else
            {
                bool toReslice = _slicedShare.Any(s => s.Value > pools[s.Key].CurrentShare);
                if (toReslice)
                {
                    Reslice(pools);
                }
            }

            var maxVal = 0;
            var minVal = Math.Pow(10, 10);
            foreach (var i in _slice.Where(s => pools[s.Key].Type == PoolType.Prop && s.Value > 0))
            {
                if (SelectType == 0)
                {
                    best = i.Key;
                    break;                    
                }
                else if (SelectType == 1 && i.Value > maxVal)   // select biggest one >> the best
                {
                    maxVal = i.Value;
                    best = i.Key;
                }
                else if (SelectType == 2 && i.Value < minVal)   // select smallest one
                {
                    minVal = i.Value;
                    best = i.Key;
                }
                else if (SelectType == 3 && pools[i.Key].GHashSpeed > maxVal)   // select fastest pool
                {
                    maxVal = pools[i.Key].GHashSpeed;
                    best = i.Key;
                }
                else if (SelectType == 4 && pools[i.Key].RoundTime < minVal)   // select shortest round time
                {
                    minVal = pools[i.Key].RoundTime;
                    best = i.Key;
                }
            }

            if (best == null)
            {
                foreach (var i in _slice.Where(s => pools[s.Key].Type == PoolType.PropEarlyHop && s.Value > 0))
                {
                    if (SelectType == 0)
                    {
                        best = i.Key;
                        break;
                    }
                    else if (SelectType == 1 && i.Value > maxVal)   // select biggest one
                    {
                        maxVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 2 && i.Value < minVal)   // select smallest one
                    {
                        minVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 3 && pools[i.Key].GHashSpeed > maxVal)   // select fastest pool
                    {
                        maxVal = pools[i.Key].GHashSpeed;
                        best = i.Key;
                    }
                    else if (SelectType == 4 && pools[i.Key].RoundTime < minVal)   // select shortest round time
                    {
                        minVal = pools[i.Key].RoundTime;
                        best = i.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var i in _slice.Where(s => pools[s.Key].Type == PoolType.Pplns && s.Value > 0))
                {
                    if (SelectType == 0)
                    {
                        best = i.Key;
                        break;
                    }
                    else if (SelectType == 1 && i.Value > maxVal)   // select biggest one
                    {
                        maxVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 2 && i.Value < minVal)   // select smallest one
                    {
                        minVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 3 && pools[i.Key].GHashSpeed > maxVal)   // select fastest pool
                    {
                        maxVal = pools[i.Key].GHashSpeed;
                        best = i.Key;
                    }
                    else if (SelectType == 4 && pools[i.Key].RoundTime < minVal)   // select shortest round time
                    {
                        minVal = pools[i.Key].RoundTime;
                        best = i.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var i in _slice.Where(s => pools[s.Key].Type == PoolType.Pplns && s.Value > 0))
                {
                    if (SelectType == 0)
                    {
                        best = i.Key;
                        break;
                    }
                    else if (SelectType == 1 && i.Value > maxVal)   // select biggest one
                    {
                        maxVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 2 && i.Value < minVal)   // select smallest one
                    {
                        minVal = i.Value;
                        best = i.Key;
                    }
                    else if (SelectType == 3 && pools[i.Key].GHashSpeed > maxVal)   // select fastest pool
                    {
                        maxVal = pools[i.Key].GHashSpeed;
                        best = i.Key;
                    }
                    else if (SelectType == 4 && pools[i.Key].RoundTime < minVal)   // select shortest round time
                    {
                        minVal = pools[i.Key].RoundTime;
                        best = i.Key;
                    }
                }
            }

            return best ?? ("smpps");
        }
    }
}