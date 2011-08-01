using System.Collections.Generic;
using System.Linq;

namespace SimHopper
{
    public class MinRTMTDP : IHopStrategy
    {
        private readonly int _difficulty;

        public MinRTMTDP(int difficulty)
        {
            _difficulty = difficulty;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count roundTime
            int minRoundTime = 100000;

            var propPoolCount = pools.Where(s => s.Value.Type == PoolType.Prop).Count();

            var t1 = 0.43f/(0.2f*propPoolCount + 0.8f);
            var threshold = new float[] { t1, (0.43f + t1)*0.5f, 0.43f }; // 3-pass

            for (int i = 0; i < threshold.Length; ++i)
            {
                if (best != null)
                {
                    break;
                }
                foreach (var pool in pools)
                {
                    if (pool.Value.Type == PoolType.Prop)
                    {
                        var progress = (float)pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        double factor = 2.0 - pool.Value.GHashSpeed / 1500.0;

                        roundTime = (int)(factor * roundTime);

                        if (progress < threshold[i] && roundTime < minRoundTime)
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
                    if (pool.Value.Type == PoolType.Pplns)
                    {
                        var progress = (float)pool.Value.CurrentShare / _difficulty;
                        var roundTime = pool.Value.RoundTime;

                        double factor = 2.0 - pool.Value.GHashSpeed / 500.0;

                        if (pool.Value.Type == PoolType.Pplns)
                        {
                            roundTime = (int)(4.0f * roundTime);
                            progress *= 4.0f;
                        }

                        //roundTime = (int)(factor * roundTime);

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

                        double factor = 2.0 - pool.Value.GHashSpeed / 500.0;

                        if (pool.Value.Type == PoolType.Score)
                        {
                            //roundTime = (int)(8.0f * roundTime);
                            progress *= 4.0f;
                        }

                        //roundTime = (int)(factor * roundTime);

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