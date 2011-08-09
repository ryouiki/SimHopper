using System.Collections.Generic;
using System.Linq;

namespace SimHopper
{
    public class MinRoundShare : IHopStrategy
    {
        public double Threshold { get; set; }
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

            double minProgress = Threshold;

            foreach (var pool in pools.Where(p => p.Value.Type != PoolType.Smpps))
            {
                var progress = pool.Value.BaseProgress + (pool.Value.CurrentShare / _difficulty) * pool.Value.PenaltyFactor;

                if (progress < minProgress)
                {
                    minProgress = progress;
                    best = pool.Key;
                }
            }

            return best ?? (best = "smpps");
        }
    }
}