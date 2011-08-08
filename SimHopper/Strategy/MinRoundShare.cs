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

    public class MinRoundShare3 : IHopStrategy
    {
        public double Threshold { get; set; }
        private readonly int _difficulty;

        public MinRoundShare3(int difficulty)
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
                var progress = pool.Value.BaseProgress + (pool.Value.CurrentShare / _difficulty)*pool.Value.PenaltyFactor;

                if (progress < minProgress)
                {
                    minProgress = progress;
                    best = pool.Key;
                }
            }

            return best ?? (best = "smpps");
        }
    }


    public class MinRoundShare2 : IHopStrategy
    {
        public double Threshold { get; set; }
        private readonly int _difficulty;

        public double ProtectProp { get; set; }
        public double EarliHopFactor { get; set; }
        public double PPLNSFactor { get; set; }
        public double ScoreFactor { get; set; }

        public MinRoundShare2(int difficulty)
        {
            _difficulty = difficulty;

            EarliHopFactor = 4.0f;
            PPLNSFactor = 4.0f;
            ScoreFactor = 4.0f;
            Threshold = 0.43f;
            ProtectProp = Threshold;
        }

        public string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds)
        {
            string best = null;

            // count progress

            double minProgress = Threshold;

            bool onlyProp = false;
            if(pools.Any(p=>p.Value.Type == PoolType.Prop && (p.Value.CurrentShare/_difficulty) < ProtectProp))
            {
                onlyProp = true;
            }

            foreach (var pool in pools.Where(p => p.Value.Type != PoolType.Smpps ))
            {
                var progress = pool.Value.CurrentShare / _difficulty;
                if (onlyProp && pool.Value.Type != PoolType.Prop)
                {
                    continue;
                }

                switch (pool.Value.Type)
                {
                    case PoolType.PropEarlyHop:
                        progress *= EarliHopFactor;
                        break;
                    case PoolType.Pplns:
                        progress *= PPLNSFactor;
                        break;
                    case PoolType.Score:
                        progress *= ScoreFactor;
                        break;
                }

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