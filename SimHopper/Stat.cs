using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimHopper
{
    public class StatSummary
    {
        public string StatName { get; private set; }

        private double[,] _stats;
        private List<string> _columns;
        private List<string> _rows;

        public StatSummary(string name, List<string> columns, List<string> rows)
        {
            StatName = name;

            _stats = new double[rows.Count, columns.Count];
            for (int i = 0; i < rows.Count;++i )
            {
                for (int j = 0; j < columns.Count; ++j)
                {
                    _stats[i, j] = 0;
                }
            }

            _columns = columns;
            _rows = rows;
        }

        public void SetStat(int row, int column, double value)
        {
            _stats[row, column] = value;
        }

        public void Dump()
        {
            string strSaveFilePath = "Summary_" + StatName + ".txt";
            
            var writer = new StreamWriter(strSaveFilePath, false, System.Text.Encoding.UTF8);

            var colLine = "_";
            for (int col = 0; col < _columns.Count;++col )
            {
                colLine += "\t" + _columns[col];
            }
            writer.WriteLine(colLine);

            for (int row = 0;row<_rows.Count;++row)
            {
                colLine = _rows[row];
                for (int col = 0; col < _columns.Count; ++col)
                {
                    colLine += "\t" + _stats[row, col];
                }
                writer.WriteLine(colLine);
            }

            writer.Close();
        }
    }

    public class StatElement
    {
        public StatElement(int day)
        {
            Day = day;
            PropEff = 0;
            PropEarn = 0;
            ScoreEff = 0;
            ScoreEarn = 0;
            PPLNSEff = 0;
            PPLNSEarn = 0;
            SMPPSEarn = 0;
            TotalEff = 0;
            TotalEarn = 0;
        }

        public StatElement(int day, double propEff, double propEarn,
            double scoreEff, double scoreEarn,
            double pplnsEff, double pplnsEarn,
            double smppsEarn, double totalEff, double totalEarn, double totalHop)
        {
            Day = day;
            PropEff = propEff;
            PropEarn = propEarn;
            ScoreEff = scoreEff;
            ScoreEarn = scoreEarn;
            PPLNSEff = pplnsEff;
            PPLNSEarn = pplnsEarn;
            SMPPSEarn = smppsEarn;
            TotalEff = totalEff;
            TotalEarn = totalEarn;
            TotalHop = totalHop;
        }

        public int Day { get; set; }

        public double PropEff { get; set; }
        public double PropEarn { get; set; }

        public double ScoreEff { get; set; }
        public double ScoreEarn { get; set; }

        public double PPLNSEff { get; set; }
        public double PPLNSEarn { get; set; }

        public double SMPPSEarn { get; set; }

        public double TotalEff { get; set; }
        public double TotalEarn { get; set; }

        public double TotalHop { get; set; }
    }

    public class StatPoolElement
    {
        public string PoolName;
        public double MyTotalShare;
        public double TotalRound;
        public double Efficiency;
        public double Profit;

        private double _accumulated;

        public StatPoolElement(string poolName, double myTotalShare, int totalRound, double efficiency, double profit)
        {
            PoolName = poolName;
            MyTotalShare = myTotalShare;
            TotalRound = totalRound;
            Efficiency = efficiency;
            Profit = profit;
            _accumulated = 0;
        }

        public void AccumulatePoolValue(double myTotalShare, int totalRound, double efficiency, double profit)
        {
            MyTotalShare = (MyTotalShare*_accumulated + myTotalShare)/(_accumulated + 1);
            TotalRound = (TotalRound*_accumulated + totalRound)/(_accumulated + 1);
            Efficiency = (Efficiency*_accumulated + efficiency)/(_accumulated + 1);
            Profit = (Profit*_accumulated + profit)/(_accumulated + 1);

            _accumulated++;
        }
    }

    public class Stat
    {
        private StatElement[] _elements;
        private Dictionary<string, StatPoolElement> _poolElements;
        private int[] _dayAccumulated;

        public Stat(int maxDay)
        {
            _elements = new StatElement[maxDay];
            _dayAccumulated = new int[maxDay];

            for(int i=0;i<maxDay;++i)
            {
                _elements[i] = new StatElement(i);
                _dayAccumulated[i]=0;
            }

            _poolElements = new Dictionary<string, StatPoolElement>();
        }

        public void AddStat(StatElement e)
        {
            var day = e.Day;

            if (day >= _elements.Length || day < 0)
            {
                return;
            }
            
            var prior = _elements[day];
            _elements[day].PropEff = (prior.PropEff * _dayAccumulated[day] + e.PropEff) / (_dayAccumulated[day] + 1);
            _elements[day].PropEarn = (prior.PropEarn * _dayAccumulated[day] + e.PropEarn) / (_dayAccumulated[day] + 1);
            _elements[day].ScoreEff = (prior.ScoreEff * _dayAccumulated[day] + e.ScoreEff) / (_dayAccumulated[day] + 1);
            _elements[day].ScoreEarn = (prior.ScoreEarn * _dayAccumulated[day] + e.ScoreEarn) / (_dayAccumulated[day] + 1);
            _elements[day].PPLNSEff = (prior.PPLNSEff * _dayAccumulated[day] + e.PPLNSEff) / (_dayAccumulated[day] + 1);
            _elements[day].PPLNSEarn = (prior.PPLNSEarn * _dayAccumulated[day] + e.PPLNSEarn) / (_dayAccumulated[day] + 1);
            _elements[day].SMPPSEarn = (prior.SMPPSEarn * _dayAccumulated[day] + e.SMPPSEarn) / (_dayAccumulated[day] + 1);
            _elements[day].TotalEff = (prior.TotalEff * _dayAccumulated[day] + e.TotalEff) / (_dayAccumulated[day] + 1);
            _elements[day].TotalEarn = (prior.TotalEarn * _dayAccumulated[day] + e.TotalEarn) / (_dayAccumulated[day] + 1);
            _elements[day].TotalHop = (prior.TotalHop * _dayAccumulated[day] + e.TotalHop) / (_dayAccumulated[day] + 1);

            _dayAccumulated[day] = _dayAccumulated[day] + 1;
        }

        public void AddPoolStat(string poolName, double myTotalShare, int totalRound, double efficiency, double profit)
        {
            if(_poolElements.ContainsKey(poolName))
            {
                _poolElements[poolName].AccumulatePoolValue(myTotalShare, totalRound, efficiency, profit);
            }
            else
            {
                _poolElements.Add(poolName, new StatPoolElement(poolName, myTotalShare, totalRound, efficiency, profit));
            }
        }

        public void Dump(string generationTitle, bool toAddSimulationTime)
        {
            string strSaveFilePath = "StatDump_" + generationTitle + ".txt";
            if (toAddSimulationTime)
            {
                strSaveFilePath = "StatDump_" + generationTitle + string.Format("_{0:yyyyMMMdd-HHmmss}", DateTime.Now) + ".txt";
            }
            var writer = new StreamWriter(strSaveFilePath, false, System.Text.Encoding.UTF8);

            writer.WriteLine("Pool\tMyShare(Total)\tTotalRound\tEfficiency\tProfit");

            foreach (var p in _poolElements)
            {
                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", p.Key, (int)p.Value.MyTotalShare, (int)p.Value.TotalRound, p.Value.Efficiency, p.Value.Profit));
            }

            writer.WriteLine("\n");

            writer.WriteLine("day\tprop eff\tprop earn\tscore eff\tscore earn\tpplns eff\tpplns earn\tpps earn\ttot eff\ttot earn\tHops");

            var maxAccumulation = 0;
            foreach (var acc in _dayAccumulated.Where(acc => maxAccumulation < acc))
            {
                maxAccumulation = acc;
            }

            for (int i = 0; i < _elements.Length;++i )
            {
                if (_dayAccumulated[i] < maxAccumulation * 0.5) // skip unstable result
                    continue;

                var e = _elements[i];
                var line = string.Format(
                        "{0}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}\t{9:0.0000}\t{10:0.0}",
                        e.Day, e.PropEff, e.PropEarn, e.ScoreEff, e.ScoreEarn, e.PPLNSEff, e.PPLNSEarn, e.SMPPSEarn,
                        e.TotalEff, e.TotalEarn, e.TotalHop
                        );

                writer.WriteLine(line);
            }

            writer.Close();
        }
    }
}