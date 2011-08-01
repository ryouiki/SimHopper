﻿using System;
using System.Collections.Generic;
using System.Linq;
using MTRand;

namespace SimHopper
{
    public class Flower : IHopStrategy
    {
        private readonly int _difficulty;

        public double SliceSize { get; set; }

        private MersenneTwister _rnd;
        
        private Dictionary<string, float> _slicedShare;
        private Dictionary<string, int> _slice;

        public Flower(int difficulty)
        {
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
                if(pool.Value.Type != PoolType.Prop)
                {
                    continue;
                }
                _slicedShare.Add(pool.Key, pool.Value.CurrentShare);
                var w = 1.0/(pool.Value.CurrentShare + 1);
                totalWeight += Math.Pow(w, 2.6);
            }

            var weight = SliceSize / totalWeight;
            foreach (var pool in pools)
            {
                if (pool.Value.Type != PoolType.Prop)
                {
                    continue;
                }
                var w = Math.Pow(1.0/(pool.Value.CurrentShare + 1), 2.6)*weight;
                w = w < 60.0 ? -1 : w;
                _slice.Add(pool.Key, (int)w);
            }
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            if (_slice.ContainsKey(currentPool))
            {
                _slice[currentPool] = _slice[currentPool] - advancedSeconds;
            }

            if (_slicedShare.Count==0 || _slice.Count==0)
            {
                Reslice(pools);
            }
            else
            {
                if( pools.Where(p => p.Value.Type == PoolType.Prop && p.Value.CurrentShare < _slicedShare[p.Key]).Count() > 0 ||
                    _slice.Where(s => s.Value > 0).Count() == 0)
                {
                    Reslice(pools);
                }
            }

            foreach (var i in _slice)
            {
                if(i.Value > 0)
                {
                    best = i.Key;
                    break;
                }
            }

            return best ?? ("smpps");
        }
    }
}