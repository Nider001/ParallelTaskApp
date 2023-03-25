using System.Collections.Generic;
using ParallelTaskApp.App.Common;

namespace ParallelTaskApp.App.BL.Interfaces
{
    public interface IRainfallBL
    {
        Dictionary<string, double> GetAveragesBySubdivision();
        Dictionary<int, double> GetAveragesByYear();
        KeyValuePair<int, int> GetMaxAboveAverageYearLinear();
        KeyValuePair<int, int> GetMaxAboveAverageYearParallel();
        KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>> GetMinMaxVolumesLinear();
        KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>> GetMinMaxVolumesParallel();
        Dictionary<string, double> GetSubdivisionsByVolume();
    }
}