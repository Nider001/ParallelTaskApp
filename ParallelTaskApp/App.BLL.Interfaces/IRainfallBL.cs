using System.Collections.Generic;
using ParallelTaskApp.App.Common;

namespace ParallelTaskApp.App.BL.Interfaces
{
    public interface IRainfallBL
    {
        Dictionary<string, double> GetAveragesBySubdivision();
        Dictionary<int, double> GetAveragesByYear();
        KeyValuePair<int, int> GetMaxAboveAverageYear(bool parallel);
        KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>> GetMinMaxVolumes(bool parallel);
        Dictionary<string, double> GetSubdivisionsByVolume(bool parallel);
    }
}