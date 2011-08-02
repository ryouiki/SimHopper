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
        public float PPLNSFactor { get; set; }
        public float ScoreFactor { get; set; }

        private MersenneTwister _rnd;
        
        private Dictionary<string, float> _slicedShare;
        private Dictionary<string, int> _slice;

        public Flower(int difficulty)
        {
            SliceSize = 1200;           // default
            PPLNSFactor = 4.0f;          // default
            ScoreFactor = 4.0f;          // default
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
                var modShare = pool.Value.CurrentShare + 1;
                switch (pool.Value.Type)
                {
                    case PoolType.Pplns:
                        modShare *= PPLNSFactor;
                        break;
                    case PoolType.Score:
                        modShare *= ScoreFactor;
                        break;
                }

                if (pool.Value.Type == PoolType.Smpps || modShare > _difficulty*0.43)
                {
                    continue;
                }
                
                _slicedShare.Add(pool.Key, pool.Value.CurrentShare);
                
                var w = 1.0 / modShare;
                totalWeight += Math.Pow(w, 2.6);
            }

            if (totalWeight>0)
            {
                var weight = SliceSize / totalWeight;
                foreach (var pool in pools)
                {
                    var modShare = pool.Value.CurrentShare + 1;
                    switch (pool.Value.Type)
                    {
                        case PoolType.Pplns:
                            modShare *= PPLNSFactor;
                            break;
                        case PoolType.Score:
                            modShare *= ScoreFactor;
                            break;
                    }

                    if (pool.Value.Type == PoolType.Smpps || modShare > _difficulty * 0.43)
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
                bool toReslice = false;
                foreach (var s in _slicedShare)
                {
                    var modShare = pools[s.Key].CurrentShare + 1;
                    switch(pools[s.Key].Type)
                    {
                        case PoolType.Pplns:
                            modShare *= PPLNSFactor;
                            break;
                        case PoolType.Score:
                            modShare *= ScoreFactor;
                            break;
                    }
                    if (s.Value > modShare)
                    {
                        toReslice = true;
                        break;
                    }
                }
                if (toReslice) Reslice(pools);
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