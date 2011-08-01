using System.Collections.Generic;

namespace SimHopper
{
    public class MinRoundTime : IHopStrategy
    {
        private readonly int _difficulty;

        public MinRoundTime(int difficulty)
        {
            _difficulty = difficulty;
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
                    var progress = (float)pool.Value.CurrentShare / _difficulty;
                    var roundTime = pool.Value.RoundTime;

                    if (progress < 0.43f && roundTime < minRoundTime)
                    {
                        best = pool.Key;
                    }
                }
            }


            if (best == null)
            {
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.Pplns)
                    {
                        var progress = (float)pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        if (pool.Value.Type == PoolType.Pplns)
                        {
                            progress *= 4.0f;
                        }

                        if (progress < 0.43f && roundTime < minRoundTime)
                        {
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
                        var progress = (float)pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        if (pool.Value.Type == PoolType.Score)
                        {
                            progress *= 4.0f;
                        }

                        if (progress < 0.43f && roundTime < minRoundTime)
                        {
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