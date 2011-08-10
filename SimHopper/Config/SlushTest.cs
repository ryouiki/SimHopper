using System;
using System.Collections.Generic;

namespace SimHopper
{
    public class SlushTest : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        private StatSummary _statSummary;
        private int _curGeneration;

        public SlushTest(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            //Servers.Add("ideal1", new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal2", new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal3", new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal4", new PoolServer("ideal4", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal5", new PoolServer("ideal5", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, 60, 8.13f, targetHandler));

            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 180, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));

            var MaxCol = 8;
            var columns = new List<string>();
            for (int col = 0; col < MaxCol; ++col)
            {
                var penaltyFactor = 3.4 + 0.2 * col;
                columns.Add(penaltyFactor.ToString());
            }

            var MaxRow = 10;
            var rows = new List<string>();
            for (int row = 0; row < MaxRow; ++row)
            {
                var baseProgress = 0.01 * row;
                rows.Add(baseProgress.ToString());
            }

            MaxSimulationGeneration = MaxCol * MaxRow;
            _statSummary = new StatSummary("slush3", columns, rows);
        }

        public string SetupGeneration(int generation)
        {
            _curGeneration = generation;
            var x = (generation - 1) / _statSummary.Columns.Count;
            var y = (generation - 1)%_statSummary.Columns.Count;

            Servers["slush"].BaseProgress = (float)Convert.ToDouble(_statSummary.Rows[x]);
            Servers["slush"].PenaltyFactor = (float)Convert.ToDouble(_statSummary.Columns[y]);

            var title = string.Format("{0}{1:0.00}-{2:0.00}", _statSummary.StatName,
                                      Servers["slush"].BaseProgress, Servers["slush"].PenaltyFactor);

            Strategy = new MinRoundShare(Difficulty);

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }

        public void FinishGeneration(Stat stat)
        {
            ///
            var x = (_curGeneration - 1) / _statSummary.Columns.Count;
            var y = (_curGeneration-1) % _statSummary.Columns.Count;

            _statSummary.SetStat(x, y, stat.GetLastDay().TotalEff);
            _statSummary.Dump();
        }
    }

    public class MinecoTest : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        private StatSummary _statSummary;
        private int _curGeneration;

        public MinecoTest(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            InitialSimulationSpeedUp = 160000;

            Servers = new Dictionary<string, PoolServer>();
            //Servers.Add("ideal1", new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal2", new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal3", new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal4", new PoolServer("ideal4", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal5", new PoolServer("ideal5", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, 60, 8.13f, targetHandler));

            Servers.Add("mineco", new PoolServer("mineco.in", PoolType.Pplns, 150, 180, 7.34f, targetHandler));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));

            var MaxCol = 15;
            var columns = new List<string>();
            for (int col = 0; col < MaxCol; ++col)
            {
                var penaltyFactor = 1.0 + 0.2 * col;
                columns.Add(penaltyFactor.ToString());
            }

            var MaxRow = 43;
            var rows = new List<string>();
            for (int row = 0; row < MaxRow; ++row)
            {
                var baseProgress = 0.01 * row;
                rows.Add(baseProgress.ToString());
            }

            MaxSimulationGeneration = MaxCol * MaxRow;
            _statSummary = new StatSummary("mineco", columns, rows);
        }

        public string SetupGeneration(int generation)
        {
            _curGeneration = generation;
            var x = (generation - 1) / _statSummary.Columns.Count;
            var y = (generation - 1) % _statSummary.Columns.Count;

            Servers["mineco"].BaseProgress = (float)Convert.ToDouble(_statSummary.Rows[x]);
            Servers["mineco"].PenaltyFactor = (float)Convert.ToDouble(_statSummary.Columns[y]);

            var title = string.Format("{0}{1:0.00}-{2:0.00}", _statSummary.StatName,
                                      Servers["mineco"].BaseProgress, Servers["mineco"].PenaltyFactor);

            Strategy = new MinRoundShare(Difficulty);

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }

        public void FinishGeneration(Stat stat)
        {
            ///
            var x = (_curGeneration - 1) / _statSummary.Columns.Count;
            var y = (_curGeneration - 1) % _statSummary.Columns.Count;

            _statSummary.SetStat(x, y, stat.GetLastDay().TotalEff);
            _statSummary.Dump();
        }
    }

    public class BclcTest : ISimConfig
    {
        public int Difficulty { get; private set; }
        public int MaxSimulationDay { get; private set; }
        public int MaxSimulationRound { get; private set; }
        public int MaxSimulationGeneration { get; private set; }
        public int InitialSimulationSpeedUp { get; private set; }

        public Dictionary<string, PoolServer> Servers { get; private set; }
        public IHopStrategy Strategy { get; private set; }

        private StatSummary _statSummary;
        private int _curGeneration;

        public BclcTest(GetTargetShareHandler targetHandler)
        {
            Difficulty = 1888786;
            MaxSimulationDay = 520;
            MaxSimulationRound = 150;
            InitialSimulationSpeedUp = 120000;

            Servers = new Dictionary<string, PoolServer>();
            //Servers.Add("ideal1", new PoolServer("ideal1", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal2", new PoolServer("ideal2", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal3", new PoolServer("ideal3", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal4", new PoolServer("ideal4", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            //Servers.Add("ideal5", new PoolServer("ideal5", PoolType.Prop, 1000, 0, 0.0f, targetHandler));
            Servers.Add("bclc", new PoolServer("bclc", PoolType.Prop, 500, 2700, 8.1f, targetHandler)); // avg. 45min delay

            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            Servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, 0, 0.0f, targetHandler));

            var MaxCol = 16;
            var columns = new List<string>();
            for (int col = 0; col < MaxCol; ++col)
            {
                var penaltyFactor = 1.0 + 0.01 * col;
                columns.Add(penaltyFactor.ToString());
            }

            var MaxRow = 24;
            var rows = new List<string>();
            for (int row = 0; row < MaxRow; ++row)
            {
                var baseProgress = 0.0 + 0.01 * row;
                rows.Add(baseProgress.ToString());
            }

            MaxSimulationGeneration = MaxCol * MaxRow;
            _statSummary = new StatSummary("bclc", columns, rows);
        }

        public string SetupGeneration(int generation)
        {
            _curGeneration = generation;
            var x = (generation - 1) / _statSummary.Columns.Count;
            var y = (generation - 1) % _statSummary.Columns.Count;

            Servers["bclc"].BaseProgress = (float)Convert.ToDouble(_statSummary.Rows[x]);
            Servers["bclc"].PenaltyFactor = (float)Convert.ToDouble(_statSummary.Columns[y]);

            var title = string.Format("{0}{1:0.00}-{2:0.00}", _statSummary.StatName,
                Servers["bclc"].BaseProgress, Servers["bclc"].PenaltyFactor);

            Strategy = new MinRoundShare(Difficulty);

            return title;
        }

        public void InitializeServers()
        {
            foreach (var poolServer in Servers)
            {
                poolServer.Value.Initialize(-1);
            }
        }

        public void FinishGeneration(Stat stat)
        {
            ///
            var x = (_curGeneration - 1) / _statSummary.Columns.Count;
            var y = (_curGeneration - 1) % _statSummary.Columns.Count;

            _statSummary.SetStat(x, y, stat.GetLastDay().TotalEff);
            _statSummary.Dump();
        }
    }
}