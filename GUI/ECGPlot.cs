using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Axes;
using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.GUI
{
    class ECGPlot
    {
        public PlotModel CurrentPlot { get; set; }
        private int _windowSize;
        private int _beginingPoint;
        private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        private Basic_Data_Worker _ecg_Basic_Data_Worker;
        private bool first;



        //test
        public ECGPlot(string plotTitle)
        {
            CurrentPlot = new PlotModel();
            CurrentPlot.Title = plotTitle;
            //CurrentPlot.LegendTitle = "Legend";
            CurrentPlot.LegendOrientation = LegendOrientation.Horizontal;
            CurrentPlot.LegendPlacement = LegendPlacement.Outside;
            CurrentPlot.LegendPosition = LegendPosition.RightMiddle;
            _windowSize = 1000;
            _beginingPoint = 0;
            first = true;
            
            //CurrentPlot.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            //CurrentPlot.LegendBorder = OxyColors.Black;
        }

        public void DisplayAnything()
        {
            CurrentPlot.Series.Add(new FunctionSeries(Math.Cos, 0, 20, 0.1, "cos(x)"));
            CurrentPlot.Series.Add(new FunctionSeries(Math.Log, 0, 20, 0.1, "log(x)"));
            CurrentPlot.Subtitle = "Juz zmieniam na prawdziwy bez spiny :p";
            CurrentPlot.Annotations.Add(AddArrowAnnotation());
            var lineraYAxis = new LinearAxis();
            lineraYAxis.Position = AxisPosition.Left;
            lineraYAxis.Minimum = -1.25;
            lineraYAxis.Maximum = 1.25;
            lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
            lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
            lineraYAxis.Title = "Voltage [mV]";

            CurrentPlot.Axes.Add(lineraYAxis);
        }

        public void DisplaySin(int startPoint, int stopPoint)
        {

            CurrentPlot.Series.Add(new FunctionSeries(Math.Sin, startPoint, stopPoint, 0.01, "sin(x)"));

            //if (startPoint % 5 != 0)
            //{
            //    var series = new ScatterSeries();
            //    series.Points.Add(new ScatterPoint(startPoint + 10, 1));
            //    CurrentPlot.Series.Add(series);
            //}

        }

        public void DisplayBasicData()
        {

            //Basic_Data_Worker worker = new Basic_Data_Worker();
            //worker.Load();

            //foreach (var signal in worker.BasicData.Signals)
            //{

            //    Vector<double> signalVector = signal.Item2;
            //    LineSeries ls = new LineSeries();
            //    ls.Title = signal.Item1;

            //    for(int i=0; i<signalVector.Count; i++)
            //    {
            //        ls.Points.Add(new DataPoint(i, signalVector[i]));
            //    }


            //    CurrentPlot.Series.Add(ls);
            //}

            if (first)
            {
                _ecg_Basic_Data_Worker = new Basic_Data_Worker();
                _ecg_Basic_Data_Worker.Load();
                first = false;
                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                lineraYAxis.Minimum = -100.0;
                lineraYAxis.Maximum = 80.0;
                lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
                lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
                lineraYAxis.Title = "Voltage [mV]";

                CurrentPlot.Axes.Add(lineraYAxis);
            }
            else
            {
                ClearPlot();
            }

            foreach (var signal in _ecg_Basic_Data_Worker.BasicData.Signals)
            {

                Vector<double> signalVector = signal.Item2;
                LineSeries ls = new LineSeries();
                ls.Title = signal.Item1;

                ls.MarkerStrokeThickness = 1;


                for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i, signalVector[i]));
                }


                CurrentPlot.Series.Add(ls);


            }

            RefreshPlot();

        }

        public void ClearPlot()
        {
            CurrentPlot.Series.Clear();
        }

        public void RefreshPlot()
        {
            CurrentPlot.InvalidatePlot(true);
        }

        public void DisplayBasicSignal()
        {
            ClearPlot();

            //CurrentPlot.Series.Add();
        }

        public void DisplayHistogram()
        {
            ColumnSeries hist = new ColumnSeries();

            hist.Items.Add(new ColumnItem(5.23, 0));
            hist.Items.Add(new ColumnItem(10.23, 1));
            hist.Items.Add(new ColumnItem(11.23, 2));
            hist.Items.Add(new ColumnItem(11.23, 3));
            hist.Items.Add(new ColumnItem(11.23, 4));
            hist.Items.Add(new ColumnItem(11.23, 5));
            hist.Items.Add(new ColumnItem(15.23, 6));
            hist.Items.Add(new ColumnItem(8.3, 7));

            CurrentPlot.Series.Add(hist);
            

        }

        public void DisplayEcgBaseline()
        {
            if (first)
            {
                _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
                _ecg_Baseline_Data_worker.Load();
                first = false;
                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                lineraYAxis.Minimum = -100.0;
                lineraYAxis.Maximum = 80.0;
                lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
                lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
                lineraYAxis.Title = "Voltage [mV]";

                CurrentPlot.Axes.Add(lineraYAxis);
            }
            else
            {
                ClearPlot();
            }

            foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                LineSeries ls = new LineSeries();
                ls.Title = signal.Item1;

                ls.MarkerStrokeThickness = 1;


                for (int i = _beginingPoint; (i <= (_beginingPoint+_windowSize) && i< signalVector.Count()) ; i++)
                {
                    ls.Points.Add(new DataPoint(i, signalVector[i]));
                }


                CurrentPlot.Series.Add(ls);
                
                
            }

            RefreshPlot();

            //foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            //{

            //    Vector<double> signalVector = signal.Item2;
            //    LineSeries ls = new LineSeries();
            //    ls.Title = signal.Item1;

            //    ls.MarkerStrokeThickness = 1;


            //    for (int i = 0; i < signalVector.Count; i++)
            //    {
            //        ls.Points.Add(new DataPoint(i, signalVector[i]));
            //    }


            //    CurrentPlot.Series.Add(ls);
            //}

        }

        public void MovePlot(int amount)
        {
            _beginingPoint = _beginingPoint + amount;
            if(_beginingPoint<0)
            {
                _beginingPoint = 0;
            }
        }

        public ScatterSeries DisplayR_Peaks(double x, double y)
        {
            ScatterSeries series = new ScatterSeries();
            series.Points.Add(new ScatterPoint { X = x, Y = y, Size = 3 });
            return series;
        }


        //Annotations
        private ArrowAnnotation AddArrowAnnotation()
        {
            return new ArrowAnnotation
            {
                StartPoint = new DataPoint(10, 0),
                EndPoint = new DataPoint(1.66, 0.5),
                Text = "Point to somewhere"
            };
        }

        private EllipseAnnotation AddEllipseAnnotation()
        {
            return new EllipseAnnotation
            {
                Height = 2.1,
                Width = 2.5,
                Text = "ellipse",
                X = 10,
                Y = 0,
                Fill = OxyColors.WhiteSmoke
            };
        }

        private RectangleAnnotation AddRectangleAnnotation()
        {
            return new RectangleAnnotation
            {
                MaximumX = 15,
                MinimumX = 11,
                MaximumY = 0.5,
                MinimumY = 0.1,
            };
        }

    }
}