using System;
using System.Collections.Generic;
using System.Diagnostics;
using MTRand;

namespace SimHopper
{
    public enum PoolType
    {
        Prop,
        PropEarlyHop,
        Pplns,
        Smpps,
        Score
    }

    public delegate int GetTargetShareHandler();

    public class RoundResult
    {
        public string PoolName { get; private set; }
        public int FinalShare { get; private set; }
        public int ValidShare { get; private set; }
        public int LostShare { get; private set; }
        public PoolType Type { get; private set; }
        public int RoundTime { get; private set; }

        public double Profit { get; private set; }

        public RoundResult(string poolName, PoolType type, int finalShare, int validShare, int lostShare, int roundTime)
        {
            PoolName = poolName;
            Type = type;
            FinalShare = finalShare;
            ValidShare = validShare;
            LostShare = lostShare;
            RoundTime = roundTime;
            switch (type)
            {
                case PoolType.Prop:
                case PoolType.PropEarlyHop:
                    Profit = validShare * 50.0f / FinalShare;
                    break;
                case PoolType.Pplns:
                    var f = (int) (FinalShare/2);
                    Profit = f > 0 ? ValidShare*50.0f/f : 0;
                    break;
                case PoolType.Score:
                    Profit = ValidShare * 50.0f / FinalShare;
                    break;
            }
        }

    }

    public class PoolServer
    {
        public string PoolName { get; private set; }
        public double CurrentRealShare { get; private set; }
        public int DelaySec { get; private set; }
        public int Round { get; private set; }
        public int TargetRoundShare { get; set; }
        public double MyValidShare { get; set; }
        public double MyLostShare { get; set; }
        private int _ghashSpeed;
        public int GHashSpeed { get { return _ghashSpeed; } set { _ghashSpeed = value; SharePerSec = value*0.25; } }
        public double SharePerSec { get; private set; }
        public double RejectPercentile { get; private set; }

        public float BaseProgress { get; set; }
        public float PenaltyFactor { get; set; }

        public double TotalScore { get; private set; }
        public double MyScore { get; private set; }

        public double MyTotalProfit { get; private set; }
        public double MyTotalValidShare { get; private set; }
        public double MyTotalShare { get; private set; }    // including lost

        public double CurrentShare
        {
            get
            {
                return RoundTime * SharePerSec;
            }
        }

        public int RoundTime
        {
            get
            {
                var time = RealRoundTime - DelaySec;
                for(int i=0;time<0;++i)
                {
                    if(i>=_lastTargetRoundShares.Capacity)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    var lastRoundTime = _lastTargetRoundShares[i] / SharePerSec;
                    time += (int)lastRoundTime;
                }
                
                return time;
            }
        }

        public int RealRoundTime
        {
            get
            {
                return (int)(CurrentRealShare / SharePerSec);
            }
        }
        public PoolType Type { get; private set; }

        private int _forwardMyShare=0;
        private int _forwardRoundShare=0;

        private List<int> _lastTargetRoundShares = new List<int>(32);

        private MersenneTwister _rnd;

        private GetTargetShareHandler GetNextShare;

        public PoolServer(string poolName, PoolType type, int speed, int delaySec, double rejectPercent, GetTargetShareHandler handler)
        {
            _rnd = new MersenneTwister((uint)DateTime.Now.Ticks);
            Round = 1;
            PoolName = poolName;
            Type = type;
            GHashSpeed = speed;
            DelaySec = delaySec;
            GetNextShare = handler;
            RejectPercentile = rejectPercent;
            CurrentRealShare = 0;
            MyValidShare = 0;
            MyLostShare = 0;
            TotalScore = 0;
            MyScore = 0;

            MyTotalProfit = 0;
            MyTotalValidShare = 0;
            MyTotalShare = 0;

            BaseProgress = 0;
            PenaltyFactor = 1.0f;

//            Initialize(initialProgress);
        }

