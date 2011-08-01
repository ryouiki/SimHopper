using System;
using System.Collections.Generic;
using System.IO;

namespace SimHopper
{
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
            double smppsEarn, double totalEff, double totalEarn)
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
    }

    public class Stat
    {
        private StatElement[] _elements;
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

            _dayAccumulated[day] = _dayAccumulated[day] + 1;
        }

        public void Dump()
        {
            string strSaveFilePath = "StatDump.txt";
            var writer = new StreamWriter(strSaveFilePath, false, System.Text.Encoding.UTF8);
            
            foreach (var e in _elements)
            {
                var line = string.Format(
                    "{0}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}\t{9:0.0000}",
                    e.Day,e.PropEff,e.PropEarn, e.ScoreEff,e.ScoreEarn,e.PPLNSEff,e.PPLNSEarn, e.SMPPSEarn,
                    e.TotalEff,e.TotalEarn
                    );

                writer.WriteLine(line);
            }

            writer.Close();
        }
    }
}