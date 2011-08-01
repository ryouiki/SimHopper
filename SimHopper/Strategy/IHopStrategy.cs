using System.Collections.Generic;

namespace SimHopper
{
    public interface IHopStrategy
    {
        string GetBestPool(Dictionary<string, PoolServer> pools, string currentPool, int advancedSeconds);
    }
}