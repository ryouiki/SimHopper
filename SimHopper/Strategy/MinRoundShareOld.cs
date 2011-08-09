using System.Collections.Generic;
using System.Linq;

namespace SimHopper
{
    public class MinRoundShareOld : IHopStrategy
    {
        public double Threshold { get; set; }
        private readonly int _difficulty;

        public MinRoundShareOld(int difficulty)
        {
            _difficulty = difficulty;
            Threshold = 0.43f;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count progress

            double minProgress = Threshold;

            foreach (var pool in pools.Where(p=>p.Value.Type==PoolType.Prop))
            {
                var progress = pool.Value.CurrentShare / _difficulty;

                if (progress < minProgress)
                {
                    minProgress = progress;
                    best = pool.Key;
                }
            }

            if (best =="")
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.PropEarlyHop))
                {
                    var progress = pool.Value.CurrentShare / _difficulty;
                    progress *= 4.0f;

                    if (progress < minProgress)
                    {
                        minProgress = progress;
                        best = pool.Key;
                    }
                }
            }

            if (best == "")
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.Pplns))
                {
                    
                    var progress = pool.Value.CurrentShare / _difficulty;
                    progress *= 4.0f;

                    if (progress < minProgress)
                    {
                        minProgress = progress;
                        best = pool.Key;
                    }
                }
            }

            if (best == "")
            {
                foreach (var pool in pools.Where(p => p.Value.Type == PoolType.Score))
                {
                    
                    var progress = pool.Value.CurrentShare / _difficulty;
                    progress *= 4.0f;
                    
                    if (progress < minProgress)
                    {
                        minProgress = progress;
                        best = pool.Key;
                    }
                }
            }

            return best ?? (best = "smpps");
        }
    }
}