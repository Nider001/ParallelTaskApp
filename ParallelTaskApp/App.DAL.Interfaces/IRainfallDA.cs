using System.Collections.Generic;
using ParallelTaskApp.App.Common;

namespace ParallelTaskApp.App.DAL.Interfaces
{
    public interface IRainfallDA
    {
        bool CheckSubdivision(string subdivision);
        List<RainfallDataRow> ExtractData();
        Dictionary<string, List<RainfallDataRow>> ExtractDataBySubdivision(bool parallel);
        Dictionary<int, List<RainfallDataRow>> ExtractDataByYear(bool parallel, int startYear, int endYear);
        List<string> GetAllSubdivisions();
        Dictionary<string, double> GetAveragesBySubdivision();
        Dictionary<int, double> GetAveragesByYear();
    }
}