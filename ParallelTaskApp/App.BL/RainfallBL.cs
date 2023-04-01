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
                        target = data.Aggregate((x, y) => x.VolumeJune < y.VolumeJune ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeJune > y.VolumeJune ? x : y);
                    res = target.VolumeJune;
                    break;
                case "July":
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeJuly < y.VolumeJuly ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeJuly > y.VolumeJuly ? x : y);
                    res = target.VolumeJuly;
                    break;
                case "August":
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeAugust < y.VolumeAugust ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeAugust > y.VolumeAugust ? x : y);
                    res = target.VolumeAugust;
                    break;
                default:
                    if (min)
                        target = data.Aggregate((x, y) => x.VolumeSeptember < y.VolumeSeptember ? x : y);
                    else
                        target = data.Aggregate((x, y) => x.VolumeSeptember > y.VolumeSeptember ? x : y);
                    res = target.VolumeSeptember;
                    break;
            }

            return new KeyValuePair<double, RainfallDataRow>(res, target);
        }

        private KeyValuePair<double, RainfallDataRow> GetTopVolumeLinear(bool min)
        {
            string[] columns = new string[] { "June", "July", "August", "September" };
            KeyValuePair<double, RainfallDataRow> res;

            var data = rainfallDA.ExtractData();

            if (min)
            {
                res = new KeyValuePair<double, RainfallDataRow>(double.MaxValue, new RainfallDataRow());

                foreach (var month in columns)
                {
                    var temp = FindTopInColumn(data, month, min);

                    if (res.Key > temp.Key)
                        res = temp;
                }
            }
            else
            {
                res = new KeyValuePair<double, RainfallDataRow>(double.MinValue, new RainfallDataRow());

                foreach (var month in columns)
                {
                    var temp = FindTopInColumn(data, month, min);

                    if (res.Key < temp.Key)
                        res = temp;
                }
            }

            return res;
        }

        private KeyValuePair<double, RainfallDataRow> GetTopVolumeParallel(bool min)
        {
            string[] columns = new string[] { "June", "July", "August", "September" };
            KeyValuePair<double, RainfallDataRow> res;

            var data = rainfallDA.ExtractData();

            object locker = new object();

            if (min)
            {
                res = new KeyValuePair<double, RainfallDataRow>(double.MaxValue, new RainfallDataRow());

                Parallel.ForEach(columns, month =>
                {
                    var temp = FindTopInColumn(data, month, min);

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
                    var temp = FindTopInColumn(data, month, min);

                    lock (locker)
                    {
                        if (res.Key < temp.Key)
                            res = temp;
                    }
                });
            }

            return res;
        }

        public KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>> GetMinMaxVolumesLinear()
        {
            double minValue = 0, maxValue = 0;
            RainfallDataRow minRow = new RainfallDataRow(), maxRow = new RainfallDataRow();

            var temp = GetTopVolumeLinear(true);
            minValue = temp.Key;
            minRow = temp.Value;

            temp = GetTopVolumeLinear(false);
            maxValue = temp.Key;
            maxRow = temp.Value;

            return new KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>>(
                new KeyValuePair<double, RainfallDataRow>(minValue, minRow),
                new KeyValuePair<double, RainfallDataRow>(maxValue, maxRow));
        }

        public KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>> GetMinMaxVolumesParallel()
        {
            double minValue = 0, maxValue = 0;
            RainfallDataRow minRow = new RainfallDataRow(), maxRow = new RainfallDataRow();

            Parallel.Invoke(
                () =>
                {
                    var temp = GetTopVolumeParallel(true);
                    minValue = temp.Key;
                    minRow = temp.Value;
                },
                () =>
                {
                    var temp = GetTopVolumeParallel(false);
                    maxValue = temp.Key;
                    maxRow = temp.Value;
                }
            );

            return new KeyValuePair<KeyValuePair<double, RainfallDataRow>, KeyValuePair<double, RainfallDataRow>>(
                new KeyValuePair<double, RainfallDataRow>(minValue, minRow),
                new KeyValuePair<double, RainfallDataRow>(maxValue, maxRow));
        }

        public Dictionary<int, double> GetAveragesByYear()
        {
            return rainfallDA.GetAveragesByYear();
        }

        public Dictionary<string, double> GetAveragesBySubdivision()
        {
            return rainfallDA.GetAveragesBySubdivision();
        }

        public KeyValuePair<int, int> GetMaxAboveAverageYearLinear()
        {
            int resYear = -1;
            int resCount = -1;

            Dictionary<int, double> averageVolumesByYear = rainfallDA.GetAveragesByYear();
            Dictionary<string, List<RainfallDataRow>> dataBySubdivision = rainfallDA.ExtractDataBySubdivision();

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

            return new KeyValuePair<int, int>(resYear, resCount);
        }

        public KeyValuePair<int, int> GetMaxAboveAverageYearParallel()
        {
            int resYear = -1;
            int resCount = -1;

            Dictionary<int, double> averageVolumesByYear = new Dictionary<int, double>();
            Dictionary<string, List<RainfallDataRow>> dataBySubdivision = new Dictionary<string, List<RainfallDataRow>>();

            Parallel.Invoke(
                () =>
                {
                    averageVolumesByYear = rainfallDA.GetAveragesByYear();
                },
                () =>
                {
                    dataBySubdivision = rainfallDA.ExtractDataBySubdivision();
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

            return new KeyValuePair<int, int>(resYear, resCount);
        }

        public Dictionary<string, double> GetSubdivisionsByVolume()
        {
            Dictionary<string, List<RainfallDataRow>> data;

            data = rainfallDA.ExtractDataBySubdivision();

            return data.Select(x => new KeyValuePair<string, double>(x.Key, x.Value.Sum(y => y.VolumeTotal))).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
