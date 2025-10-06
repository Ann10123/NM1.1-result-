using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RegressionApp
{
    public partial class Form1 : Form
    {
        private DataGridView dataGrid;
        private NumericUpDown pointCountInput;
        private Button createButton, buildButton, predictButton;
        private PlotView plotView;
        private PlotModel plotModel;
        private TextBox xInput;
        private Label predictionLabel;

        private double k, b; 

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(255, 200, 150);
            this.Font = new Font("Arial", 10, FontStyle.Italic);
            InitUI();
        }

        private void InitUI()
        {
            this.Width = 950;
            this.Height = 650;
            this.Text = "Лінійна регресія";

            Label lbl = new Label
            {
                Text = "Кількість точок:",
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 40
            };
            this.Controls.Add(lbl);

            pointCountInput = new NumericUpDown
            {
                Left = 110,
                Top = 25,
                Width = 80,
                Minimum = 2,
                Maximum = 100
            };
            this.Controls.Add(pointCountInput);

            createButton = new Button
            {
                Text = "Створити таблицю",
                Left = 240,
                Top = 25,
                Width = 150
            };
            createButton.Click += CreateButton_Click;
            this.Controls.Add(createButton);

            buildButton = new Button
            {
                Text = "Побудувати графік",
                Left = 410,
                Top = 25,
                Width = 150
            };
            buildButton.Click += BuildButton_Click;
            this.Controls.Add(buildButton);

            Label xLbl = new Label
            {
                Text = "X для прогнозу:",
                Left = 600,
                Top = 25,
                Width = 80,
                Height = 35
            };
            this.Controls.Add(xLbl);

            xInput = new TextBox
            {
                Left = 680,
                Top = 27,
                Width = 80
            };
            this.Controls.Add(xInput);

            predictButton = new Button
            {
                Text = "Прогнозувати Y",
                Left = 770,
                Top = 27,
                Width = 130
            };
            predictButton.Click += PredictButton_Click;
            this.Controls.Add(predictButton);

            predictionLabel = new Label
            {
                Text = "Y = ",
                Left = 600,
                Top = 60,
                Width = 150
            };
            this.Controls.Add(predictionLabel);

            dataGrid = new DataGridView
            {
                Left = 10,
                Top = 120,
                Width = 243,
                Height = 250,
                BackgroundColor = Color.LightBlue,
                AllowUserToAddRows = false
            };
            dataGrid.Columns.Add("X", "X");
            dataGrid.Columns.Add("Y", "Y");

            this.Controls.Add(dataGrid);

            plotModel = new PlotModel { Title = "Linear Regression"};
            plotModel.TitleFont = "Arial";           
            plotModel.TitleFontSize = 16;            
            plotModel.TitleFontWeight = FontWeights.Bold;

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

            plotView = new PlotView
            {
                Left = 270,
                Top = 75,
                Width = 650,
                Height = 530,
                Model = plotModel
        };
            this.Controls.Add(plotView);
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            int n = (int)pointCountInput.Value;
            dataGrid.Rows.Clear();
            for (int i = 0; i < n; i++)
            {
                dataGrid.Rows.Add();
            }
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            try
            {
                int n = dataGrid.Rows.Count;
                double[] x = new double[n];
                double[] y = new double[n];

                for (int i = 0; i < n; i++)
                {
                    x[i] = Convert.ToDouble(dataGrid.Rows[i].Cells[0].Value);
                    y[i] = Convert.ToDouble(dataGrid.Rows[i].Cells[1].Value);
                }

                double sumX = x.Sum();
                double sumY = y.Sum();
                double sumXY = 0;
                double sumX2 = 0;

                for (int i = 0; i < n; i++)
                {
                    sumXY += x[i] * y[i];
                    sumX2 += x[i] * x[i];
                }

                k = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                b = (sumY - k * sumX) / n;

                string equation = k >= 0 ? $"y = {b:F2} + {k:F2}x" : $"y = {b:F2} - {Math.Abs(k):F2}x";

                var model = new PlotModel { Title = equation };

                var scatter = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red};
                for (int i = 0; i < n; i++)
                    scatter.Points.Add(new ScatterPoint(x[i], y[i]));
                model.Series.Add(scatter);

                var line = new LineSeries { Color = OxyColors.Black, StrokeThickness = 2 };
                double minX = x.Min();
                double maxX = x.Max();
                line.Points.Add(new DataPoint(minX, b + k * minX));
                line.Points.Add(new DataPoint(maxX, b + k * maxX));
                model.Series.Add(line);

                plotView.Model = model;
            }
            catch
            {
                MessageBox.Show("Будь ласка, введіть усі координати точок!");
            }
        }

        private void PredictButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(xInput.Text, out double xNew))
            {
                double yPred = b + k * xNew;
                predictionLabel.Text = $"Y = {yPred:F3}";
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть правильне число для X!");
            }
        }
    }
}
