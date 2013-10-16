using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestKalmanFilter
{
    using System.Windows.Forms.DataVisualization.Charting;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.TestFilter();
        }

        private double sigmaPsi = 5d;
        private double sigmaEta = 50d;

        public void TestFilter()
        {
            
            var a = 0.1;
            var N = 100;
            var values = new double[N];
            var modelValues = new double[N];
            var realValues = new double[N];

            var val = 0d;
            var rnd = new Random();

            chart1.Series.Clear();

            for (int i = 0; i < N; i++)
            {
                modelValues[i] = a * (i * i);
                var rVal = modelValues[i] + (rnd.NextDouble() * 2 * sigmaPsi) - sigmaPsi;
                realValues[i] = rVal;
                val = rVal + (rnd.NextDouble() * 2 * sigmaEta) - sigmaEta;
                values[i] = val;
            }
            var res = this.Filter(values, modelValues);

            this.AddChart("Real Values", realValues, Color.CadetBlue, chart1.ChartAreas[0]);
            this.AddChart("Raw Values", values, Color.Brown, chart1.ChartAreas[0]);
            this.AddChart("Processed Values", res, Color.DarkGreen, chart1.ChartAreas[0]);

            this.AddChart("e Opt", eOpt, Color.DarkMagenta, chart1.ChartAreas[1]);
            this.AddChart("K", K, Color.DarkGoldenrod, chart1.ChartAreas[1]);

        }

        double[] eOpt;
        double[] xOpt;
        double[] K;

        public double[] Filter(double[] values, double[] modelValues)
        {
            eOpt = new double[values.Length];
            xOpt = new double[values.Length];
            K = new double[values.Length];


            var s = 0d; //СКО 
            for (int i = 1; i < values.Length; i++)
            {
                var val = values[i];
                var mVal = modelValues[i];
                var err = Math.Abs(val - mVal);
                s = (s * (i - 1) + err) / i;
            }
            var sigmaEtaCalc = s;
            var sigmaPsiCalc = 1d;

            xOpt[0] = values[0];
            eOpt[0] = sigmaEtaCalc;
            for (int t = 0; t < values.Length - 1; t++)
            {
                //eOpt[t + 1] =
                //    Math.Sqrt( 
                //    sigmaEtaCalc.pow(2) * (eOpt[t].pow(2) + sigmaPsiCalc.pow(2)) / 
                //    (sigmaEtaCalc.pow(2) + eOpt[t].pow(2) + sigmaPsiCalc.pow(2))
                //    );
                eOpt[t + 1] = Math.Sqrt( sigmaEtaCalc.pow(2) * eOpt[t].pow(2) / (sigmaEtaCalc.pow(2) + eOpt[t].pow(2)) );
                K[t + 1] = eOpt[t + 1].pow(2) / sigmaEtaCalc.pow(2);
                xOpt[t + 1] = (modelValues[t + 1]) * (1 - K[t + 1]) + K[t + 1] * values[t + 1];
            }
            return xOpt;
        }

        public void AddChart(string name, double[] val, Color color, ChartArea area)
        {
            var ser = new Series(name) {ChartArea  = area.Name, ChartType = SeriesChartType.Line, Color = color };
            for (int i = 0; i < val.Length; i++)
            {
                ser.Points.AddXY(i, val[i]);
            }
            chart1.Series.Add(ser);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].Visible = checkBox1.Checked;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[1].Visible = checkBox2.Checked;
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            HitTestResult result = chart1.HitTest(e.X, e.Y);
            if (result != null && result.Object != null)
            {
                // When user hits the LegendItem
                if (result.Object is LegendItem)
                {
                    // Legend item result
                    LegendItem legendItem = (LegendItem)result.Object;
                }
            }
        }
    }

    public static class powExtensions
    {
        public static double pow(this double val, double pow)
        {
            return Math.Pow(val, pow);
        }
    } 
}
