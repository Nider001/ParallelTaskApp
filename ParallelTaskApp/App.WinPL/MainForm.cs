using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParallelTaskApp.App.BL;
using ParallelTaskApp.App.BL.Interfaces;
using ParallelTaskApp.App.DAL.Interfaces;

namespace ParallelTaskApp.App.WinPL
{
    public partial class MainForm : Form
    {
        private IRainfallBL rainfallBL;

        public MainForm()
        {
            LoginHandler loginHandler = new LoginHandler();

            if (!loginHandler.HandleLogin())
                Environment.Exit(0);

            rainfallBL = new RainfallBL();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void StartActionProcess()
        {
            textBox1.Clear();

            labelLinear.Text = "|: 00ms";
            labelParallel.Text = "||: 00ms";
        }

        private void ButtonMinMaxVolumes_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {
                var temp = rainfallBL.GetMinMaxVolumesParallel();
                var min = temp.Key;
                var max = temp.Value;

                string monthMin, monthMax;

                if (min.Key == min.Value.VolumeJune)
                    monthMin = "июнь";
                else if (min.Key == min.Value.VolumeJuly)
                    monthMin = "июль";
                else if (min.Key == min.Value.VolumeAugust)
                    monthMin = "август";
                else monthMin = "сентябрь";

                if (max.Key == max.Value.VolumeJune)
                    monthMax = "июнь";
                else if (max.Key == max.Value.VolumeJuly)
                    monthMax = "июль";
                else if (max.Key == max.Value.VolumeAugust)
                    monthMax = "август";
                else monthMax = "сентябрь";

                textBox1.Text += "Минимум: " + min.Key + " (" + min.Value.Subdivision + ", " + monthMin + " " + min.Value.Year + ")" + Environment.NewLine;
                textBox1.Text += "Максимум: " + max.Key + " (" + max.Value.Subdivision + ", " + monthMax + " " + max.Value.Year + ")";
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var temp = rainfallBL.GetMinMaxVolumesLinear();
                var min = temp.Key;
                var max = temp.Value;

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                var temp2 = rainfallBL.GetMinMaxVolumesParallel();
                var min2 = temp2.Key;
                var max2 = temp2.Value;

                watch.Stop();

                string monthMin, monthMax;

                if (min2.Key == min2.Value.VolumeJune)
                    monthMin = "июнь";
                else if (min2.Key == min2.Value.VolumeJuly)
                    monthMin = "июль";
                else if (min2.Key == min2.Value.VolumeAugust)
                    monthMin = "август";
                else monthMin = "сентябрь";

                if (max2.Key == max2.Value.VolumeJune)
                    monthMax = "июнь";
                else if (max2.Key == max2.Value.VolumeJuly)
                    monthMax = "июль";
                else if (max2.Key == max2.Value.VolumeAugust)
                    monthMax = "август";
                else monthMax = "сентябрь";

                textBox1.Text += "Минимум: " + min2.Key + " (" + min2.Value.Subdivision + ", " + monthMin + " " + min2.Value.Year + ")" + Environment.NewLine;
                textBox1.Text += "Максимум: " + max2.Key + " (" + max2.Value.Subdivision + ", " + monthMax + " " + max2.Value.Year + ")";

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void ButtonAverageVolumesByYear_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = new SortedList<int, double>(rainfallBL.GetAveragesByYear());

            foreach(var item in data)
                textBox1.Text += item.Key + " - " + Math.Round(item.Value, 1) + Environment.NewLine;
        }

        private void ButtonAverageVolumesBySubdivision_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = new SortedList<string, double>(rainfallBL.GetAveragesBySubdivision());

            foreach (var item in data)
                textBox1.Text += item.Key + " - " + Math.Round(item.Value, 1) + Environment.NewLine;
        }

        private void ButtonGetMaxAboveAverageYear_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {
                KeyValuePair<int, int> res = rainfallBL.GetMaxAboveAverageYearParallel();
                textBox1.Text += res.Key + " (" + res.Value + " районов)";
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                KeyValuePair<int, int> res = rainfallBL.GetMaxAboveAverageYearLinear();

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                KeyValuePair<int, int> res2 = rainfallBL.GetMaxAboveAverageYearParallel();

                watch.Stop();

                textBox1.Text += res2.Key + " (" + res2.Value + " районов)";

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void ButtonGetMostQuietSubdivisions_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = rainfallBL.GetSubdivisionsByVolume().OrderBy(x => x.Value);

            foreach (var row in data)
            {
                textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
            }
        }

        private void ButtonGetLeastQuietSubdivisions_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = rainfallBL.GetSubdivisionsByVolume().OrderByDescending(x => x.Value);

            foreach (var row in data)
            {
                textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
            }
        }

        private void CheckBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDebug.Checked)
            {
                labelLinear.Show();
                labelParallel.Show();
            }
            else
            {
                labelLinear.Hide();
                labelParallel.Hide();
            }
        }
    }
}
