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
            if (!LoginHandler.HandleLogin())
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

        private void button1_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {

                var min = rainfallBL.GetTopVolume(true, true);
                var max = rainfallBL.GetTopVolume(false, true);

                textBox1.Text += "Минимум: " + min.Key + " (" + min.Value.Subdivision + ", " + min.Value.Year + ")" + Environment.NewLine;
                textBox1.Text += "Максимум: " + max.Key + " (" + max.Value.Subdivision + ", " + max.Value.Year + ")";
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var min = rainfallBL.GetTopVolume(true, false);
                var max = rainfallBL.GetTopVolume(false, false);

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                var min2 = rainfallBL.GetTopVolume(true, true);
                var max2 = rainfallBL.GetTopVolume(false, true);

                watch.Stop();

                textBox1.Text += "Минимум: " + min2.Key + " (" + min2.Value.Subdivision + ", " + min2.Value.Year + ")" + Environment.NewLine;
                textBox1.Text += "Максимум: " + max2.Key + " (" + max2.Value.Subdivision + ", " + max2.Value.Year + ")";

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = new SortedList<int, double>(rainfallBL.GetAveragesByYear());

            foreach(var item in data)
                textBox1.Text += item.Key + " - " + Math.Round(item.Value, 1) + Environment.NewLine;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            var data = new SortedList<string, double>(rainfallBL.GetAveragesBySubdivision());

            foreach (var item in data)
                textBox1.Text += item.Key + " - " + Math.Round(item.Value, 1) + Environment.NewLine;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {
                KeyValuePair<int, int> res = rainfallBL.GetMaxAboveAverageYear(true);
                textBox1.Text += res.Key + " (" + res.Value + " районов)";
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                KeyValuePair<int, int> res = rainfallBL.GetMaxAboveAverageYear(false);

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                KeyValuePair<int, int> res2 = rainfallBL.GetMaxAboveAverageYear(true);

                watch.Stop();

                textBox1.Text += res2.Key + " (" + res2.Value + " районов)";

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {
                var data = rainfallBL.GetSubdivisionsByVolume(true).OrderBy(x => x.Value);

                foreach (var row in data)
                {
                    textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
                }
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var data = rainfallBL.GetSubdivisionsByVolume(false).OrderBy(x => x.Value);

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                var data2 = rainfallBL.GetSubdivisionsByVolume(true).OrderBy(x => x.Value);

                watch.Stop();

                foreach (var row in data2)
                {
                    textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
                }

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            StartActionProcess();

            if (!checkBoxDebug.Checked)
            {
                var data = rainfallBL.GetSubdivisionsByVolume(true).OrderByDescending(x => x.Value);

                foreach (var row in data)
                {
                    textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
                }
            }
            else
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                var data = rainfallBL.GetSubdivisionsByVolume(false).OrderByDescending(x => x.Value);

                watch.Stop();

                labelLinear.Text = "|: " + watch.ElapsedMilliseconds.ToString() + "ms";

                watch.Restart();

                var data2 = rainfallBL.GetSubdivisionsByVolume(true).OrderByDescending(x => x.Value);

                watch.Stop();

                foreach (var row in data2)
                {
                    textBox1.Text += row.Key + " - " + row.Value + Environment.NewLine;
                }

                labelParallel.Text = "||: " + watch.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
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
