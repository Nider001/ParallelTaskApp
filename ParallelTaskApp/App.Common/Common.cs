using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTaskApp.App.Common
{
    public struct RainfallDataRow
    {
        public string Subdivision { get; set; }
        public int Year { get; set; }

        public double VolumeJune { get; set; }
        public double VolumeJuly { get; set; }
        public double VolumeAugust { get; set; }
        public double VolumeSeptember { get; set; }
        public double VolumeTotal { get; set; }

        public RainfallDataRow(string subdivision, int year, double volumeJune, double volumeJuly, double volumeAugust, double volumeSeptember, double volumeTotal)
        {
            Subdivision = subdivision;
            Year = year;
            VolumeJune = volumeJune;
            VolumeJuly = volumeJuly;
            VolumeAugust = volumeAugust;
            VolumeSeptember = volumeSeptember;
            VolumeTotal = volumeTotal;
        }
    }
}
