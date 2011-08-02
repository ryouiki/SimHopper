using System;
using System.Collections.Generic;
using System.Linq;
using MTRand;

namespace SimHopper
{
    public class RouletteRoundShare : IHopStrategy
    {
        private readonly int _difficulty;
        private const int MaxRouletteDelay = 20;
        private int _currentRouletteDelay = 0;
        private MersenneTwister _rnd;

        public RouletteRoundShare(int difficulty)
        {
            _difficulty = difficulty;
            _rnd = new MersenneTwister((uint)DateTime.Now.Ticks);
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            if (currentPool != "")
            {
                if (_currentRouletteDelay++ < MaxRouletteDelay)
                {
                    return currentPool;
                }
                else
                {
                    _currentRouletteDelay = 0;
                }
            }

            var sharePropPools = pools
                .Where(pool => pool.Value.Type == PoolType.Prop && pool.Value.CurrentShare < _difficulty * 0.43)
                .ToDictionary(pool => pool.Key, pool => _difficulty / pool.Value.CurrentShare);

            if (sharePropPools.Count == 0)
            {
                sharePropPools = pools
                    .Where(pool => pool.Value.Type == PoolType.PropEarlyHop && pool.Value.CurrentShare * 4.0 < _difficulty * 0.43)
                    .ToDictionary(pool => pool.Key, pool => _difficulty / pool.Value.CurrentShare);
            }

            if (sharePropPools.Count == 0)
            {
                sharePropPools = pools
                    .Where(pool => pool.Value.Type == PoolType.Score && pool.Value.CurrentShare * 4.0 < _difficulty * 0.43)
                    .ToDictionary(pool => pool.Key, pool => _difficulty / pool.Value.CurrentShare);
            }

            if (sharePropPools.Count == 0)
            {
                sharePropPools = pools
                    .Where(pool => pool.Value.Type == PoolType.Pplns && pool.Value.CurrentShare * 4.0 < _difficulty * 0.43)
                    .ToDictionary(pool => pool.Key, pool => _difficulty / pool.Value.CurrentShare);
            }

            if (sharePropPools.Count > 0)
            {
                best = _rnd.Roulette(sharePropPools);
            }

            return best ?? (best = "smpps");
        }
    }
}