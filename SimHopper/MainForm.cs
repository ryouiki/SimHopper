using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTRand;

namespace SimHopper
{
    public partial class MainForm : Form
    {
        private MersenneTwister _rnd;
        private int _elapsedTime = 0;
        private List<int> _roundShares;
        private bool _toLog = true;
        private Stat _stat;
        private int _totalHop;

        private int Difficulty = 1888786;
        private int MaxSimulationDay = 100;
        private int MaxSimulationRound = 200;
        private int MaxSimulationGeneration = 1;
        private int InitialSimulationSpeedUp = 8192;

        private Dictionary<string, PoolServer> _servers = new Dictionary<string, PoolServer>();
        private List<RoundResult> _results = new List<RoundResult>();
        private Dictionary<string, IHopStrategy> _strategies;

        private string _currentStrategy;
        private string _currentServer = "";
        private int _advPerTick = 1;
        private int _currentSimRound = 0;
        private int _currentSimGeneration = 0;
        private string _currentGenerationTitle = "";

        public MainForm()
        {
            InitializeComponent();

            InitializeSimutlator();
        }

        public void InitializeSimutlator()
        {
            var seed = (uint)DateTime.Now.Ticks;
            _rnd = new MersenneTwister(seed);
            _currentSimGeneration = 0;
            labelAdvPerTick.Text = InitialSimulationSpeedUp.ToString();
            SetupGeneration();
        }

        public void SetupGeneration()
        {
            ++_currentSimGeneration;
            _currentSimRound = 0;
            _strategies = new Dictionary<string, IHopStrategy>
                              {
                                  {"MinRoundShare", new MinRoundShare(Difficulty)},
                                  {"MinRoundTime", new MinRoundTime(Difficulty)},
                                  {"RouletteRoundShare", new RouletteRoundShare(Difficulty)},
                                  {"RouletteRoundShare2", new RouletteRoundShare2(Difficulty)},
                                  {"MinRTMTDP", new MinRTMTDP(Difficulty)},
                                  {"Flower_2400", new Flower(Difficulty) {SliceSize = 2400.0}},
                                  {"Flower_1200", new Flower(Difficulty) {SliceSize = 1200.0}},
                                  {"Flower_600", new Flower(Difficulty) {SliceSize = 600.0}},
                                  {"Flower_300", new Flower(Difficulty) {SliceSize = 300.0}}
                              };

            // simuation setup for the generation

            //{
            //    // test threshold for time-slicing
            //    MaxSimulationDay = 500;
            //    MaxSimulationRound = 20;
            //    MaxSimulationGeneration = 10;
            //    var threshold = 0.35f + 0.02f*_currentSimGeneration;
            //    _currentGenerationTitle = string.Format("Flower_thr{0:0.00}", threshold);
            //    _strategies.Add(_currentGenerationTitle, new Flower(Difficulty) { Threshold = threshold });
            //    labelGeneration.Text = _currentGenerationTitle;

            //    _currentStrategy = _currentGenerationTitle;
            //}

            { 
                // test thresholds for ryouiki's
                MaxSimulationDay = 1000;
                MaxSimulationRound = 20;
                MaxSimulationGeneration = 20;
                var threshold = 0.35f + 0.01f * _currentSimGeneration;
                _currentGenerationTitle = string.Format("ryouiki_thr{0:0.00}", threshold);
                _strategies.Add(_currentGenerationTitle, new MinRTMTDP(Difficulty) { Threshold = threshold });
                labelGeneration.Text = _currentGenerationTitle;

                _currentStrategy = _currentGenerationTitle;
            }

            _stat = new Stat(MaxSimulationDay);
            SetupRound();
        }

        public void FinishSimGeneration()
        {
            if (_currentSimGeneration< MaxSimulationGeneration)
            {
                SetupGeneration();
            }
            else
            {
                checkBoxAuto.Checked = false;
            }
        }

