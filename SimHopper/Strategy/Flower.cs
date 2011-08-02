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
        public float EarliHopFactor { get; set; }
        public float PPLNSFactor { get; set; }
        public float ScoreFactor { get; set; }
        public float Threshold { get; set; }

        private MersenneTwister _rnd;
        
        private Dictionary<string, float> _slicedShare;
        private Dictionary<string, int> _slice;

        public Flower(int difficulty)
        {
            SliceSize = 1200;           // default
            EarliHopFactor = 4.0f;          // default
            PPLNSFactor = 4.0f;          // default
            ScoreFactor = 4.0f;          // default
            Threshold = 0.43f;           // default

            _difficulty = difficulty;
            _rnd = new MersenneTwister((uint)DateTime.Now.Ticks);
            _slicedShare = new Dictionary<string, float>();
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
                totalWeight += Math.Pow(w, 2.6);
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

                    var w = Math.Pow(1.0 / modShare, 2.6) * weight;
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

            foreach (var i in _slice)
            {
                if(i.Value > 0 && pools[i.Key].Type == PoolType.Prop)
                {
                    best = i.Key;
                    break;
                }
            }

            if (best == null)
            {
                foreach (var i in _slice)
                {
                    if (i.Value > 0 && pools[i.Key].Type == PoolType.PropEarlyHop)
                    {
                        best = i.Key;
                        break;
                    }
                }
            }

            if (best == null)
            {
                foreach (var i in _slice)
                {
                    if (i.Value > 0 && pools[i.Key].Type == PoolType.Pplns)
                    {
                        best = i.Key;
                        break;
                    }
                }
            }

            if (best == null)
            {
                foreach (var i in _slice)
                {
                    if (i.Value > 0 && pools[i.Key].Type == PoolType.Score)
                    {
                        best = i.Key;
                        break;
                    }
                }
            }

            return best ?? ("smpps");
        }
    }
}