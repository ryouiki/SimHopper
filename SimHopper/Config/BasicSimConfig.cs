using System.Collections.Generic;

namespace SimHopper
{
    public class BasicSimConfig : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        public BasicSimConfig(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            MaxSimulationGeneration = 73;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            Servers.Add("ideal", new PoolServer("ideal", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("mtred", new PoolServer("mtred", PoolType.Prop, 350, 300, 8.18f, targetHandler));
            //Servers.Add("polmine", new PoolServer("polmine", PoolType.Prop, 130, 300, 6.7f, targetHandler));
            //Servers.Add("bitclockers", new PoolServer("bitclockers", PoolType.Prop, 233, 60, 5.0f, targetHandler));
            //Servers.Add("rfcpool", new PoolServer("rfcpool", PoolType.Prop, 90, 60, 2.1f, targetHandler));
            //Servers.Add("triplemining", new PoolServer("triplemining", PoolType.Prop, 72, 60, 7.6f, targetHandler));
            //Servers.Add("ozco", new PoolServer("ozco", PoolType.Prop, 122, 60, 8.52f, targetHandler));
            //Servers.Add("nofeemining", new PoolServer("nofeemining", PoolType.Prop, 30, 60, 3.5f, targetHandler));
            //Servers.Add("poolmunity", new PoolServer("poolmunity", PoolType.Prop, 10, 60, 2.26f, targetHandler));
            //Servers.Add("bclc", new PoolServer("bclc", PoolType.Prop, 500, 1800, 8.1f, targetHandler));
            //_servers.Add("mini-1", new PoolServer("mini-1", PoolType.Prop, 8, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("mini-2", new PoolServer("mini-2", PoolType.Prop, 8, -1, 60, 5.0f, GetNextTarget));

            //_servers.Add("tiny-1", new PoolServer("tiny-1", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-2", new PoolServer("tiny-2", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-3", new PoolServer("tiny-3", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-4", new PoolServer("tiny-4", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));

            //_servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, -1, 60, 8.13f, GetNextTarget));
            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));
        }

        public string SetupGeneration(int generation)
        {
            var threshold = 0.235f + 0.005f * generation;
            var title = string.Format("{0}{1:0.000}", "minRS", threshold);

            Strategy = new MinRoundShare(Difficulty) {Threshold = threshold};

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }
    }

    public class SlushTestConfig : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        public SlushTestConfig(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            MaxSimulationGeneration = 140;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            Servers.Add("ideal", new PoolServer("ideal", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, 60, 8.13f, targetHandler));

            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));
        }

        public string SetupGeneration(int generation)
        {
            var x = generation%14 + 1;
            var y = (int) (generation/14) + 1;
            var protectProp = 0.15 + 0.02 * x;
            var scoreFactor = 1.5 + 0.3 * y;
            var title = string.Format("{0}{1:0.00}-{2:0.00}", "slushTest", protectProp, scoreFactor);

            Strategy = new MinRoundShare2(Difficulty) { ScoreFactor = scoreFactor, ProtectProp = protectProp };

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }
    }

    public class SlushTestConfig2 : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        public SlushTestConfig2(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            MaxSimulationGeneration = 112;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            Servers.Add("ideal1", new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal2", new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal3", new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal4", new PoolServer("ideal4", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal5", new PoolServer("ideal5", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, 60, 8.13f, targetHandler));

            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));
        }

        public string SetupGeneration(int generation)
        {
            var x = (int)(generation / 8) + 1;
            var y = generation % 8;
            var baseProgress = 0.13 + 0.02 * x;
            var penaltyFactor = 1.8 + 0.4 * y;
            Servers["slush"].BaseProgress = (float)baseProgress;
            Servers["slush"].PenaltyFactor = (float)penaltyFactor;

            var title = string.Format("{0}{1:0.00}-{2:0.00}", "rs3_slush", baseProgress, penaltyFactor);

            Strategy = new MinRoundShare3(Difficulty);

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }
    }

    public class BclcTestConfig : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        public BclcTestConfig(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 170;
            MaxSimulationGeneration = 112;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            Servers.Add("ideal1", new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal2", new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("ideal3", new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("bclc", new PoolServer("bclc", PoolType.Prop, 500, 2700, 8.1f, targetHandler));

            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));
        }

        public string SetupGeneration(int generation)
        {
            var x = (int)(generation / 8) + 1;
            var y = generation % 8;
            var baseProgress = 0.10 + 0.02 * x;
            var penaltyFactor = 1.0 + 0.025 * y;
            Servers["bclc"].BaseProgress = (float)baseProgress;
            Servers["bclc"].PenaltyFactor = (float)penaltyFactor;

            var title = string.Format("{0}{1:0.00}-{2:0.00}", "rs3_bclc", baseProgress, penaltyFactor);

            Strategy = new MinRoundShare3(Difficulty);

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }
    }
}