        public void SetupRound()
        {
            _roundShares = new List<int>();
            _servers.Clear();
            _results.Clear();
            _elapsedTime = 0;
            _totalHop = 0;
            _currentServer = "";
            ++_currentSimRound;

            LoadRoundshares();

            labelSimulRound.Text = "Simulation Round #" + _currentSimRound;

            _advPerTick = Convert.ToInt32(labelAdvPerTick.Text);
            _toLog = true;

            _servers.Add("mtred", new PoolServer("mtred", PoolType.Prop, 350, -1, 300, 8.18f, GetNextTarget));
            _servers.Add("polmine", new PoolServer("polmine", PoolType.Prop, 130, -1, 300, 6.7f, GetNextTarget));
            _servers.Add("bitclockers", new PoolServer("bitclockers", PoolType.Prop, 233, -1, 60, 5.0f, GetNextTarget));
            _servers.Add("rfcpool", new PoolServer("rfcpool", PoolType.Prop, 90, -1, 60, 2.1f, GetNextTarget));
            _servers.Add("triplemining", new PoolServer("triplemining", PoolType.Prop, 72, -1, 60, 7.6f, GetNextTarget));
            _servers.Add("ozco", new PoolServer("ozco", PoolType.Prop, 122, -1, 60, 8.52f, GetNextTarget));
            _servers.Add("nofeemining", new PoolServer("nofeemining", PoolType.Prop, 30, -1, 60, 3.5f, GetNextTarget));
            _servers.Add("poolmunity", new PoolServer("poolmunity", PoolType.Prop, 10, -1, 60, 2.26f, GetNextTarget));
            _servers.Add("bclc", new PoolServer("bclc", PoolType.Prop, 500, -1, 1800, 8.1f, GetNextTarget));

            //_servers.Add("btcg", new PoolServer("btcg", PoolType.PropEarlyHop, 2500, -1, 3600, 8.1f, GetNextTarget));

            //_servers.Add("btcserv", new PoolServer("btcserv", PoolType.Prop, 5, -1, 60, 5.0f, GetNextTarget));

            //_servers.Add("mini-1", new PoolServer("mini-1", PoolType.Prop, 8, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("mini-2", new PoolServer("mini-2", PoolType.Prop, 8, -1, 60, 5.0f, GetNextTarget));

            //_servers.Add("tiny-1", new PoolServer("tiny-1", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-2", new PoolServer("tiny-2", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-3", new PoolServer("tiny-3", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));
            //_servers.Add("tiny-4", new PoolServer("tiny-4", PoolType.Prop, 2, -1, 60, 5.0f, GetNextTarget));

            //_servers.Add("slush", new PoolServer("slush", PoolType.Score, 2000, -1, 60, 8.13f, GetNextTarget));
            //_servers.Add("mineco.in", new PoolServer("mineco.in", PoolType.Pplns, 150, -1, 60, 7.34f, GetNextTarget));
            _servers.Add("smpps", new PoolServer("smpps", PoolType.Smpps, 20, -1, 0, 0.0f, GetNextTarget));

            _currentServer = SelectBestPool(0);
        }

        private void LoadRoundshares()
        {
            string strSaveFilePath = "rand.txt";
            StreamReader reader = new StreamReader(strSaveFilePath, System.Text.Encoding.UTF8);
            string strFileLine = string.Empty;
            while ((strFileLine = reader.ReadLine()) != null)
            {
                var val = Convert.ToInt32(strFileLine);
                _roundShares.Add(val);
            }
            reader.Close();
        }

        public int GetNextTarget()
        {
            //var target = _roundShares.Dequeue();
            var index = _rnd.Next(_roundShares.Count - 1);
            var target = _roundShares[index];
            _roundShares.RemoveAt(index);

            if (_roundShares.Count == 0)
            {
                _rnd.GenerateRoundShares(_roundShares, 50, (uint)Difficulty);
            }

            return target;
        }

        private string SelectBestPool(int advancedSeconds)
        {
            return _strategies[_currentStrategy].GetBestPool(_servers, _currentServer, advancedSeconds);
        }

