using System.Collections.Generic;

namespace SimHopper
{
    public class MinRoundTime : IHopStrategy
    {
        public double Threshold { get; set; }
        private readonly int _difficulty;

        public MinRoundTime(int difficulty)
        {
            _difficulty = difficulty;
            Threshold = 0.43f;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count roundTime
            int minRoundTime = 100000;

            foreach (var pool in pools)
            {
                if (pool.Value.Type == PoolType.Prop)
                {
                    var progress = pool.Value.CurrentShare / _difficulty;
                    var roundTime = pool.Value.RoundTime;

                    if (progress < Threshold && roundTime < minRoundTime)
                    {
                        minRoundTime = roundTime;
                        best = pool.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.PropEarlyHop)
                    {
                        var progress = pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        progress *= 4.0f;

                        if (progress < Threshold && roundTime < minRoundTime)
                        {
                            minRoundTime = roundTime;
                            best = pool.Key;
                        }
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.Pplns)
                    {
                        var progress = pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        progress *= 4.0f;

                        if (progress < Threshold && roundTime < minRoundTime)
                        {
                            minRoundTime = roundTime;
                            best = pool.Key;
                        }
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.Score)
                    {
                        var progress = pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        progress *= 4.0f;

                        if (progress < Threshold && roundTime < minRoundTime)
                        {
                            minRoundTime = roundTime;
                            best = pool.Key;
                        }
                    }
                }
            }

            if (best == null)
            {
                best = "smpps";
            }

            return best;
        }
    }
}