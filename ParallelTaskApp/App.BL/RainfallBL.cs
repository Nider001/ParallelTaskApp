using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParallelTaskApp.App.BL.Interfaces;
using ParallelTaskApp.App.Common;
using ParallelTaskApp.App.DAL;
using ParallelTaskApp.App.DAL.Interfaces;

namespace ParallelTaskApp.App.BL
{
    public class RainfallBL : IRainfallBL
    {
        private IRainfallDA rainfallDA;

        public RainfallBL()
        {
            rainfallDA = new RainfallDA();
        }

        private KeyValuePair<double, RainfallDataRow> FindTopInColumn(List<RainfallDataRow> data, string column, bool min)
        {
            double res = 0.0;
            RainfallDataRow target = new RainfallDataRow();

            switch (column)
            {
                case "June":
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeJune < y.VolumeJune || y.VolumeJune < 0 ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeJune > y.VolumeJune ? x : y);
                    res = target.VolumeJune;
                    break;
                case "July":
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeJuly < y.VolumeJuly || y.VolumeJuly < 0 ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeJuly > y.VolumeJuly ? x : y);
                    res = target.VolumeJuly;
                    break;
                case "August":
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeAugust < y.VolumeAugust || y.VolumeAugust < 0 ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeAugust > y.VolumeAugust ? x : y);
                    res = target.VolumeAugust;
                    break;
                default:
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeTotal < y.VolumeTotal || y.VolumeTotal < 0 ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeTotal > y.VolumeTotal ? x : y);
                    res = target.VolumeTotal;
                    break;
            }

            return new KeyValuePair<double, RainfallDataRow>(res, target);
        }

        public KeyValuePair<double, RainfallDataRow> GetTopVolume(bool min, bool parallel)
        {
            string[] columns = new string[] { "June", "July", "August", "Total" };
            KeyValuePair<double, RainfallDataRow> res;

            var data = rainfallDA.ExtractData();

            if (parallel)
            {
                object locker = new object();

                if (min)
                {
                    res = new KeyValuePair<double, RainfallDataRow>(double.MaxValue, new RainfallDataRow());

                    Parallel.ForEach(columns, month =>
                    {
                        var temp = FindTopInColumn(data, month, true);

                        lock (locker)
                        {
                            if (res.Key > temp.Key)
                                res = temp;
                        }
                    });
                }
                else
                {
                    res = new KeyValuePair<double, RainfallDataRow>(double.MinValue, new RainfallDataRow());

                    Parallel.ForEach(columns, month =>
                    {
                        var temp = FindTopInColumn(data, month, false);

                        lock (locker)
                        {
                            if (res.Key < temp.Key)
                                res = temp;
                        }
                    });
                }
            }
            else
            {
                if (min)
                {
                    res = new KeyValuePair<double, RainfallDataRow>(double.MaxValue, new RainfallDataRow());

                    foreach (var month in columns)
                    {
                        var temp = FindTopInColumn(data, month, true);

                        if (res.Key > temp.Key)
                            res = temp;
                    }
                }
                else
                {
                    res = new KeyValuePair<double, RainfallDataRow>(double.MinValue, new RainfallDataRow());

                    foreach (var month in columns)
                    {
                        var temp = FindTopInColumn(data, month, false);

                        if (res.Key < temp.Key)
                            res = temp;
                    }
                }
            }

            return res;
        }

        public Dictionary<int, double> GetAveragesByYear()
        {
            return rainfallDA.GetAveragesByYear();
        }

        public Dictionary<string, double> GetAveragesBySubdivision()
        {
            return rainfallDA.GetAveragesBySubdivision();
        }

        public KeyValuePair<int, int> GetMaxAboveAverageYear(bool parallel)
        {
            int resYear = -1;
            int resCount = -1;

            Dictionary<int, double> averageVolumesByYear = new Dictionary<int, double>();
            Dictionary<string, List<RainfallDataRow>> dataBySubdivision = new Dictionary<string, List<RainfallDataRow>>();

            if (parallel)
            {
                Parallel.Invoke(
                    () =>
                    {
                        averageVolumesByYear = rainfallDA.GetAveragesByYear();
                    },
                    () =>
                    {
                        dataBySubdivision = rainfallDA.ExtractDataBySubdivision(true);
                    }
                );

                object locker = new object();

                Parallel.ForEach(averageVolumesByYear.Keys, year =>
                {
                    int aboveAverages = 0;

                    foreach (var subdivision in dataBySubdivision.Keys)
                    {
                        if (dataBySubdivision[subdivision].Any(x => x.Year == year) && dataBySubdivision[subdivision].First(x => x.Year == year).VolumeTotal > averageVolumesByYear[year])
                        {
                            aboveAverages++;
                        }
                    }

                    lock (locker)
                    {
                        if (aboveAverages > resCount)
                        {
                            resYear = year;
                            resCount = aboveAverages;
                        }
                    }
                });
            }
            else
            {
                averageVolumesByYear = rainfallDA.GetAveragesByYear();
                dataBySubdivision = rainfallDA.ExtractDataBySubdivision(false);

                foreach (int year in averageVolumesByYear.Keys)
                {
                    int aboveAverages = 0;

                    foreach (var subdivision in dataBySubdivision.Keys)
                    {
                        if (dataBySubdivision[subdivision].Any(x => x.Year == year) && dataBySubdivision[subdivision].First(x => x.Year == year).VolumeTotal > averageVolumesByYear[year])
                        {
                            aboveAverages++;
                        }
                    }

                    if (aboveAverages > resCount)
                    {
                        resYear = year;
                        resCount = aboveAverages;
                    }
                }
            }

            return new KeyValuePair<int, int>(resYear, resCount);
        }

        public Dictionary<string, double> GetSubdivisionsByVolume(bool parallel)
        {
            Dictionary<string, List<RainfallDataRow>> data;

            if (parallel)
                data = rainfallDA.ExtractDataBySubdivision(true);
            else
                data = rainfallDA.ExtractDataBySubdivision(false);

            return data.Select(x => new KeyValuePair<string, double>(x.Key, x.Value.Sum(y => y.VolumeTotal))).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