        private void InternalAdvance()
        {
            const int MIN_PER_TICK = 1;
            const int MYSHARE_PER_MIN = 48;

            var advancedSeconds = MIN_PER_TICK*60;
            var newServer = SelectBestPool(advancedSeconds); 
            if(_currentServer != newServer)
            {
                ++_totalHop;
            }
            _currentServer = newServer;

            _elapsedTime += MIN_PER_TICK * 60;

            // pool processing
            foreach (var info in _servers)
            {
                if (info.Key == _currentServer)
                {
                    var result = info.Value.Advance(advancedSeconds, MYSHARE_PER_MIN);
                    if (result != null)
                    {
                        _results.Add(result);
                    }
                }
                else
                {
                    var result = info.Value.Advance(advancedSeconds, 0);
                    if (result != null)
                    {
                        _results.Add(result);
                    }
                }
            }    
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_currentServer == "")
                return;

            for (int i = 0; i < _advPerTick; ++i)
            {
                InternalAdvance();
            }

            var days = (int)(_elapsedTime / 86400);
            var hours = (int)((_elapsedTime - (days * 86400)) / 3600);
            var minutes = (int)(((_elapsedTime - (days * 86400)) - (hours * 3600)) / 60);

            labelElapsedTime.Text = string.Format("{0}d {1}h {2}m", days, hours, minutes);
            labelCurrentPool.Text = string.Format("Current : {0}", _currentServer);

            // print current info
            double pps = 50.0f / Difficulty;
            var pool = _servers[_currentServer];
            if (pool.Type == PoolType.Prop || pool.Type == PoolType.PropEarlyHop)
            {
                var dd = (int)(pool.RoundTime / 86400);
                var hh = (int)((pool.RoundTime - (dd * 86400)) / 3600);
                var mm = (int)(((pool.RoundTime - (dd * 86400)) - (hh * 3600)) / 60);

                labelPoolInfo.Text = string.Format("Share - {3}({4}stale)/{5} RoundTime - {0}d {1}h {2}m : est earn {6:0.00000000}",
                    dd, hh, mm, (int)pool.MyValidShare, (int)pool.MyLostShare, (int)pool.CurrentShare, (int)pool.MyValidShare * 50.0f / pool.CurrentShare);
            }
            else if (pool.Type == PoolType.Pplns)
            {
                var dd = (int)(pool.RoundTime / 86400);
                var hh = (int)((pool.RoundTime - (dd * 86400)) / 3600);
                var mm = (int)(((pool.RoundTime - (dd * 86400)) - (hh * 3600)) / 60);

                labelPoolInfo.Text = string.Format("Share - {3}/{4} RoundTime - {0}d {1}h {2}m : est earn {5:0.00000000}",
                    dd, hh, mm, (int)pool.MyValidShare, (int)pool.CurrentShare, (int)pool.MyValidShare * 50.0f / pool.CurrentShare);                
            }
            else if (pool.Type == PoolType.Score)
            {
                labelPoolInfo.Text = string.Format("Score : {0:0.00}/{1:0.00} Share : {2}/{3}", pool.MyScore,
                                                   pool.TotalScore, (int)pool.MyValidShare, pool.CurrentShare);
            }
            else if (pool.Type == PoolType.Smpps)
            {
                labelPoolInfo.Text = string.Format("SMPPS my share : {0} / {1}rejected", (int)pool.MyValidShare, (int)pool.MyLostShare);
            }

            // print earn / eff
            double propEarn = 0.0;
            int propTotalShare = 0;
            double pplnsEarn = 0.0;
            int pplnsTotalShare = 0;

            double scoreEarn = 0.0;
            int scoreTotalShare = 0;

            long totalShare = 0;
            foreach (var roundResult in _results)
            {
                switch (roundResult.Type)
                {
                    case PoolType.Prop:
                    case PoolType.PropEarlyHop:
                        propEarn += roundResult.Profit;
                        propTotalShare += roundResult.ValidShare + roundResult.LostShare;
                        break;

                    case PoolType.Pplns:
                        pplnsEarn += roundResult.Profit;
                        pplnsTotalShare += roundResult.ValidShare + roundResult.LostShare;
                        break;
                    case PoolType.Score:
                        scoreEarn += roundResult.Profit;
                        scoreTotalShare += roundResult.ValidShare + roundResult.LostShare;
                        break;
                }
            }

