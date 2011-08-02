using System;
using MTRand;

namespace SimHopper
{
    public enum PoolType
    {
        Prop,
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

        public float Profit { get; private set; }

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
                    Profit = (float)ValidShare * 50.0f / (float)FinalShare;
                    break;
                case PoolType.Pplns:
                    var f = (int) (FinalShare/2);
                    Profit = f > 0 ? (float) ValidShare*50.0f/f : 0;
                    break;
                case PoolType.Score:
                    Profit = (float)ValidShare * 50.0f / (float)FinalShare;
                    break;
            }
        }

    }

    public class PoolServer
    {
        public string PoolName { get; private set; }
        public float CurrentShare { get; private set; }
        public float CurrentRealShare { get; private set; }
        public int DelaySec { get; private set; }
        public int Round { get; private set; }
        public int TargetRoundShare { get; set; }
        public float MyValidShare { get; set; }
        public float MyLostShare { get; set; }
        public int GHashSpeed { get; private set; }
        public float RejectPercentile { get; private set; }

        public double TotalScore { get; private set; }
        public double MyScore { get; private set; }

        public int RoundTime { get ; private set; }
        public int RealRoundTime { get; private set; }
        public PoolType Type { get; private set; }

        public int _delayRemain = -1;

        private MersenneTwister _rnd;

        private GetTargetShareHandler GetNextShare;

        public PoolServer(string poolName, PoolType type, int speed, float initialProgress, int delaySec, float rejectPercent, GetTargetShareHandler handler)
        {
            _rnd = new MersenneTwister((uint)DateTime.Now.Ticks);
            Round = 1;
            PoolName = poolName;
            Type = type;
            GHashSpeed = speed;
            DelaySec = delaySec;
            GetNextShare = handler;
            RejectPercentile = rejectPercent;
            CurrentShare = 0;
            CurrentRealShare = 0;
            MyValidShare = 0;
            MyLostShare = 0;
            RoundTime = 0;
            RealRoundTime = 0;
            TotalScore = 0;
            MyScore = 0;

            TargetRoundShare = GetNextShare();

            if ( initialProgress < 0)
            {
                initialProgress = (float)_rnd.NextDouble();
            }

            CurrentRealShare = (int)(initialProgress*TargetRoundShare);
            CurrentShare = CurrentRealShare;
        }

        public RoundResult Advance(int seconds, float myShare)
        {
            RoundResult result = null;
            RealRoundTime += seconds;
            float shareIncrease = ((float) GHashSpeed)*seconds/4.0f;
            CurrentRealShare += shareIncrease;

            var myLost = RejectPercentile * 0.01f * myShare;
            var myValid = myShare - myLost;

            _delayRemain -= seconds;
            if (_delayRemain<0)
            {
                CurrentShare = CurrentRealShare;
                RoundTime = RealRoundTime;
                _delayRemain = DelaySec;
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

            if (Type != PoolType.Smpps &&
                (int)CurrentRealShare > TargetRoundShare)
            {
                // block Found !!!
                if(Type==PoolType.Score)
                {
                    var validShare = (int)( (MyScore/TotalScore)*TargetRoundShare );
                    var lostShare = (int) (MyValidShare - validShare);

                    result = new RoundResult(PoolName, Type, TargetRoundShare, validShare, lostShare, RoundTime);
                }
                else
                {
                    result = new RoundResult(PoolName, Type, TargetRoundShare, (int)MyValidShare, (int)MyLostShare, RoundTime);
                }

                CurrentRealShare = 0;
                MyValidShare = 0;
                MyLostShare = 0;
                TargetRoundShare = GetNextShare();
                RealRoundTime = 0;
                TotalScore = 0;
                MyScore = 0;
                Round++;
            }

            return result;
        }
    }
}