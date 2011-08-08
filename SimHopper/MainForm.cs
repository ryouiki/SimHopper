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

        private ISimConfig _simConfig;

        private List<RoundResult> _results = new List<RoundResult>();

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

            //
            //
            // Simulation Configuration Here //
            //
            //
            _simConfig = new SlushTestConfig2(GetNextTarget);

            labelAdvPerTick.Text = _simConfig.InitialSimulationSpeedUp.ToString();

            SetupGeneration();
        }

        public void SetupGeneration()
        {
            ++_currentSimGeneration;
            _currentSimRound = 0;

            // simuation setup for the generation

            _currentGenerationTitle = _simConfig.SetupGeneration(_currentSimGeneration);
            labelGeneration.Text = _currentGenerationTitle;

            _stat = new Stat(_simConfig.MaxSimulationDay);
            SetupRound();
        }

        public void FinishSimGeneration()
        {
            if (_currentSimGeneration < _simConfig.MaxSimulationGeneration)
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
            LoadRoundshares();

            _simConfig.InitializeServers();

            _results.Clear();
            _elapsedTime = 0;
            _totalHop = 0;
            _currentServer = "";
            ++_currentSimRound;

            labelSimulRound.Text = "Simulation Round #" + _currentSimRound;

            _advPerTick = Convert.ToInt32(labelAdvPerTick.Text);
            _toLog = true;

            _currentServer = SelectBestPool(0);
        }

        private void LoadRoundshares()
        {
            _roundShares = new List<int>();
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
                _rnd.GenerateRoundShares(_roundShares, 50, (uint)_simConfig.Difficulty);
            }

            return target;
        }

        private string SelectBestPool(int advancedSeconds)
        {
            return _simConfig.Strategy.GetBestPool(_simConfig.Servers, _currentServer, advancedSeconds);
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
            foreach (var info in _simConfig.Servers)
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
            double pps = 50.0f / _simConfig.Difficulty;
            var pool = _simConfig.Servers[_currentServer];
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

            dataGridPools.Rows.Clear();
            foreach (var server in _simConfig.Servers)
            {
                double eff = 0;
                if (server.Value.MyTotalShare>0)
                {
                    eff = 100*(server.Value.MyTotalProfit/server.Value.MyTotalShare)/pps;
                }
                var progress = string.Format("{0}% {1}", (int) (server.Value.CurrentShare/_simConfig.Difficulty*100),
                                             server.Value.CurrentShare);

                dataGridPools.Rows.Add(server.Key, (int)server.Value.MyValidShare, progress,
                                       server.Value.RoundTime,
                                       string.Format("{0:0.00}", eff),
                                       server.Value.MyTotalProfit);
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

            double propEff=0;
            double pplnsEff=0;
            double scoreEff=0;
            double hopPerDay = 0;

            if (propTotalShare>0)
            {
                propEff = 100.0 * propEarn / (pps * propTotalShare);
            }
            labelPropEarn.Text = string.Format("Prop. earn : {0:0.00000000} BTC", propEarn);
            labelPropEff.Text = string.Format("Prop. Eff : {1} share - {0:0.00}%", propEff, propTotalShare);

            if(pplnsTotalShare>0)
            {
                pplnsEff = 100.0*pplnsEarn/(pps*pplnsTotalShare);
            }
            labelPplnsEarn.Text = string.Format("PPLNS earn : {0:0.00000000} BTC", pplnsEarn);
            labelPplnsEff.Text = string.Format("PPLNS Eff : {1} share - {0:0.00}%", pplnsEff, pplnsTotalShare);

            if (scoreTotalShare>0)
            {
                scoreEff = 100.0 * scoreEarn / (pps * scoreTotalShare);
            }
            labelScoreEarn.Text = string.Format("Score earn : {0:0.00000000} BTC", scoreEarn);
            labelScoreEff.Text = string.Format("Score Eff : {1} share - {0:0.00}%", scoreEff, scoreTotalShare);

            var smppsEarn = pps * _simConfig.Servers["smpps"].MyValidShare;

            labelPPSEarn.Text = string.Format("SMPPS earn : {0:0.00000000} BTC", smppsEarn);

            labelTotalEarn.Text = string.Format("Total earn : {0:0.00000000} BTC", propEarn + pplnsEarn + smppsEarn);

            totalShare = propTotalShare + pplnsTotalShare + scoreTotalShare + (int)_simConfig.Servers["smpps"].MyValidShare;

            var totalEff = 100.0*(propEarn + pplnsEarn + scoreEarn + smppsEarn)/(pps*totalShare);
            labelTotalEff.Text = string.Format("Total Eff : {1} share - {0:0.00}%", totalEff, totalShare);

            if (days > 0)
            {
                hopPerDay = (double)_totalHop/days;
            }
            labelHop.Text = string.Format("{0:0.00} Hops per day", hopPerDay);

            var totalEarn = propEarn + pplnsEarn + scoreEarn + smppsEarn;

            var stat = new StatElement(days, propEff, propEarn, scoreEff, scoreEarn, pplnsEff, pplnsEarn, smppsEarn,
                                       totalEff, totalEarn, _totalHop);
            _stat.AddStat(stat);

            if (days > _simConfig.MaxSimulationDay)
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
                    if (_currentSimRound < _simConfig.MaxSimulationRound)
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
