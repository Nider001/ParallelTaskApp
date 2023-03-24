using System.Collections.Generic;
using ParallelTaskApp.App.Common;

namespace ParallelTaskApp.App.BL.Interfaces
{
    public interface IRainfallBL
    {
        Dictionary<string, double> GetAveragesBySubdivision();
        Dictionary<int, double> GetAveragesByYear();
        KeyValuePair<int, int> GetMaxAboveAverageYear(bool parallel);
        Dictionary<string, double> GetSubdivisionsByVolume(bool parallel);
        KeyValuePair<double, RainfallDataRow> GetTopVolume(bool min, bool parallel);
    }
}