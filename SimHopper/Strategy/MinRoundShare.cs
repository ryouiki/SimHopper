using System.Collections.Generic;

namespace SimHopper
{
    public class MinRoundShare : IHopStrategy
    {
        public float Threshold { get; set; }
        private readonly int _difficulty;

        public MinRoundShare(int difficulty)
        {
            _difficulty = difficulty;
            Threshold = 0.43f;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count progress

            float minProgress = 1000.0f;

            foreach (var pool in pools)
            {
                var progress = minProgress;
                if (pool.Value.Type == PoolType.Prop)
                {
                    progress = (float)pool.Value.CurrentShare / _difficulty;
                }

                if (progress < Threshold && progress < minProgress)
                {
                    minProgress = progress;
                    best = pool.Key;
                }
            }

            if (best =="")
            {
                foreach (var pool in pools)
                {
                    var progress = minProgress;
                    if (pool.Value.Type == PoolType.PropEarlyHop)
                    {
                        progress = (float)pool.Value.CurrentShare / _difficulty;
                        progress *= 4.0f;
                    }

                    if (progress < Threshold && progress < minProgress)
                    {
                        minProgress = progress;
                        best = pool.Key;
                    }
                }
            }

            if (best == "")
            {
                foreach (var pool in pools)
                {
                    var progress = minProgress;
                    if (pool.Value.Type == PoolType.Pplns)
                    {
                        progress = (float)pool.Value.CurrentShare / _difficulty;
                        progress *= 4.0f;
                    }

                    if (progress < Threshold && progress < minProgress)
                    {
                        minProgress = progress;
                        best = pool.Key;
                    }
                }
            }

            if (best == "")
            {
                foreach (var pool in pools)
                {
                    var progress = minProgress;
                    if (pool.Value.Type == PoolType.Score)
                    {
                        progress = (float)pool.Value.CurrentShare / _difficulty;
                        progress *= 4.0f;
                    }

                    if (progress < Threshold && progress < minProgress)
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