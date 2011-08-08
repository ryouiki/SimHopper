using System.Collections.Generic;

namespace SimHopper
{
    public class PoolAddingSimConfig : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }
        
        private PoolServer[] _serversToAdd;

        public PoolAddingSimConfig(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();            
            _serversToAdd = new PoolServer[]
                                {
                                    new PoolServer("smpps", PoolType.Smpps, 100, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal4", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal5", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal6", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal7", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal8", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal9", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                    new PoolServer("ideal10", PoolType.Prop, 1000, 0, 0.0f, targetHandler),
                                };

            MaxSimulationGeneration = _serversToAdd.Length-1;
        }

        public string SetupGeneration(int generation)
        {
            Servers.Clear();
            Servers.Add(_serversToAdd[0].PoolName, _serversToAdd[0]);   // smpps

            for(int i=1;i<=generation;++i)
            {
                Servers.Add(_serversToAdd[i].PoolName, _serversToAdd[i]);
            }

            var title = string.Format("Prop.Pools-{0}RT", generation);

            Strategy = new MinRoundTime(Difficulty);

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