            labelPropEarn.Text = string.Format("Prop. earn : {0:0.00000000} BTC", propEarn);

            propTotalShare = propTotalShare == 0 ? 1 : propTotalShare;
            var propEff = 100.0*propEarn/(pps*propTotalShare);
            labelPropEff.Text = string.Format("Prop. Eff : {1} share - {0:0.00}%", propEff, propTotalShare);

            labelPplnsEarn.Text = string.Format("PPLNS earn : {0:0.00000000} BTC", pplnsEarn);
            pplnsTotalShare = pplnsTotalShare == 0 ? 1 : pplnsTotalShare;
            var pplnsEff = 100.0*pplnsEarn/(pps*pplnsTotalShare);
            labelPplnsEff.Text = string.Format("PPLNS Eff : {1} share - {0:0.00}%", pplnsEff, pplnsTotalShare);

            labelScoreEarn.Text = string.Format("Score earn : {0:0.00000000} BTC", scoreEarn);
            scoreTotalShare = scoreTotalShare == 0 ? 1 : scoreTotalShare;
            var scoreEff = 100.0*scoreEarn/(pps*scoreTotalShare);
            labelScoreEff.Text = string.Format("Score Eff : {1} share - {0:0.00}%", scoreEff, scoreTotalShare);

            var smppsEarn = pps * _servers["smpps"].MyValidShare;

            labelPPSEarn.Text = string.Format("SMPPS earn : {0:0.00000000} BTC", smppsEarn);

            labelTotalEarn.Text = string.Format("Total earn : {0:0.00000000} BTC", propEarn + pplnsEarn + smppsEarn);

            totalShare = propTotalShare + pplnsTotalShare + scoreTotalShare + (int)_servers["smpps"].MyValidShare;

            var totalEff = 100.0*(propEarn + pplnsEarn + scoreEarn + smppsEarn)/(pps*totalShare);
            labelTotalEff.Text = string.Format("Total Eff : {1} share - {0:0.00}%", totalEff, totalShare);

            var hopPerDay = (float) _totalHop/days;
            labelHop.Text = string.Format("{0:0.00} Hops per day", hopPerDay);

            var totalEarn = propEarn + pplnsEarn + scoreEarn + smppsEarn;

            var stat = new StatElement(days, propEff, propEarn, scoreEff, scoreEarn, pplnsEff, pplnsEarn, smppsEarn,
                                       totalEff, totalEarn, _totalHop);
            _stat.AddStat(stat);

            if (days > MaxSimulationDay)
            {
                if (_toLog)
                {
                    var log = string.Format("{0:0.0} / {1:0.0} / {2:0.0} : / {3:0.0} | {4:0.000} BTC/day\n", propEff, pplnsEff, scoreEff, totalEff, totalEarn / (_elapsedTime / 86400.0));
                    PrintLog(log);
                    _stat.Dump(_currentGenerationTitle, false);
                    _toLog = false;
                }

                _advPerTick = 0;

                if (checkBoxAuto.Checked)
                {
                    if (_currentSimRound < MaxSimulationRound)
                    {
                        SetupRound();
                    }
                    else
                    {
                        _stat.Dump(_currentGenerationTitle, true);
                        FinishSimGeneration();
                    }
                }
            }
        }

        private void PrintLog(string log)
        {
            textBoxLog.AppendText(log);
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();
        }

        private void buttonSpeedDown_Click(object sender, EventArgs e)
        {
            _advPerTick /= 2;
            labelAdvPerTick.Text = _advPerTick.ToString();
        }

        private void buttonSpeedUp_Click(object sender, EventArgs e)
        {
            if (_advPerTick == 0)
            {
                _advPerTick = 1;
            }
            else
            {
                _advPerTick *= 2;
            }
            
            labelAdvPerTick.Text = _advPerTick.ToString();
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            SetupRound();
        }
    }


}
