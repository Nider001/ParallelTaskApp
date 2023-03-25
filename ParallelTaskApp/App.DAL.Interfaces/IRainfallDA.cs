using System.Collections.Generic;
using ParallelTaskApp.App.Common;

namespace ParallelTaskApp.App.DAL.Interfaces
{
    public interface IRainfallDA
    {
        bool CheckSubdivision(string subdivision);
        List<RainfallDataRow> ExtractData();
        Dictionary<string, List<RainfallDataRow>> ExtractDataBySubdivision();
        Dictionary<int, List<RainfallDataRow>> ExtractDataByYear(int startYear, int endYear);
        List<string> GetAllSubdivisions();
        Dictionary<string, double> GetAveragesBySubdivision();
        Dictionary<int, double> GetAveragesByYear();
    }
}