        public void Initialize(double initialProgress)
        {
            Round = 1;
            MyValidShare = 0;
            MyLostShare = 0;           
            TotalScore = 0;
            MyScore = 0;
            _forwardMyShare = 0;
            _forwardRoundShare = 0;

            MyTotalProfit = 0;
            MyTotalValidShare = 0;
            MyTotalShare = 0;

            _lastTargetRoundShares.Clear();
            for (int i = 0; i < _lastTargetRoundShares.Capacity;++i )
            {
                _lastTargetRoundShares.Add(GetNextShare());
            }

            TargetRoundShare = GetNextShare();

            if (initialProgress < 0)
            {
                initialProgress = (double)_rnd.NextDouble();
            }

            CurrentRealShare = (int)(initialProgress * TargetRoundShare);
        }

        public RoundResult Advance(int seconds, double myShare)
        {
            RoundResult result = null;
            double shareIncrease = SharePerSec * seconds;
            CurrentRealShare += shareIncrease + _forwardRoundShare;
            myShare += _forwardMyShare;
            _forwardRoundShare = 0;
            _forwardMyShare = 0;

            if (myShare < 0 || MyValidShare < 0)
            {
                throw new Exception();
            }
            //Debug.WriteLine(String.Format("out {0} - {1} - {2}", CurrentRealShare, TargetRoundShare, myShare));

            if(Type!=PoolType.Smpps)
            {
                var cutoff = (CurrentRealShare - TargetRoundShare) / shareIncrease;
                if (cutoff > 0)
                {
                    _forwardMyShare = (int)(cutoff * myShare);
                    if (_forwardMyShare > myShare)
                    {
                        _forwardMyShare = (int)myShare;
                    }
                    _forwardRoundShare = (int)(cutoff * shareIncrease);

                    myShare -= _forwardMyShare;
                    CurrentRealShare = TargetRoundShare;
                }
            }

            var myLost = RejectPercentile * 0.01f * myShare;
            var myValid = myShare - myLost;

            if (myShare < 0 || MyValidShare< 0)
            {
                throw new Exception();
            }

            switch (Type)
            {
                case PoolType.Pplns:
                    if (CurrentRealShare > TargetRoundShare / 2.0)
                    {
                        MyValidShare += myValid;
                        MyLostShare += myLost;
                    }
                    else
                    {
                        MyLostShare += myShare;
                    }
                    break;
                case PoolType.Prop:
                case PoolType.PropEarlyHop:
                case PoolType.Smpps:
                    MyValidShare += myValid;
                    MyLostShare += myLost;
                    break;
                case PoolType.Score:
                    TotalScore += shareIncrease * Math.Exp(RealRoundTime / 300.0);
                    MyScore += myValid * Math.Exp((double)RealRoundTime / 300.0);
                    MyValidShare += myValid;
                    MyLostShare += myLost;
                    break;
            }

            if (MyValidShare < 0)
            {
                throw new Exception();
            }

            if (Type != PoolType.Smpps &&
                (int)CurrentRealShare >= TargetRoundShare)
            {
                // block Found !!!

                if(Type==PoolType.Score)
                {
                    var validShare = (int)( (MyScore/TotalScore)*TargetRoundShare );
                    var lostShare = (int) (MyValidShare - validShare);

                    result = new RoundResult(PoolName, Type, TargetRoundShare, validShare, lostShare, RealRoundTime);
                }
                else
                {
                    result = new RoundResult(PoolName, Type, TargetRoundShare, (int)MyValidShare, (int)MyLostShare, RealRoundTime);
                }

                MyTotalProfit += result.Profit;
                MyTotalValidShare += result.ValidShare;
                MyTotalShare += result.ValidShare + result.LostShare;
                
                MyValidShare = 0;
                MyLostShare = 0;

                _lastTargetRoundShares.RemoveAt(_lastTargetRoundShares.Capacity-1);
                _lastTargetRoundShares.Insert(0,TargetRoundShare);

                TargetRoundShare = GetNextShare();
                TotalScore = 0;
                MyScore = 0;

                CurrentRealShare = 0;

                Round++;
            }

            return result;
        }
    }
}