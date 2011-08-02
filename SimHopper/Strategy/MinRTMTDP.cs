using System.Collections.Generic;
using System.Linq;

namespace SimHopper
{
    public class MinRTMTDP : IHopStrategy
    {
        public float Threshold { get; set; }
        private readonly int _difficulty;

        public MinRTMTDP(int difficulty)
        {
            _difficulty = difficulty;
            Threshold = 0.43f;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count roundTime
            int minRoundTime = 100000;

            var propPoolCount = pools.Where(s => s.Value.Type == PoolType.Prop).Count();

            var t1 = Threshold / (0.2f * propPoolCount + 0.8f);
            var threshold = new float[] { t1, (Threshold + t1) * 0.5f, Threshold }; // 3-pass

            for (int i = 0; i < threshold.Length; ++i)
            {
                if (best != null)
                {
                    break;
                }
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.Prop))
                {
                    var progress = (float) pool.Value.CurrentShare/_difficulty;
                    var roundTime = pool.Value.RoundTime;

                    double factor = 2.0 - pool.Value.GHashSpeed/1500.0;

                    roundTime = (int) (factor*roundTime);

                    if (progress < threshold[i] && roundTime < minRoundTime)
                    {
                        minRoundTime = roundTime;
                        best = pool.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.PropEarlyHop))
                {
                    var progress = (float)pool.Value.CurrentShare / _difficulty;
                    var roundTime = pool.Value.RoundTime;

                    roundTime = (int)(4.0f * roundTime);
                    progress *= 4.0f;

                    if (progress < Threshold && roundTime < minRoundTime)
                    {
                        minRoundTime = roundTime;
                        best = pool.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.Pplns))
                {
                    var progress = (float) pool.Value.CurrentShare/_difficulty;
                    var roundTime = pool.Value.RoundTime;

                    roundTime = (int) (4.0f*roundTime);
                    progress *= 4.0f;

                    if (progress < Threshold && roundTime < minRoundTime)
                    {
                        minRoundTime = roundTime;
                        best = pool.Key;
                    }
                }
            }

            if (best == null)
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.Score))
                {
                    var progress = (float)pool.Value.CurrentShare / _difficulty;
                    var roundTime = pool.Value.RoundTime;

                    roundTime = (int)(4.0f * roundTime);
                    progress *= 4.0f;

                    if (progress < Threshold && roundTime < minRoundTime)
                    {
                        minRoundTime = roundTime;
                        best = pool.Key;
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