using System.Collections.Generic;

namespace SimHopper
{
    /*
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
     */

    public delegate IHopStrategy NewGenerationHandler(int generation);

    public interface ISimConfig
    {
        int Difficulty { get; }
        int MaxSimulationDay { get; }
        int MaxSimulationRound { get; }
        int MaxSimulationGeneration { get; }
        int InitialSimulationSpeedUp { get; }

        Dictionary<string, PoolServer> Servers { get; }
        IHopStrategy Strategy { get; }

        void InitializeServers();
        string SetupGeneration(int generation);
    }
}