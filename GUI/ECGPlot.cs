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
using EKG_Project.Modules;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.Modules.QT_Disp;

namespace EKG_Project.GUI
{
    class ECGPlot
    {
        public PlotModel CurrentPlot { get; set; }
        private double _windowSize;
        private double _maxSeriesTime; 
        private int _beginingPoint;
        private int _scalingPlotValue; 
        private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        private Basic_Data_Worker _ecg_Basic_Data_Worker;
        private R_Peaks_Data_Worker _r_Peaks_Data_Worker;
        private bool first;
        private bool _visible;
        private double _analyseFrequency;
        private double _analyseSamples;

        private R_Peaks_Data _r_PeaksData;
        private ECG_Baseline_Data _ecg_Baseline_Data;
        private Waves_Data _waves_Data;
        private Heart_Class_Data _heart_Class_Data;
        private QT_Disp_Data _qt_Disp_Data; 
        List<Tuple<string, List<int>>> _hear_Class_Data_Trans;
        private Basic_Data _basic_Data;

        private List<string> _displayedSeries;


        //Program ver 2.0 

        private string _currentAnalysisName;
        private string _currentLeadName;
        private bool _readNewData = false;

        private uint _currentBaselineLeadStartIndex;
        private uint _currentBaselineLeadEndIndex;
        private uint _currentBaselineLeadNumberOfSamples;

        private Vector<double> _currentBaselineLeadVector;

        private int _currentSavedPlotNumber = 0;
        private string _otherTabs = "";



        private Dictionary<string, uint> modulesVisualisationNeeds = new Dictionary<string, uint>()
        {
            {"ecgBaseline",1 },
            {"ecgBasic", 0 },
            { "r_Peaks", 3 }
        };

        private Dictionary<string, bool> _baselineDisplayedSeries = new Dictionary<string, bool>()
        {
            { "I",false },
            { "II", false },
            { "III", false },
            { "aVR", false },
            { "aVL", false },
            { "aVF", false },
            { "V1", false },
            { "V2", false },
            { "V3", false },
            { "V4", false },
            { "V5", false },
            { "V6", false },
            { "R_Peaks", false},
            { "QRSOnsets", false },
            { "QRSEnds", false },
            { "POnsets", false },
            { "PEnds", false },
            { "TEnds", false }
        };



        private Dictionary<string, bool> _otherDisplayedSeries = new Dictionary<string, bool>()
        {
            { "R_Peaks", false},
            { "QRSOnsets", false },
            { "QRSEnds", false },
            { "POnsets", false },
            { "PEnds", false },
            { "TEnds", false },
            { "Basic", false },
            { "TEnd_local", false }
           
        };

        
        private Dictionary<string, bool> _annotations = new Dictionary<string, bool>()
        {
            { "HeartClass", false }
        };

        private class AnnotationDescriber
        {
            public string idName { get; set; }
        }

        //test
        public ECGPlot(string plotTitle)
        {
            CurrentPlot = new PlotModel();
            CurrentPlot.Title = plotTitle;
            CurrentPlot.TitleFontSize = 16;
            //CurrentPlot.LegendTitle = "Legend";
            CurrentPlot.LegendOrientation = LegendOrientation.Horizontal;
            CurrentPlot.LegendPlacement = LegendPlacement.Outside;
            CurrentPlot.LegendPosition = LegendPosition.RightMiddle;
            _beginingPoint = 0;
            first = true;
            _visible = true;
            _displayedSeries = new List<string>();

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

        public void DisplayBasicData(List<Tuple<string, Vector<double>>> data)
        {
            if (first)
            {
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

            foreach (var signal in data)
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

        //public void DisplayBasicSignal()
        //{
        //    ClearPlot();

        //    //CurrentPlot.Series.Add();
        //}

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

        //public void DisplayEcgBaseline()
        //{
        //    if (first)
        //    {
        //        _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
        //        _ecg_Baseline_Data_worker.Load();
        //        first = false;
        //        var lineraYAxis = new LinearAxis();
        //        lineraYAxis.Position = AxisPosition.Left;
        //        lineraYAxis.Minimum = -100.0;
        //        lineraYAxis.Maximum = 80.0;
        //        lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
        //        lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
        //        lineraYAxis.Title = "Voltage [mV]";

        //        CurrentPlot.Axes.Add(lineraYAxis);
        //    }
        //    else
        //    {
        //        ClearPlot();
        //    }

        //    foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
        //    {

        //        Vector<double> signalVector = signal.Item2;
        //        LineSeries ls = new LineSeries();
        //        ls.Title = signal.Item1;
                

        //        ls.MarkerStrokeThickness = 1;
                

        //        for (int i = _beginingPoint; (i <= (_beginingPoint+_windowSize) && i< signalVector.Count()) ; i++)
        //        {
        //            ls.Points.Add(new DataPoint(i, signalVector[i]));
        //        }


        //        CurrentPlot.Series.Add(ls);
                
                
        //    }

        //    RefreshPlot();

        //    //foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
        //    //{

        //    //    Vector<double> signalVector = signal.Item2;
        //    //    LineSeries ls = new LineSeries();
        //    //    ls.Title = signal.Item1;

        //    //    ls.MarkerStrokeThickness = 1;


        //    //    for (int i = 0; i < signalVector.Count; i++)
        //    //    {
        //    //        ls.Points.Add(new DataPoint(i, signalVector[i]));
        //    //    }


        //    //    CurrentPlot.Series.Add(ls);
        //    //}

        //}

        public void DisplayEcgBaseline(List<Tuple<string, Vector<double>>> data)
        {
            if (first)
            {
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

            foreach (var signal in data)
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

        public void DisplayEcgBaseline(List<Tuple<string, Vector<double>>> data, bool whole)
        {
            if (first)
            {
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

            foreach (var signal in data)
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

        public void MovePlot(int amount)
        {
            _beginingPoint = _beginingPoint + amount;
            if(_beginingPoint<0)
            {
                _beginingPoint = 0;
            }
        }

        //public void DisplayR_Peaks()
        //{
        //    if (first)
        //    {
        //        _r_Peaks_Data_Worker = new R_Peaks_Data_Worker();
        //        _r_Peaks_Data_Worker.Load();

        //        //tutaj potrzebne do naniesienia na coś tych naszych r_peaksów
        //        _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
        //        _ecg_Baseline_Data_worker.Load();
        //        //

        //        first = false;

        //        //var lineraYAxis = new LinearAxis();
        //        //lineraYAxis.Position = AxisPosition.Left;
        //        //lineraYAxis.Minimum = -100.0;
        //        //lineraYAxis.Maximum = 80.0;
        //        //lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
        //        //lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
        //        //lineraYAxis.Title = "Voltage [mV]";

        //        //CurrentPlot.Axes.Add(lineraYAxis);
        //    }
        //    else
        //    {
        //        ClearPlot();
        //    }

        //    //wyśiwtlenie ecgBaseline
        //    foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
        //    {

        //        Vector<double> signalVector = signal.Item2;
        //        //var r_Peaks = _r_Peaks_Data_Worker.Data.RPeaks.Where(sig => sig.Item1 == signal.Item1);
        //        var r_Peaks = _r_Peaks_Data_Worker.Data.RPeaks.Find(sig => sig.Item1 == signal.Item1);
        //        Vector<double> r_PeaksVector = r_Peaks.Item2;
        //        bool addR_Peak = false;

        //        LineSeries ls = new LineSeries();
        //        ls.Title = signal.Item1;
        //        ScatterSeries rPeaksSeries = new ScatterSeries();
        //        rPeaksSeries.Title = "R_Peak_" + signal.Item1;
        //        //rPeaksSeries.MarkerType = MarkerType.Star;
                

        //        ls.MarkerStrokeThickness = 1;

        //        for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
        //        {
        //            ls.Points.Add(new DataPoint(i, signalVector[i]));
        //            if(r_PeaksVector.Contains(i))
        //            {
        //                rPeaksSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3});
                        
        //                addR_Peak = true;
        //            }
        //        }

        //        CurrentPlot.Series.Add(ls);
        //        if (addR_Peak)
        //        {
        //            CurrentPlot.Series.Add(rPeaksSeries);
        //        }
        //    }

        //    ////R_Peaks
        //    //foreach (var signal in _r_Peaks_Data_Worker.Data.RPeaks)
        //    //{

        //    //    Vector<double> signalVector = signal.Item2;
        //    //    LineSeries ls = new LineSeries();
        //    //    ls.Title = signal.Item1;

        //    //    ls.MarkerStrokeThickness = 1;


        //    //    for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
        //    //    {
        //    //        ls.Points.Add(new DataPoint(i, signalVector[i]));
        //    //    }


        //    //    CurrentPlot.Series.Add(ls);


        //    //}

        //    RefreshPlot();
        //}

        public void DisplayR_Peaks(List<Tuple<string, Vector<double>>> data)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                ClearPlot();
            }

            //wyśiwtlenie ecgBaseline
            foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                //var r_Peaks = _r_Peaks_Data_Worker.Data.RPeaks.Where(sig => sig.Item1 == signal.Item1);
                //var r_Peaks = _r_Peaks_Data_Worker.Data.RPeaks.Find(sig => sig.Item1 == signal.Item1);
                var r_Peaks = data.Find(sig => sig.Item1 == signal.Item1);
                Vector<double> r_PeaksVector = r_Peaks.Item2;
                bool addR_Peak = false;

                LineSeries ls = new LineSeries();
                ls.Title = signal.Item1;
                ScatterSeries rPeaksSeries = new ScatterSeries();
                rPeaksSeries.Title = "R_Peak_" + signal.Item1;
                //rPeaksSeries.MarkerType = MarkerType.Star;


                ls.MarkerStrokeThickness = 1;

                for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i, signalVector[i]));
                    if (r_PeaksVector.Contains(i))
                    {
                        rPeaksSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3 });

                        addR_Peak = true;
                    }
                }

                CurrentPlot.Series.Add(ls);
                if (addR_Peak)
                {
                    CurrentPlot.Series.Add(rPeaksSeries);
                }
            }

            RefreshPlot();
        }

        public void DisplayR_Peaks(List<Tuple<string, Vector<double>>> data, bool whole)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                //ClearPlot();
            }

            //wyśiwtlenie ecgBaseline
            foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                var r_Peaks = data.Find(sig => sig.Item1 == signal.Item1);
                Vector<double> r_PeaksVector = r_Peaks.Item2;
                bool addR_Peak = false;

                LineSeries ls = new LineSeries();
                ls.Title = signal.Item1;
                ScatterSeries rPeaksSeries = new ScatterSeries();
                rPeaksSeries.Title = "R_Peak_" + signal.Item1;
                //rPeaksSeries.MarkerType = MarkerType.Star;


                ls.MarkerStrokeThickness = 1;

                for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i, signalVector[i]));
                    if (r_PeaksVector.Contains(i))
                    {
                        rPeaksSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3 });

                        addR_Peak = true;
                    }
                }

                CurrentPlot.Series.Add(ls);
                if (addR_Peak)
                {
                    CurrentPlot.Series.Add(rPeaksSeries);
                }
            }

            RefreshPlot();
        }

        public ScatterSeries DisplayR_Peaks(double x, double y)
        {
            ScatterSeries series = new ScatterSeries();
            series.Points.Add(new ScatterPoint { X = x, Y = y, Size = 3 });
            return series;
        }


        public void DisplayControler(string whatToDisplay)
        {
            switch (whatToDisplay)
            {
                case "ecgBaseline":
                    DisplayEcgBaseline();
                    break;

                case "ecgBasic":
                    DisplayBasicData();
                    break;

                case "r_Peaks":
                    DisplayR_Peaks();
                    break;

                default:
                    break;
            }

        }

        public void DisplayControler(string whatToDisplay, List<Tuple<string,Vector<double>>> dataToDisplay)
        {
            switch (whatToDisplay)
            {
                case "ecgBaseline":
                    DisplayEcgBaseline(dataToDisplay);
                    break;

                case "ecgBasic":
                    DisplayBasicData(dataToDisplay);
                    break;

                case "r_Peaks":
                    DisplayR_Peaks(dataToDisplay);
                    break;

                default:
                    break;
            }

        }








        // Program ver2.0 

        //Konstruktor z nazwą analizy i tytułem wykresu.

        public ECGPlot(string currentAnalysysName, string plotTitle)
        {
            CurrentPlot = new PlotModel();
            CurrentPlot.Title = plotTitle;
            CurrentPlot.TitleFontSize = 16;
            CurrentPlot.LegendOrientation = LegendOrientation.Horizontal;
            CurrentPlot.LegendPlacement = LegendPlacement.Outside;
            CurrentPlot.LegendPosition = LegendPosition.RightMiddle;

            _beginingPoint = 0;
            first = true;
            _visible = true;
            _displayedSeries = new List<string>();

            Basic_New_Data_Worker bNW = new Basic_New_Data_Worker(currentAnalysysName);
            _analyseFrequency = bNW.LoadAttribute(Basic_Attributes.Frequency); 

           _currentAnalysisName = currentAnalysysName;
           _currentBaselineLeadStartIndex = 0;

           _windowSize = 5;
            _maxSeriesTime = 300; 


        }

        private void CalculateAmoutOfProcessingSamples()
        {
            //frequency = const
            //analyseSamples = const
            //_currentBaselineLeadStartIndex = changed by almost constant
            //_currentBaselineLeadEndIndex = changed by almost constant 

            //windowsSize -> figurate
            //

            //double ecgTimeInSeconds = ((_analyseSamples / _analyseFrequency));
            _currentBaselineLeadStartIndex = 0;
            _currentBaselineLeadEndIndex = ((uint)(_analyseFrequency * _maxSeriesTime)-1);
            if(_currentBaselineLeadEndIndex> _currentBaselineLeadNumberOfSamples)
            {
                _currentBaselineLeadEndIndex = _currentBaselineLeadNumberOfSamples;
            }

            _scalingPlotValue = 10;
        }

        private void CalculateAmoutOfProcessingSamplesWhenAskedToReadMore()
        {
            _readNewData = true;
            uint difference = _currentBaselineLeadEndIndex - _currentBaselineLeadStartIndex;
            _currentBaselineLeadStartIndex = _currentBaselineLeadEndIndex;

            _currentBaselineLeadEndIndex += _currentBaselineLeadEndIndex;
            if (_currentBaselineLeadEndIndex > _currentBaselineLeadNumberOfSamples)
            {
                _currentBaselineLeadEndIndex = _currentBaselineLeadNumberOfSamples;
            }

            RemoveAllPlotSeries();
            DisplayBaselineLeads(_currentLeadName);

        }


        //METODA DO WYSWIETLANIA KONKRETNEGO LEAD Z BASELINE
        public bool DisplayBaselineLeads(string leadName)
        {
            try
            {
                _currentLeadName = leadName;

                ECG_Baseline_New_Data_Worker ecg_Baseline = new ECG_Baseline_New_Data_Worker(_currentAnalysisName);
                _currentBaselineLeadNumberOfSamples =  ecg_Baseline.getNumberOfSamples(_currentLeadName);
                _analyseSamples = _currentBaselineLeadNumberOfSamples;
                // !!! TO DO !!! potrzebna logika do określania zakresy próbek 
                //_currentBaselineLeadEndIndex = _currentBaselineLeadNumberOfSamples; 
                CalculateAmoutOfProcessingSamples();
                Vector<double> myTemp =  ecg_Baseline.LoadSignal(leadName, (int)_currentBaselineLeadStartIndex, (int)_currentBaselineLeadEndIndex);
                _currentBaselineLeadVector = myTemp;
                //System.Windows.MessageBox.Show(myTemp.Count.ToString());
                


                //display plot
                try
                {
                    if (first)
                    {
                        first = false;
                        var lineraYAxis = new LinearAxis();
                        lineraYAxis.Position = AxisPosition.Left;
                        lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
                        lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
                        lineraYAxis.Title = "Amplitude [mV]";

                        CurrentPlot.Axes.Add(lineraYAxis);


                        var lineraXAxis = new LinearAxis();
                        lineraXAxis.Position = AxisPosition.Bottom;
                        lineraXAxis.Minimum = 0;
                        lineraXAxis.Maximum = _windowSize;
                        lineraXAxis.MajorGridlineStyle = LineStyle.Solid;
                        lineraXAxis.MinorGridlineStyle = LineStyle.Dot;
                        lineraXAxis.Title = "Time [s]";

                        CurrentPlot.Axes.Add(lineraXAxis);

                    }
                    else
                    {
                        ClearPlot();
                    }

                    LineSeries ls = new LineSeries();
                    ls.Title = leadName;
                    //ls.IsVisible = _visible;
                    //if (_visible)
                    //    _baselineDisplayedSeries[leadName] = true;
                    //_visible = false;

                    ls.MarkerStrokeThickness = 1;

                    for (int i = _beginingPoint; (i <= _analyseSamples && i < myTemp.Count()); i++)
                    {
                        ls.Points.Add(new DataPoint(i / _analyseFrequency, myTemp[i]));
                    }

                    CurrentPlot.Series.Add(ls);

                    RefreshPlot();

                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    return false; 
                }


                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHRV2Leads(string leadName)
        {
            try
            {
                CurrentPlot.Axes.Clear();

                _currentLeadName = leadName;
                bool addPoincare = false;

                HRV2_New_Data_Worker hW = new HRV2_New_Data_Worker(_currentAnalysisName);
                Vector<double> pX = hW.LoadSignal(HRV2_Signal.PoincarePlotData_x, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)hW.getNumberOfSamples(HRV2_Signal.PoincarePlotData_x, _currentLeadName));
                Vector<double> pY = hW.LoadSignal(HRV2_Signal.PoincarePlotData_y, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)hW.getNumberOfSamples(HRV2_Signal.PoincarePlotData_y, _currentLeadName));

                double elipseX = hW.LoadAttribute(HRV2_Attributes.ElipseCenter_x, _currentLeadName);
                double elipseY = hW.LoadAttribute(HRV2_Attributes.ElipseCenter_y, _currentLeadName);

                double sD1 = hW.LoadAttribute(HRV2_Attributes.SD1, _currentLeadName);
                double sD2 = hW.LoadAttribute(HRV2_Attributes.SD2, _currentLeadName);

                //EllipseAnnotation elipse = new EllipseAnnotation()
                //{
                //    Tag = new AnnotationDescriber { idName = leadName },
                //    X = elipseX,
                //    Y = elipseY,
                //    Height = sD1,
                //    Width = sD2,
                //    StrokeThickness = 1
                //};
                //elipse.Fill = OxyColors.Transparent;            
                //CurrentPlot.Annotations.Add(elipse);



                //double Xe1 = (-sD2 / 2);
                double Xe1 = (-sD1 / 2);
                double Ye1 = (0);
                double Xrot1 = elipseX + Xe1 * Math.Cos(0.7853981634) + Ye1 * Math.Sin(0.7853981634);
                double Yrot1 = elipseY - Xe1 * Math.Sin(0.7853981634) + Ye1 * Math.Cos(0.7853981634);

                double Xe2 = (0);
                //double Ye2 = (sD1 / 2);
                double Ye2 = (sD2 / 2);
                double Xrot2 = elipseX + Xe2 * Math.Cos(0.7853981634) + Ye2 * Math.Sin(0.7853981634);
                double Yrot2 = elipseY - Xe2 * Math.Sin(0.7853981634) + Ye2 * Math.Cos(0.7853981634);

                ArrowAnnotation sd1 = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(elipseX, elipseY),
                    EndPoint = new DataPoint(Xrot1, Yrot1),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 1,
                    Text = " SD2 "
                };
                ArrowAnnotation sd2 = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(elipseX, elipseY),
                    EndPoint = new DataPoint(Xrot2, Yrot2),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 1,
                    Text = " SD1 "                   
                };
                CurrentPlot.Annotations.Add(sd1);
                CurrentPlot.Annotations.Add(sd2);


                ScatterSeries elipseRot = new ScatterSeries();
                elipseRot.Title = "Elipse";

                double C_x = elipseX, C_y = elipseY, h = sD1, w = sD2;
                for (double t = 0; t <= 2 * 3.14; t += 0.01)
                {
                    //double Xe =  (sD2 / 2) * Math.Cos(t);
                    //double Ye =  (sD1 / 2) * Math.Sin(t);
                    double Xe = (sD1 / 2) * Math.Cos(t);
                    double Ye = (sD2 / 2) * Math.Sin(t);

                    double Xrot = elipseX + Xe * -Math.Cos(0.7853981634) + Ye * -Math.Sin(0.7853981634);
                    double Yrot = elipseY - Xe * -Math.Sin(0.7853981634) + Ye * -Math.Cos(0.7853981634);

                    elipseRot.Points.Add(new ScatterPoint { X = Xrot, Y = Yrot, Size = 1});
                    elipseRot.MarkerStroke = OxyColor.Parse("#ff0000");
                }

                //for (double t = 0; t <= 2 * 3.14; t += 0.01)
                //{
                //    //double Xe = C_x + (w / 2) * Math.Cos(t);
                //    //double Ye = C_y + (h / 2) * Math.Sin(t);

                //    double Xe = (w / 2) * Math.Cos(t);
                //    double Ye = (h / 2) * Math.Sin(t);

                //    double Xrot = C_x + Xe * Math.Cos(0.7853981634) + Ye * Math.Sin(0.7853981634);
                //    double Yrot = C_y - Xe * Math.Sin(0.7853981634) + Ye * Math.Cos(0.7853981634);

                //    //double Xrot = Xe;
                //    //double Yrot = Ye;

                //    //elipse.Points.Add(new ScatterPoint { X = Xe, Y = Ye, Size = 1.0 });
                //    elipseRot.Points.Add(new ScatterPoint { X = Xrot, Y = Yrot, Size = 1.0 });
                //    // Do what you want with X & Y here 
                //}

                CurrentPlot.Series.Add(elipseRot);


                ScatterSeries poincare = new ScatterSeries();
                poincare.Title = "Poincare";


                for(int i =0; i<pX.Count;i++)
                {
                    poincare.Points.Add(new ScatterPoint { X =pX[i], Y = pY[i], Size = 1.5 });

                    addPoincare = true;
                }

                CurrentPlot.Series.Add(poincare);


                if (addPoincare)
                {
                    var lineraYAxis = new LinearAxis();
                    lineraYAxis.Position = AxisPosition.Left;
                    lineraYAxis.Minimum = pY.Minimum();
                    lineraYAxis.Maximum = pY.Maximum();
                    lineraYAxis.Title = "RR(j+1) [ms]";

                    CurrentPlot.Axes.Add(lineraYAxis);


                    var lineraXAxis = new LinearAxis();
                    lineraXAxis.Position = AxisPosition.Bottom;
                    lineraXAxis.Minimum = pX.Minimum();
                    lineraXAxis.Maximum = pX.Maximum();
                    lineraXAxis.Title = "RR(j) [ms]";

                    CurrentPlot.Axes.Add(lineraXAxis);
                }

                //ArrowAnnotation xAx = new ArrowAnnotation
                //{
                //    StartPoint = new DataPoint(pX.Minimum(), pY.Minimum()),
                //    EndPoint = new DataPoint(pX.Maximum(), pY.Maximum()),
                //    Color = OxyColor.Parse("#000000"),
                //    StrokeThickness = 0.5
                //};
                //ArrowAnnotation yAx = new ArrowAnnotation
                //{
                //    StartPoint = new DataPoint(pX.Maximum(), pY.Minimum()),
                //    EndPoint = new DataPoint(pX.Minimum(), pY.Maximum()),
                //    Color = OxyColor.Parse("#000000"),
                //    StrokeThickness = 0.5
                //};
                //CurrentPlot.Annotations.Add(xAx);
                //CurrentPlot.Annotations.Add(yAx);

                //System.Windows.MessageBox.Show(CurrentPlot.Title + " " + _currentLeadName);

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHRV2HistogramLeads(string leadName)
        {
            try
            {
                _otherTabs = "HIST";
                HRV2_New_Data_Worker hW = new HRV2_New_Data_Worker(_currentAnalysisName);
                List<Tuple<double,double>> myTemp = hW.LoadHistogram(leadName, 0, (int)hW.getHistogramNumberOfSamples(leadName));

                CurrentPlot.Axes.Clear();

                ColumnSeries hist = new ColumnSeries();
                int i=1;
                foreach (var hV in myTemp)
                {
                    hist.Items.Add(new ColumnItem(hV.Item2, i));
                    i++;
                }

                //for (i = 0; i <= myTemp.Max(a => a.Item1); i++)
                //{
                //    if (myTemp.Exists(a => a.Item1 == i))
                //    {
                //        System.Windows.MessageBox.Show("znaleziono " + i);
                //        hist.Items.Add(new ColumnItem(myTemp.Find(a => a.Item1 == i).Item2, i));

                //    }
                //    else
                //    {
                //        hist.Items.Add(new ColumnItem(0, i));
                //    }
                //}

                //System.Windows.MessageBox.Show(i.ToString());

                CurrentPlot.Series.Add(hist);

                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                lineraYAxis.Minimum = myTemp.Min(a => a.Item2);
                lineraYAxis.Maximum = myTemp.Max(a => a.Item2)+2;
                lineraYAxis.Title = "Count [n]";

                CurrentPlot.Axes.Add(lineraYAxis);

                var categoryXaxis = new CategoryAxis();
                categoryXaxis.Position = AxisPosition.Bottom;
                //categoryXaxis.Minimum = myTemp.Min(a => a.Item1);
                //categoryXaxis.Maximum = myTemp.Max(a => a.Item1);
                categoryXaxis.Title = "t [s]";
                //categoryXaxis.IntervalLength = 20;
                categoryXaxis.IsAxisVisible = false;
                CurrentPlot.Axes.Add(categoryXaxis);

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayEcgBasicLeadVersion()
        {
            try
            {
                Basic_New_Data_Worker bNW = new Basic_New_Data_Worker(_currentAnalysisName);
                Vector<double> myTemp = bNW.LoadSignal(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)_currentBaselineLeadEndIndex);

                LineSeries ls = new LineSeries();
                ls.Title = "Basic";
                ls.MarkerStrokeThickness = 1;

                for (int i = _beginingPoint; (i <= _analyseSamples && i < myTemp.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i / _analyseFrequency, myTemp[i]));
                }

                CurrentPlot.Series.Add(ls);

                RefreshPlot();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DisplayR_PeaksLeadVersion()
        {
            try
            {
                
                R_Peaks_New_Data_Worker rPW = new R_Peaks_New_Data_Worker(_currentAnalysisName);
                Vector<double> myTemp = rPW.LoadSignal(R_Peaks_Attributes.RPeaks, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)rPW.getNumberOfSamples(R_Peaks_Attributes.RPeaks, _currentLeadName));
                bool addR_Peak = false;

                ScatterSeries rPeaksSeries = new ScatterSeries();
                rPeaksSeries.Title = "RPeaks";

                //for (int i = _beginingPoint; (i <= _analyseSamples && i < _currentBaselineLeadVector.Count()); i++)
                //{
                //    if (myTemp.Contains(i))
                //    {
                //        rPeaksSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                //        addR_Peak = true;
                //    }
                //}

                foreach (int i in myTemp.Where(a => (a <= _currentBaselineLeadEndIndex && a > 0)))
                {

                    rPeaksSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                    addR_Peak = true;
                }


                if (addR_Peak)
                {
                    CurrentPlot.Series.Add(rPeaksSeries);
                }



                RefreshPlot();
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }


        public bool DisplayWavesLeadAndWaveParVersion(string waveParametr)
        {
            try
            {
                Waves_New_Data_Worker wW = new Waves_New_Data_Worker(_currentAnalysisName);
                List<int> myTemp = new List<int>();                                
                switch (waveParametr)
                {
                    case "QRSOnsets":
                        myTemp = wW.LoadSignal(Waves_Signal.QRSOnsets, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)wW.getNumberOfSamples(Waves_Signal.QRSOnsets, _currentLeadName));
                        break;
                    case "QRSEnds":
                        myTemp = wW.LoadSignal(Waves_Signal.QRSEnds, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)wW.getNumberOfSamples(Waves_Signal.QRSEnds, _currentLeadName));
                        break;
                    case "POnsets":
                        myTemp = wW.LoadSignal(Waves_Signal.POnsets, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)wW.getNumberOfSamples(Waves_Signal.POnsets, _currentLeadName));
                        break;
                    case "PEnds":
                        myTemp = wW.LoadSignal(Waves_Signal.PEnds, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)wW.getNumberOfSamples(Waves_Signal.PEnds, _currentLeadName));
                        break;
                    case "TEnds":
                        myTemp = wW.LoadSignal(Waves_Signal.TEnds, _currentLeadName, (int)_currentBaselineLeadStartIndex, (int)wW.getNumberOfSamples(Waves_Signal.TEnds, _currentLeadName));
                        break;
                    default:
                        break;
                }

                bool addWave = false;

                ScatterSeries waveSeries = new ScatterSeries();
                waveSeries.Title = waveParametr;

                //for (int i = _beginingPoint; (i <= _analyseSamples && i < _currentBaselineLeadVector.Count()); i++)
                //{
                //    if (myTemp.Contains(i))
                //    {
                //        waveSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                //        addWave = true;
                //    }
                //}

                foreach (int i in myTemp.Where(a => (a <= _currentBaselineLeadEndIndex && a > 0)))
                {

                    waveSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                    addWave = true;
                }

                if (addWave)
                {
                    CurrentPlot.Series.Add(waveSeries);
                }

                RefreshPlot();
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHeartClassLeadVersion()
        {
            try
            {
                Heart_Class_New_Data_Worker hCW = new Heart_Class_New_Data_Worker(_currentAnalysisName);
                List<Tuple<int, int>> myTemp = hCW.LoadClassificationResult(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)hCW.getNumberOfSamples(_currentLeadName));

                foreach(var tp in myTemp)
                {

                    // _analyseSamples
                    if (tp.Item1 <= _currentBaselineLeadEndIndex)
                    {
                        Double yvalue = _currentBaselineLeadVector[tp.Item1];
                        //if (yvalue > 0)
                        //{
                        //    yvalue += 0.3;
                        //}
                        //else
                        //{
                        //    yvalue -= 0.6;
                        //}
                        if (tp.Item2 == 0)
                        {                           
                            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "V",
                                                                             Tag = new AnnotationDescriber {idName= "HeartClass"},
                                                                             TextPosition = new DataPoint(tp.Item1 / _analyseFrequency, yvalue) });
                        }
                        else
                        {
                            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "SV",
                                                                             Tag = new AnnotationDescriber { idName = "HeartClass" },
                                                                             TextPosition = new DataPoint(tp.Item1 / _analyseFrequency, yvalue) });

                        }
                        
                    }
                }

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayQTDispLeadVersion()
        {
            try
            {
                Qt_Disp_New_Data_Worker qDW = new Qt_Disp_New_Data_Worker(_currentAnalysisName);
                List<int> myTemp = qDW.LoadTEndLocal(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)qDW.getNumberOfSamples(Qt_Disp_Signal.T_End_Local, _currentLeadName));

                bool addQt = false;

                ScatterSeries qtSeries = new ScatterSeries();
                qtSeries.Title = "QTDisp";

                //for (int i = _beginingPoint; (i <= _analyseSamples && i < _currentBaselineLeadVector.Count()); i++)
                //{
                //    if (myTemp.Contains(i))
                //    {
                //        qtSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                //        addQt = true;
                //    }
                //}

                foreach (int i in myTemp.Where(a => (a <= _currentBaselineLeadEndIndex && a>0)))
                {
                    
                    qtSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 3 });

                    addQt = true;
                }

                if (addQt)
                {
                    CurrentPlot.Series.Add(qtSeries);
                }

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayAtrialFiberLeadVersion()
        {
            try
            {
                Atrial_Fibr_New_Data_Worker aFW = new Atrial_Fibr_New_Data_Worker(_currentAnalysisName);
                Tuple<bool,Vector<double>, string, string> myTemp = aFW.LoadAfDetection(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)aFW.getNumberOfSamples(_currentLeadName));
                       
                       
                System.Windows.MessageBox.Show(myTemp.Item3 + System.Environment.NewLine + myTemp.Item4);

                if (myTemp.Item1)
                {

                    ScatterSeries atrialFSeries = new ScatterSeries();
                    atrialFSeries.Title = "AtrialFiber";

                    foreach (int i in myTemp.Item2.Where(a => (a <= _currentBaselineLeadEndIndex && a > 0)))
                    {
                        atrialFSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 1.5 });
                    }


                    CurrentPlot.Series.Add(atrialFSeries);
                    

                    RefreshPlot();
                }
              
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayFlutterLeadVersion()
        {
            try
            {
                
                Flutter_New_Data_Worker fW = new Flutter_New_Data_Worker(_currentAnalysisName);
                List<Tuple<int, int>> myTemp = fW.LoadFlutterAnnotations(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)fW.getNumberOfSamples(_currentLeadName));
                
             
                if (myTemp.Count>0)
                {
                    //jakas logika jak bedzie sygnał 
                    //bool addFlutter = false;
                    ScatterSeries flutterSeries = new ScatterSeries();
                    flutterSeries.Title = "Flutter";

                    foreach (var tup in myTemp.Where(a => a.Item1 < _currentBaselineLeadEndIndex))
                    {
                        for (int i = tup.Item1; (i <= tup.Item2 && i<_currentBaselineLeadEndIndex) ; i++)
                        {
                            flutterSeries.Points.Add(new ScatterPoint { X = i / _analyseFrequency, Y = _currentBaselineLeadVector[i], Size = 1.5 });
                        }
                    }


                    CurrentPlot.Series.Add(flutterSeries);


                    RefreshPlot();

                    System.Windows.MessageBox.Show("Wykryto trzepotanie przedsionków");
                   //RefreshPlot();
                }
                else
                {
                    System.Windows.MessageBox.Show("Nie wykryto trzepotania przedsionków");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayTWaveAltLeadVersion()
        {
            try
            {
                T_Wave_Alt_New_Data_Worker tWA = new T_Wave_Alt_New_Data_Worker(_currentAnalysisName);
                List<Tuple<int,int>> myTemp = tWA.LoadAlternansDetectedList(_currentLeadName, (int)_currentBaselineLeadStartIndex, (int)tWA.getNumberOfSamples(_currentLeadName));

                foreach (var tp in myTemp)
                {

                    // _analyseSamples
                    if (tp.Item1 <= _currentBaselineLeadEndIndex)
                    {
                        Double yvalue = _currentBaselineLeadVector[tp.Item1];

                        if (tp.Item2 == 1)
                        {
                            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "Alt",
                                                                             Tag = new AnnotationDescriber { idName = "TWaveAlt" },
                                                                             TextPosition = new DataPoint(tp.Item1 / _analyseFrequency, yvalue) });
                        }
                    }
                }

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHeartAxisLeadVersion()
        {
            try
            {

                Heart_Axis_New_Data_Worker hAW = new Heart_Axis_New_Data_Worker(_currentAnalysisName);
                double heartAxis = hAW.LoadAttribute();
                
                //ScatterSeries zeroSeries = new ScatterSeries();
                //zeroSeries.IsVisible = true; 
                //zeroSeries.Points.Add
                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                lineraYAxis.Minimum = -110.0;
                lineraYAxis.Maximum = 112.0;
                lineraYAxis.IsAxisVisible = false;
                //lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
                //lineraYAxis.MinorGridlineStyle = LineStyle.Dot;

                var lineraXAxis = new LinearAxis();
                lineraXAxis.Position = AxisPosition.Bottom;
                lineraXAxis.Minimum = -120.0;
                lineraXAxis.Maximum = 110.0;
                lineraXAxis.IsAxisVisible = false;
                //lineraXAxis.MajorGridlineStyle = LineStyle.Solid;
                //lineraXAxis.MinorGridlineStyle = LineStyle.Dot;

                CurrentPlot.Axes.Add(lineraYAxis);
                CurrentPlot.Axes.Add(lineraXAxis);

                CurrentPlot.Annotations.Add(new TextAnnotation { Text = "0", TextPosition = new DataPoint(102, -3), StrokeThickness = 0 });
                CurrentPlot.Annotations.Add(new TextAnnotation { Text = "+-180", TextPosition = new DataPoint(-110, -3), StrokeThickness = 0 });
                CurrentPlot.Annotations.Add(new TextAnnotation { Text = "-90", TextPosition = new DataPoint(-1, 102), StrokeThickness = 0 });
                CurrentPlot.Annotations.Add(new TextAnnotation { Text = "90", TextPosition = new DataPoint(0, -107), StrokeThickness = 0 });

                ArrowAnnotation arrow = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(100 * Math.Cos(heartAxis), -100 * Math.Sin(heartAxis)),
                    Text = heartAxis.ToString("0.000") + "rad"
                };

                ArrowAnnotation yup = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(0, 100),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 0.2
                };
                ArrowAnnotation ydown = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(0, -100),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 0.2
                };
                ArrowAnnotation xleft = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(-100, 0),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 0.2
                };

                ArrowAnnotation xright = new ArrowAnnotation
                {
                    StartPoint = new DataPoint(0, 0),
                    EndPoint = new DataPoint(100, 0),
                    Color = OxyColor.Parse("#000000"),
                    StrokeThickness = 0.2
                };

                CurrentPlot.Annotations.Add(yup);
                CurrentPlot.Annotations.Add(ydown);
                CurrentPlot.Annotations.Add(xleft);
                CurrentPlot.Annotations.Add(xright);
                CurrentPlot.Annotations.Add(arrow);

                RefreshPlot();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public bool DisplaySleepApneaLeadVersion(string leadName)
        {
            try
            {
                Sleep_Apnea_New_Data_Worker sAW = new Sleep_Apnea_New_Data_Worker(_currentAnalysisName);
                double illAp = sAW.LoadIlApnea(leadName);

                if (illAp > 0)
                {

                    uint[] tempU = sAW.getHAmpNumberOfSamples(leadName);
                    int[] tempInt = new int[tempU.Length];
                    int[] tempIntS = new int[tempU.Length];
                    for (int i = 0; i < tempU.Length; i++)
                    {
                        tempInt[i] = (int)tempU[i];
                        tempIntS[i] = 0;
                    }


                    List<Tuple<int, int>> myTemp = sAW.LoadDetectedApnea(leadName, 0, (int)sAW.getNumberOfSamples(leadName));
                    List<List<double>> myTempH = sAW.LoadHAmp(leadName, tempIntS, tempInt);


                    

                    LineSeries ls = new LineSeries();
                    ls.Title = leadName;
                    ls.MarkerStrokeThickness = 1;

                    //foreach (var l in myTempH)
                    //{
                    //    ls.Points.Add(new DataPoint(l[0], l[1]));
                    //}

                    //foreach (var a in myTempH[0])
                    //{

                    //}

                    //for (int i =0; i<2;i++)
                    //{
                    //    ls.Points.Add(new DataPoint(myTempH[i], myTempH[i]));
                    //}

                    List<double> xes = myTempH[0];
                    List<double> yes = myTempH[1];


                    for (int i = 0; i < 2; i++)
                    {
                        ls.Points.Add(new DataPoint(xes[i], yes[i]));
                    }






                    CurrentPlot.Series.Add(ls);

                    RefreshPlot();



                    System.Windows.MessageBox.Show("Sleep anpea was detected in " + illAp +"% signal time.");
                }
                else
                {
                    System.Windows.MessageBox.Show("There was no sleep apnea in current signal.");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplaySleepApneaLeadVersionBaseline()
        {
            try
            {
                Sleep_Apnea_New_Data_Worker sAW = new Sleep_Apnea_New_Data_Worker(_currentAnalysisName);
                double illAp = sAW.LoadIlApnea(_currentLeadName);

                if (illAp > 0)
                {


                    List<Tuple<int, int>> myTemp = sAW.LoadDetectedApnea(_currentLeadName, 0, (int)sAW.getNumberOfSamples(_currentLeadName));

                    ScatterSeries sleepApnea = new ScatterSeries();
                    sleepApnea.Title = "SleepApnea";
                    double min = _currentBaselineLeadVector.Minimum();

                    foreach (var tup in myTemp.Where(a => a.Item1 < _currentBaselineLeadEndIndex))
                    {
                        for (int i = tup.Item1; (i <= tup.Item2 && i < _currentBaselineLeadEndIndex); i++)
                        {

                            sleepApnea.Points.Add(new ScatterPoint { X = i, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.1, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.2, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.3, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.4, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.5, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.6, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.7, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.8, Y = min, Size = 2 });
                            sleepApnea.Points.Add(new ScatterPoint { X = i + 0.9, Y = min, Size = 2 });

                        }
                    }


                    CurrentPlot.Series.Add(sleepApnea);
                    RefreshPlot();

                    System.Windows.MessageBox.Show("Sleep anpea was detected in " + illAp + "% signal time.");
                }
                else
                {
                    System.Windows.MessageBox.Show("There was no sleep apnea in current signal.");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHrvDfaLeadVersion(string leadName)
        {
            try
            {
                CurrentPlot.Axes.Clear();
                HRV_DFA_New_Data_Worker hDW = new HRV_DFA_New_Data_Worker(_currentAnalysisName);
                Tuple<Vector<double>, Vector<double>> dFANumberN = hDW.LoadSignal(HRV_DFA_Signals.DfaNumberN, leadName, 0, (int)hDW.getNumberOfSamples(HRV_DFA_Signals.DfaNumberN, leadName));
                Tuple<Vector<double>, Vector<double>> dFAValueFn = hDW.LoadSignal(HRV_DFA_Signals.DfaValueFn, leadName, 0, (int)hDW.getNumberOfSamples(HRV_DFA_Signals.DfaValueFn, leadName));
                Tuple<Vector<double>, Vector<double>> fluctuations = hDW.LoadSignal(HRV_DFA_Signals.Fluctuations, leadName, 0, (int)hDW.getNumberOfSamples(HRV_DFA_Signals.Fluctuations, leadName));
                Tuple<Vector<double>, Vector<double>> paramAlpha = hDW.LoadSignal(HRV_DFA_Signals.ParamAlpha, leadName, 0, (int)hDW.getNumberOfSamples(HRV_DFA_Signals.ParamAlpha, leadName));



                if (dFANumberN.Item1.Maximum() > 0)
                {
                    LineSeries ls = new LineSeries();
                    ls.Title = leadName+"-1";
                    ls.MarkerStrokeThickness = 1;

                    for (int i = 0; i < dFANumberN.Item1.Count; i++)
                    {
                        ls.Points.Add(new DataPoint(dFANumberN.Item1[i], dFAValueFn.Item1[i]));
                    }

                    CurrentPlot.Series.Add(ls);
                }

                if (dFANumberN.Item2.Maximum() > 0)
                {
                    LineSeries ls = new LineSeries();
                    ls.Title = leadName + "-2";
                    ls.MarkerStrokeThickness = 1;

                    for (int i = 0; i < dFANumberN.Item2.Count; i++)
                    {
                        ls.Points.Add(new DataPoint(dFANumberN.Item2[i], dFAValueFn.Item2[i]));
                    }

                    CurrentPlot.Series.Add(ls);
                }

                if(fluctuations.Item1.Count>0)
                {
                    ScatterSeries fluctuationsSeries = new ScatterSeries();
                    fluctuationsSeries.Title = "Fluctuations";

                    for (int i = 0; i < fluctuations.Item1.Count; i++)
                    {
                        fluctuationsSeries.Points.Add(new ScatterPoint { X = fluctuations.Item1[i], Y = fluctuations.Item2[i], Size = 2 });
                    }

                    CurrentPlot.Series.Add(fluctuationsSeries);
                }

                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                lineraYAxis.Title = "log F(n)";

                CurrentPlot.Axes.Add(lineraYAxis);

                var lineraXAxis = new LinearAxis();
                lineraXAxis.Position = AxisPosition.Bottom;
                lineraXAxis.Title = "log n";

                CurrentPlot.Axes.Add(lineraXAxis);

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHrv1LeadVersion(string leadName)
        {
            try
            {
                HRV1_New_Data_Worker hW = new HRV1_New_Data_Worker(_currentAnalysisName);
                CurrentPlot.Axes.Clear();
                //Vector<double> freqBP =  hW.LoadSignal(HRV1_Signal.FreqBasedParams, leadName, 0, (int)hW.getNumberOfSamples(HRV1_Signal.FreqBasedParams, leadName));

                Vector<double> freQV = hW.LoadSignal(HRV1_Signal.FreqVector, leadName, 0, (int)hW.getNumberOfSamples(HRV1_Signal.FreqVector, leadName));              
                Vector<double> pSD = hW.LoadSignal(HRV1_Signal.PSD, leadName, 0, (int)hW.getNumberOfSamples(HRV1_Signal.PSD, leadName));
                //Vector<double> timeBP = hW.LoadSignal(HRV1_Signal.TimeBasedParams, leadName, 0, (int)hW.getNumberOfSamples(HRV1_Signal.TimeBasedParams, leadName));


                LineSeries ls = new LineSeries();
                ls.Title = leadName;

                ls.MarkerStrokeThickness = 1;

                for (int i = 0; i < freQV.Count; i++)
                {
                    ls.Points.Add(new DataPoint(freQV[i], pSD[i]));
                    //Points.Add(new DataPoint(pSD[i], freQV[i]));
                }

                CurrentPlot.Series.Add(ls);

                //var lineraYAxis = new LinearAxis();
                //lineraYAxis.Position = AxisPosition.Left;
                //lineraYAxis.Title = "Power [ms2]";

                //CurrentPlot.Axes.Add(lineraYAxis);

                //var lineraXAxis = new LinearAxis();
                //lineraXAxis.Position = AxisPosition.Bottom;
                //lineraXAxis.Minimum = freQV.Minimum();
                //lineraXAxis.Maximum = freQV.Maximum();
                //lineraXAxis.Title = "Frequency [Hz]";

                //CurrentPlot.Axes.Add(lineraXAxis);


                var logarithmicAxis1 = new LogarithmicAxis();
                logarithmicAxis1.Maximum = pSD.Maximum()+0.01;
                logarithmicAxis1.Minimum = pSD.Minimum();
                logarithmicAxis1.Title = "Power";
                logarithmicAxis1.Position = AxisPosition.Left;
                logarithmicAxis1.UseSuperExponentialFormat = true;
                CurrentPlot.Axes.Add(logarithmicAxis1);
                var logarithmicAxis2 = new LogarithmicAxis();
                logarithmicAxis2.Maximum = freQV.Maximum() + 0.01;
                logarithmicAxis2.Minimum = freQV.Minimum();
                logarithmicAxis2.Position = AxisPosition.Bottom;
                logarithmicAxis2.Title = "Frequency";
                logarithmicAxis2.UseSuperExponentialFormat = true;
                CurrentPlot.Axes.Add(logarithmicAxis2);


                RefreshPlot();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHrtLeadVersion(string leadName, bool turb)
        {
            try
            {
                ClearPlot();
                HRT_New_Data_Worker hWD = new HRT_New_Data_Worker(_currentAnalysisName);

                if(hWD.LoadVPC(leadName)== Modules.HRT.HRT.VPC.LETS_PLOT)
                {
                    CurrentPlot.Axes.Clear();
                    //double[] meanTachogram = hWD.LoadMeanTachogramGUI(leadName);
                    int[] statClass = hWD.LoadStatisticsClassNumbersPDF(leadName);

                    //int a = 0;
                    //System.Windows.MessageBox.Show(leadName);
                    //System.Windows.MessageBox.Show(statClass[2].ToString());
                    //System.Windows.MessageBox.Show(tachogram.Count.ToString());

                    for(int i = 0; i<statClass[2]; i++)
                    {
                        List<List<double>> tachogram = hWD.LoadTachogramGUI(leadName, i);

                        foreach (List<double> tach in tachogram)
                        {
                            LineSeries ls = new LineSeries();
                            //ls.Title = leadName + i.ToString();
                            ls.MarkerStrokeThickness = 1;
                            ls.Color = OxyColor.Parse("#ffc04c");

                            //System.Windows.MessageBox.Show("HRT" + i);
                            for (int j = 0; j < tach.Count; j++)
                            {
                                ls.Points.Add(new DataPoint(j, tach[j]));
                            }

                            CurrentPlot.Series.Add(ls);
                        }
                    }

                    double[] meanTachogram = hWD.LoadMeanTachogramGUI(leadName);
                    if (meanTachogram.Length > 0)
                    {
                        LineSeries ls = new LineSeries();
                        ls.MarkerStrokeThickness = 2;
                        ls.Color = OxyColor.Parse("#ff0000");
                        ls.Title = "MeanTach";
                        

                        for (int i = 0; i < meanTachogram.Length; i++)
                        {
                            ls.Points.Add(new DataPoint(i, meanTachogram[i]));
                        }

                        CurrentPlot.Series.Add(ls);
                    }

                    var lineraYAxis = new LinearAxis();
                    lineraYAxis.Position = AxisPosition.Left;
                    lineraYAxis.Title = "RR interval [ms]";

                    CurrentPlot.Axes.Add(lineraYAxis);

                    var lineraXAxis = new LinearAxis();
                    lineraXAxis.Position = AxisPosition.Bottom;
                    lineraXAxis.Title = "RR interval";

                    CurrentPlot.Axes.Add(lineraXAxis);

                    if(turb)
                    {
                        System.Windows.MessageBox.Show("Turbulance fo lead" + leadName);

                        
                        double[] turbulenceSlopeMax = hWD.LoadTurbulenceSlopeMaxGUI(leadName);
                        int[] loadXPointsMaxSlope = hWD.LoadXPointsMaxSlopeGUI(leadName);
                        //int[] loadXAxisTachogram = hWD.LoadXAxisTachogramGUI(leadName);
                        

                        if (loadXPointsMaxSlope.Length > 0)
                        {
                            LineSeries ls = new LineSeries();
                            ls.MarkerStrokeThickness = 2;
                            //ls.BrokenLineStyle = LineStyle.DashDashDot;
                            ls.Color = OxyColor.Parse("#4c0026");
                            ls.Title = "TurbSlope";

                            for (int i = 0; i <loadXPointsMaxSlope.Length; i++)
                            {
                                ls.Points.Add(new DataPoint(loadXPointsMaxSlope[i], turbulenceSlopeMax[i]));
                            }

                            CurrentPlot.Series.Add(ls);
                        }

                        int[] loadXPointsMeanOnset = hWD.LoadXPointsMeanOnsetGUI(leadName);
                        double[] turbulenceOnsetMean = hWD.LoadTurbulenceOnsetMeanGUI(leadName);

                        if(loadXPointsMeanOnset.Length>0)
                        {
                            try
                            {


                                LineSeries ls1 = new LineSeries();
                                ls1.MarkerStrokeThickness = 2;
                                ls1.Color = OxyColor.Parse("#0000ff");
                                ls1.Title = "TurbMeanF";

                                ls1.Points.Add(new DataPoint(loadXPointsMeanOnset[0], turbulenceOnsetMean[0]));
                                ls1.Points.Add(new DataPoint(loadXPointsMeanOnset[1], turbulenceOnsetMean[1]));
                                CurrentPlot.Series.Add(ls1);

                                LineSeries ls2 = new LineSeries();
                                ls2.MarkerStrokeThickness = 2;
                                ls2.Color = OxyColor.Parse("#0000ff");
                                ls2.Title = "TurbMeanS";

                                ls2.Points.Add(new DataPoint(loadXPointsMeanOnset[2], turbulenceOnsetMean[2]));
                                ls2.Points.Add(new DataPoint(loadXPointsMeanOnset[3], turbulenceOnsetMean[3]));
                                CurrentPlot.Series.Add(ls2);



                            }
                            catch(Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message);
                            }


                        }


                    }

                }
                else
                {
                    System.Windows.MessageBox.Show("There was no possitive detection or" + System.Environment.NewLine +"detected values does not allowed to plot them.");
                }
                

                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DisplayHeartClusterLeadVersion(string leadName)
        {
            try
            {
                Heart_Cluster_Data_Worker hCW = new Heart_Cluster_Data_Worker(_currentAnalysisName);

                //hCW.LoadClusterizationResult(leadName, 0, int lenght <- skąd mam znać rozmiar? nie ma go w workerze)

                
                RefreshPlot();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }


        public bool ControlOtherModulesSeries(string moduleName, bool visible)
        {
            try
            {
                if(visible)
                {
                    DisplayOtherNewSeries(moduleName);
                }
                else
                {
                    RemoveOtherSeries(moduleName);
                }
                RefreshPlot();
                return true;
            }
            catch
            {
                RefreshPlot();
                return false;
            }
        }

        private bool DisplayOtherNewSeries(string modName)
        {
            try
            {
                switch (modName)
                {
                    case "Basic":
                        DisplayEcgBasicLeadVersion();
                        break;
                    case "RPeaks":
                        DisplayR_PeaksLeadVersion();
                        break;
                    case "QRSOnsets":
                        DisplayWavesLeadAndWaveParVersion(modName);
                        break;
                    case "QRSEnds":
                        DisplayWavesLeadAndWaveParVersion(modName);
                        break;
                    case "POnsets":
                        DisplayWavesLeadAndWaveParVersion(modName);
                        break;
                    case "PEnds":
                        DisplayWavesLeadAndWaveParVersion(modName);
                        break;
                    case "TEnds":
                        DisplayWavesLeadAndWaveParVersion(modName);
                        break;
                    case "HeartClass":
                        DisplayHeartClassLeadVersion();
                        break;
                    case "QTDisp":
                        DisplayQTDispLeadVersion();
                        break;
                    case "AtrialFiber":
                        DisplayAtrialFiberLeadVersion();
                        break;
                    case "TWaveAlt":
                        DisplayTWaveAltLeadVersion();
                        break;
                    case "Flutter":
                        DisplayFlutterLeadVersion();
                        break;
                    case "SleepApnea":
                        DisplaySleepApneaLeadVersionBaseline();
                        break;
                    case "Turb":
                        DisplayHrtLeadVersion(_currentLeadName, true);
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool RemoveOtherSeries(string modName)
        {
            try
            {
                if(modName == "HeartClass" || modName == "TWaveAlt")
                {
                    AnnotationDescriber aD = new AnnotationDescriber { idName = modName };
                    bool goOn = true;
                    while (goOn)
                    {
                        try
                        {
                            Annotation temp = CurrentPlot.Annotations.First(a => (a.Tag as AnnotationDescriber).idName  ==  aD.idName);
                            CurrentPlot.Annotations.Remove(temp); 
                        }
                        catch(Exception ex)
                        {
                            goOn = false;
                        }

                    }
                }
                else
                {
                    CurrentPlot.Series.Remove(CurrentPlot.Series.First(a => a.Title == modName));
                }
                
                return true;
            }
            catch(Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }











        //ver 1.0
        //ostatnio developowana wersja begining

        public void DisplayControler(Dictionary<string, List<Tuple<string, Vector<double>>>> dataToDisplay)
        {
            bool wasEcgBaselineSet = false;
            List<string> modulesToDisplay = new List<string>();
            foreach(var data in dataToDisplay)
            {
                switch (data.Key)
                {
                    case "ecgBaseline":
                        _ecg_Baseline_Data = new ECG_Baseline_Data();
                        _ecg_Baseline_Data.SignalsFiltered = dataToDisplay[data.Key];
                        wasEcgBaselineSet = true;
                        modulesToDisplay.Add(data.Key);
                        //DisplayEcgBaseline(dataToDisplay[data.Key], true);
                        break;
                    case "r_Peaks":
                        _r_PeaksData = new R_Peaks_Data();
                        _r_PeaksData.RPeaks = dataToDisplay[data.Key];
                        if(wasEcgBaselineSet)
                            modulesToDisplay.Add(data.Key);
                        //DisplayR_Peaks(dataToDisplay[data.Key], true);
                        break;
                    default:
                        break;
                }
                
            }

            DisplayListedPlots(modulesToDisplay);

        }

        public void DisplayControler(Dictionary<string, List<Tuple<string, Vector<double>>>> dataToDisplayV, Dictionary<string, List<Tuple<string, List<int>>>> dataToDisplayL, uint freq, uint samples)
        {
            bool wasEcgBaselineSet = false;
            _analyseFrequency = freq;
            //_analyseFrequency = _analyseFrequency
            

            _analyseSamples = samples;

            if(_analyseSamples<10000)
            {
                _scalingPlotValue = 10;
                _windowSize =((_analyseSamples/_analyseFrequency));
                //System.Windows.MessageBox.Show("windowssize=" + _windowSize);
                //_windowSize = _windowSize / 10;
                _windowSize = _windowSize / _scalingPlotValue;
                //System.Windows.MessageBox.Show("windowssize=" + _windowSize);
            }
            else if(_analyseSamples<30000)
            {
                _scalingPlotValue = 30;
                _windowSize = ((_analyseSamples / _analyseFrequency));
                //_windowSize = _windowSize / 10;
                _windowSize = _windowSize / _scalingPlotValue;
            }
            else
            {
                _scalingPlotValue = 200;
                _windowSize = ((_analyseSamples / _analyseFrequency));
                //_windowSize = _windowSize / 10;
                _windowSize = _windowSize / _scalingPlotValue;
            }

            //System.Windows.MessageBox.Show("windowssize=" + _windowSize);

            List<string> modulesToDisplay = new List<string>();
           
            foreach (var data in dataToDisplayV)
            {
                switch (data.Key)
                {
                    case "ecgBaseline":
                        _ecg_Baseline_Data = new ECG_Baseline_Data();
                        _ecg_Baseline_Data.SignalsFiltered = dataToDisplayV[data.Key];
                        wasEcgBaselineSet = true;
                        modulesToDisplay.Add(data.Key);
                        //DisplayEcgBaseline(dataToDisplay[data.Key], true);
                        break;
                    case "r_Peaks":
                        _r_PeaksData = new R_Peaks_Data();
                        _r_PeaksData.RPeaks = dataToDisplayV[data.Key];
                        if (wasEcgBaselineSet)
                            modulesToDisplay.Add(data.Key);
                        //DisplayR_Peaks(dataToDisplay[data.Key], true);
                        break;
                    case "ecgBasic":
                        _basic_Data = new Basic_Data();
                        _basic_Data.Signals = dataToDisplayV[data.Key];
                        modulesToDisplay.Add(data.Key); 
                        break; 


                    default:
                        break;
                }

            }

            _waves_Data = new Waves_Data();
            

            foreach (var data in dataToDisplayL)
            {
                switch (data.Key)
                {
                    case "QRSOnsets":
                        _waves_Data.QRSOnsets = dataToDisplayL[data.Key];
                        modulesToDisplay.Add(data.Key);
                        break;
                    case "QRSEnds":
                        _waves_Data.QRSEnds= dataToDisplayL[data.Key];
                        modulesToDisplay.Add(data.Key);
                        break;
                    case "POnsets":
                        _waves_Data.POnsets = dataToDisplayL[data.Key];
                        modulesToDisplay.Add(data.Key);
                        break;
                    case "PEnds":
                        _waves_Data.PEnds = dataToDisplayL[data.Key];
                        modulesToDisplay.Add(data.Key);
                        break;
                    case "TEnds":
                        _waves_Data.TEnds = dataToDisplayL[data.Key];
                        modulesToDisplay.Add(data.Key);
                        break;
                    case "HeartClass":
                        _hear_Class_Data_Trans = new List<Tuple<string, List<int>>>();
                        _hear_Class_Data_Trans = data.Value;   
                        modulesToDisplay.Add("HeartClass");
                        break;
                    case "TEnd_local":
                        _qt_Disp_Data = new QT_Disp_Data();
                        _qt_Disp_Data.T_End_Local = data.Value; 
                        modulesToDisplay.Add("TEnd_local");
                        break;
                    default:
                        break;
                }

            }


            DisplayListedPlots(modulesToDisplay);

        }

        private void DisplayListedPlots(List<string> whatToDisplay)
        {
            foreach(string dis in whatToDisplay)
            {
                switch (dis)
                {
                    case "ecgBaseline":
                        DisplayEcgBaseline();
                        break;
                    case "ecgBasic":
                        DisplayEcgBasic();
                        break;
                    case "r_Peaks":
                        DisplayR_Peaks();
                        break;
                    case "QRSOnsets":
                        DisplayWaves(dis);
                        break;
                    case "QRSEnds":
                        DisplayWaves(dis);
                        break;
                    case "POnsets":
                        DisplayWaves(dis);
                        break;
                    case "PEnds":
                        DisplayWaves(dis);
                        break;
                    case "TEnds":
                        DisplayWaves(dis);
                        break;
                    case "HeartClass":
                        //DisplayHeartClass();
                        break;
                    case "TEnd_local":
                        DisplayQt_Disp();
                        break;
                    default:
                        break;
                }
            }
        }


        private void DisplayEcgBasic()
        {
            if (first)
            {
                first = false;
            }
            else
            {
                //ClearPlot();
            }

            foreach (var signal in _basic_Data.Signals)
            {

                Vector<double> signalVector = signal.Item2;
                LineSeries ls = new LineSeries();
                ls.Title = "Basic" + signal.Item1;
                ls.IsVisible = _visible;

                ls.MarkerStrokeThickness = 1;


                for (int i = _beginingPoint; (i <= _analyseSamples && i < signalVector.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i/_analyseFrequency, signalVector[i]));
                }

                CurrentPlot.Series.Add(ls);

            }

            RefreshPlot();
        }

        private void DisplayEcgBaseline()
        {
            if (first)
            {
                first = false;
                var lineraYAxis = new LinearAxis();
                lineraYAxis.Position = AxisPosition.Left;
                //lineraYAxis.Minimum = -100.0;
                //lineraYAxis.Maximum = 80.0;
                lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
                lineraYAxis.MinorGridlineStyle = LineStyle.Dot;
                //lineraYAxis.Title = "Voltage [mV]";
                lineraYAxis.Title = "Amplitude [mV]";

                CurrentPlot.Axes.Add(lineraYAxis);
                //_windowSize = _ecg_Baseline_Data.SignalsFiltered.First().Item2.Count;
                
            }
            else
            {
                ClearPlot();
            }

            foreach (var signal in _ecg_Baseline_Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                LineSeries ls = new LineSeries();
                ls.Title = signal.Item1;
                ls.IsVisible = _visible;
                if (_visible)
                    _baselineDisplayedSeries[signal.Item1] = true;
                _visible = false;


                ls.MarkerStrokeThickness = 1;


                //for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                //{
                //    ls.Points.Add(new DataPoint(i/_analyseFrequency, signalVector[i]));
                //}
                for (int i = _beginingPoint; (i <= _analyseSamples && i < signalVector.Count()); i++)
                {
                    ls.Points.Add(new DataPoint(i / _analyseFrequency, signalVector[i]));
                }


                CurrentPlot.Series.Add(ls);

            }


            var lineraXAxis = new LinearAxis();
            lineraXAxis.Position = AxisPosition.Bottom;
            lineraXAxis.Minimum = 0;
            lineraXAxis.Maximum = _windowSize;
            //System.Windows.MessageBox.Show("linearaxis maximum= " + lineraXAxis.Maximum);
            lineraXAxis.MajorGridlineStyle = LineStyle.Solid;
            lineraXAxis.MinorGridlineStyle = LineStyle.Dot;
            lineraXAxis.Title = "Time [s]";
            CurrentPlot.Axes.Add(lineraXAxis);
            

            RefreshPlot();
        }

        private void DisplayR_Peaks()
        {
            if (first)
            {
                first = false;
            }
            else
            {
                //ClearPlot();
            }

            //wyśiwtlenie ecgBaseline var signal in _ecg_Baseline_Data.Data.SignalsFiltered
            foreach (var signal in _ecg_Baseline_Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                var r_Peaks = _r_PeaksData.RPeaks.Find(sig => sig.Item1 == signal.Item1);
                Vector<double> r_PeaksVector = r_Peaks.Item2;
                bool addR_Peak = false;

                ScatterSeries rPeaksSeries = new ScatterSeries();
                rPeaksSeries.Title = "R_Peaks" + signal.Item1;
                rPeaksSeries.IsVisible = _visible;
                //rPeaksSeries.DataFieldTag = "R_Peak_" + signal.Item1;


                //for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                //{
                //    if (r_PeaksVector.Contains(i))
                //    {
                //        rPeaksSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3 });

                //        addR_Peak = true;
                //    }
                //}

                for (int i = _beginingPoint; (i <= _analyseSamples && i < signalVector.Count()); i++)
                {
                    if (r_PeaksVector.Contains(i))
                    {
                        rPeaksSeries.Points.Add(new ScatterPoint { X = i/_analyseFrequency, Y = signalVector[i], Size = 3 });

                        addR_Peak = true;
                    }
                }


                if (addR_Peak)
                {
                    CurrentPlot.Series.Add(rPeaksSeries);
                }
            }

            RefreshPlot();
        }

        private void DisplayWaves(string wavePart)
        {
            foreach (var signal in _ecg_Baseline_Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                List<int> waveList = new List<int>();
                switch (wavePart)
                {
                    case "QRSOnsets":
                        waveList = _waves_Data.QRSOnsets.Find(sig => sig.Item1 == signal.Item1).Item2;                     
                        break;
                    case "QRSEnds":
                        waveList = _waves_Data.QRSEnds.Find(sig => sig.Item1 == signal.Item1).Item2;
                        break;
                    case "POnsets":
                        waveList = _waves_Data.POnsets.Find(sig => sig.Item1 == signal.Item1).Item2;
                        break;
                    case "PEnds":
                        waveList = _waves_Data.PEnds.Find(sig => sig.Item1 == signal.Item1).Item2;
                        break;
                    case "TEnds":
                        waveList = _waves_Data.TEnds.Find(sig => sig.Item1 == signal.Item1).Item2;
                        break;
                    default:
                        break;
                }

                bool addWave = false;

                ScatterSeries waveSeries = new ScatterSeries();
                //waveSeries.Title = wavePart + "_" + signal.Item1;
                waveSeries.Title = wavePart + signal.Item1;
                waveSeries.IsVisible = _visible;



                //for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                //{
                //    if (waveList.Contains(i))
                //    {
                //        waveSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3 });

                //        addWave = true;
                //    }
                //}
                for (int i = _beginingPoint; (i <= _analyseSamples && i < signalVector.Count()); i++)
                {
                    if (waveList.Contains(i))
                    {
                        waveSeries.Points.Add(new ScatterPoint { X = i/_analyseFrequency, Y = signalVector[i], Size = 3 });

                        addWave = true;
                    }
                }

                if (addWave)
                {
                    CurrentPlot.Series.Add(waveSeries);
                }
            }

            RefreshPlot();
        }

        private void DisplayHeartClass()
        {
            if (first)
            {
                first = false;
            }
            else
            {
                //ClearPlot();
            }

            //wyśiwtlenie ecgBaseline var signal in _ecg_Baseline_Data.Data.SignalsFiltered


            //List<Tuple<string, List<int>>> _hear_Class_Data_Trans;

            //ScatterSeries heartClassSeries = new ScatterSeries();
            //heartClassSeries.Title = "HeartClassII";
            //heartClassSeries.IsVisible = _visible;


            //bool addHeartClass = false;

            //foreach (var t in _hear_Class_Data_Trans)
            //{

            //    foreach (int val in t.Item2)
            //    {
            //        if (val <= (_beginingPoint + _windowSize))
            //        {
            //            Double yvalue = _ecg_Baseline_Data.SignalsFiltered.First(a => a.Item1 == "II").Item2[val];
            //            if (yvalue > 0)
            //            {
            //                yvalue += 3;
            //            }
            //            else
            //            {
            //                yvalue -= 6;
            //            }
            //            CurrentPlot.Annotations.Add(new TextAnnotation { Text = t.Item1, TextPosition = new DataPoint(val, yvalue) });                       
            //            //heartClassSeries.Points.Add(new ScatterPoint { X = val, Y = yvalue, Size = 3 });
            //            //addHeartClass = true;
            //        }
            //    }
            //}

            foreach (var t in _hear_Class_Data_Trans)
            {

                foreach (int val in t.Item2)
                {
                    if (val <= (_analyseSamples))
                    {
                        Double yvalue = _ecg_Baseline_Data.SignalsFiltered.First(a => a.Item1 == "II").Item2[val];
                        if (yvalue > 0)
                        {
                            yvalue += 3;
                        }
                        else
                        {
                            yvalue -= 6;
                        }
                        CurrentPlot.Annotations.Add(new TextAnnotation { Text = t.Item1, TextPosition = new DataPoint(val/_analyseFrequency, yvalue) });
                        //heartClassSeries.Points.Add(new ScatterPoint { X = val, Y = yvalue, Size = 3 });
                        //addHeartClass = true;
                    }
                }
            }



            //if(addHeartClass)
            //{
            //   CurrentPlot.Series.Add(heartClassSeries);
            //}

            RefreshPlot();
        }
        
        private void DisplayQt_Disp()
        {
            foreach (var signal in _ecg_Baseline_Data.SignalsFiltered)
            {

                Vector<double> signalVector = signal.Item2;
                List<int> waveList = new List<int>();

                var _t_EndLocal = _qt_Disp_Data.T_End_Local.Find(a => a.Item1 == signal.Item1).Item2;

                bool addWave = false;

                ScatterSeries waveSeries = new ScatterSeries();
                //waveSeries.Title = wavePart + "_" + signal.Item1;
                waveSeries.Title = "TEnd_local" + signal.Item1; 
                waveSeries.IsVisible = _visible;



                //for (int i = _beginingPoint; (i <= (_beginingPoint + _windowSize) && i < signalVector.Count()); i++)
                //{

                //    if (_t_EndLocal.Contains(i) && i>0)
                //    {
                //        waveSeries.Points.Add(new ScatterPoint { X = i, Y = signalVector[i], Size = 3 });

                //        addWave = true;
                //    }
                //}


                for (int i = _beginingPoint; (i <= _analyseSamples && i < signalVector.Count()); i++)
                {

                    if (_t_EndLocal.Contains(i) && i > 0)
                    {
                        waveSeries.Points.Add(new ScatterPoint { X = i/_analyseFrequency, Y = signalVector[i], Size = 3 });

                        addWave = true;
                    }
                }

                if (addWave)
                {
                    CurrentPlot.Series.Add(waveSeries);
                }
            }

            RefreshPlot();
        }

        public void DisplayHeartAxis(double heartAxis)
        {
            //ScatterSeries zeroSeries = new ScatterSeries();
            //zeroSeries.IsVisible = true; 
            //zeroSeries.Points.Add
            var lineraYAxis = new LinearAxis();
            lineraYAxis.Position = AxisPosition.Left;
            lineraYAxis.Minimum = -110.0;
            lineraYAxis.Maximum = 112.0;
            lineraYAxis.IsAxisVisible = false;
            //lineraYAxis.MajorGridlineStyle = LineStyle.Solid;
            //lineraYAxis.MinorGridlineStyle = LineStyle.Dot;

            var lineraXAxis = new LinearAxis();
            lineraXAxis.Position = AxisPosition.Bottom;
            lineraXAxis.Minimum = -120.0;
            lineraXAxis.Maximum = 110.0;
            lineraXAxis.IsAxisVisible = false;
            //lineraXAxis.MajorGridlineStyle = LineStyle.Solid;
            //lineraXAxis.MinorGridlineStyle = LineStyle.Dot;

            CurrentPlot.Axes.Add(lineraYAxis);
            CurrentPlot.Axes.Add(lineraXAxis);

            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "0", TextPosition = new DataPoint(102, -3), StrokeThickness = 0 });
            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "+-180", TextPosition = new DataPoint(-110, -3), StrokeThickness = 0 });
            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "-90", TextPosition = new DataPoint(-1, 102), StrokeThickness = 0 });
            CurrentPlot.Annotations.Add(new TextAnnotation { Text = "90", TextPosition = new DataPoint(0, -107),StrokeThickness = 0 });

            ArrowAnnotation arrow = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(100 * Math.Cos(heartAxis), -100 * Math.Sin(heartAxis)),
                Text = heartAxis.ToString("0.000") + "rad"
            };

            ArrowAnnotation yup = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(0, 100),
                Color = OxyColor.Parse("#000000"),
                StrokeThickness = 0.2
            };
            ArrowAnnotation ydown = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(0, -100),
                Color = OxyColor.Parse("#000000"),
                StrokeThickness = 0.2
            };
            ArrowAnnotation xleft = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(-100, 0),
                Color = OxyColor.Parse("#000000"),
                StrokeThickness = 0.2
            };

            ArrowAnnotation xright = new ArrowAnnotation
            {
                StartPoint = new DataPoint(0, 0),
                EndPoint = new DataPoint(100, 0),
                Color = OxyColor.Parse("#000000"),
                StrokeThickness = 0.2
            };

            CurrentPlot.Annotations.Add(yup);
            CurrentPlot.Annotations.Add(ydown);
            CurrentPlot.Annotations.Add(xleft);
            CurrentPlot.Annotations.Add(xright);
            CurrentPlot.Annotations.Add(arrow);
            RefreshPlot();
        }




        //public void SeriesControler(string seriesName, bool visible)
        //{

        //    try
        //    {

        //        if (seriesName == "R_Peaks")
        //        {
        //            _baselineDisplayedSeries[seriesName] = visible;

        //            foreach(var sig in _baselineDisplayedSeries)
        //            {
        //                if(sig.Value!=visible)
        //                {
        //                    CurrentPlot.Series.First(a => a.Title == sig.Key).IsVisible = visible;
        //                    _baselineDisplayedSeries[sig.Key] = visible;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var ser in CurrentPlot.Series)
        //            {
        //                if (_baselineDisplayedSeries["R_Peaks"])
        //                {
        //                    if (ser.Title == seriesName || ser.Title == "R_Peaks_" + seriesName)
        //                    {
        //                        ser.IsVisible = visible;
        //                        _baselineDisplayedSeries[seriesName] = visible;
        //                    }
        //                }
        //                else
        //                {
        //                    if (ser.Title == seriesName)
        //                    {
        //                        ser.IsVisible = visible;
        //                        _baselineDisplayedSeries[seriesName] = visible;
        //                    }
        //                }
        //            }
        //        }

        //        RefreshPlot();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        
        public bool RemoveAllPlotSeries()
        {
            try
            {
                CurrentPlot.Series.Clear();
                CurrentPlot.Annotations.Clear();
                RefreshPlot();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SeriesControler(string seriesName, bool visible)
        {
            //SavePlot();
            try
            {
                if(seriesName == "HeartClass")
                {
                    if (visible)
                    {
                        _annotations[seriesName] = true;
                        if (_baselineDisplayedSeries["II"])
                        {
                            DisplayHeartClass();                          
                        }
                    }
                    else
                    {
                        CurrentPlot.Annotations.Clear();
                        _annotations[seriesName] = false;
                    }
                }

                foreach (var oS in _otherDisplayedSeries)
                {
                    if (oS.Key == seriesName)
                    {
                        _otherDisplayedSeries[seriesName] = visible;
                        OtherSeries(seriesName, visible);
                    }
                }

                foreach (var bS in _baselineDisplayedSeries)
                {
                    if(bS.Key == seriesName)
                    {
                        MainSeries(seriesName, visible);
                    }
                }


                RefreshPlot();
            }
            catch (Exception ex)
            {

            }
        }


        private void MainSeries(string seriesN, bool isvisible)
        {

            if (isvisible)
            {
                if (seriesN == "II")
                {
                    if (_annotations["HeartClass"] == true)
                        DisplayHeartClass();
                }

                foreach (var oS in _otherDisplayedSeries.Where(a => a.Value == isvisible))
                {
                   
                    foreach (var ser in CurrentPlot.Series)
                    {
                        if (ser.Title == seriesN || ser.Title == (oS.Key + seriesN))
                        {
                            ser.IsVisible = isvisible;
                            _baselineDisplayedSeries[seriesN] = isvisible;

                        }
                    }
                }
            }
            else
            {
                if (seriesN == "II")
                {
                    if (_annotations["HeartClass"] == true)
                        CurrentPlot.Annotations.Clear();
                }

                foreach (var oS in _otherDisplayedSeries)
                {
                    foreach (var ser in CurrentPlot.Series)
                    {
                        if (ser.Title == seriesN || ser.Title == (oS.Key + seriesN))
                        {
                            ser.IsVisible = isvisible;
                            _baselineDisplayedSeries[seriesN] = isvisible;

                        }
                    }
                }
            }


            foreach (var ser in CurrentPlot.Series)
            {

                if (ser.Title == seriesN)
                {
                    ser.IsVisible = isvisible;
                    _baselineDisplayedSeries[seriesN] = isvisible;
                }
            }

            RefreshPlot();      
        }

        private void OtherSeries(string seriesN, bool isvisible)
        {
            if (isvisible)
            {
               
                foreach (var mS in _baselineDisplayedSeries)
                {
                    if (mS.Value == isvisible)
                    {
                        foreach (var ser in CurrentPlot.Series)
                        {
                            if (ser.Title.StartsWith(seriesN) && ser.Title.EndsWith(mS.Key))
                            {
                                if (ser.Title.IndexOf(mS.Key) == ser.Title.LastIndexOf(mS.Key))
                                {
                                    ser.IsVisible = isvisible;
                                }
                            }
                        }


                    }
                }
            }
            else
            {
                foreach (var mS in _baselineDisplayedSeries)
                {
                    
                        foreach (var ser in CurrentPlot.Series)
                        {
                            if (ser.Title.StartsWith(seriesN) && ser.Title.EndsWith(mS.Key))
                            {
                                if (ser.Title.IndexOf(mS.Key) == ser.Title.LastIndexOf(mS.Key))
                                {
                                    ser.IsVisible = isvisible;
                                }
                            }
                        }
                    
                }
            }

            RefreshPlot();
        }



        public void XAxesControl(double slide)
        {

            CurrentPlot.Axes.Remove(CurrentPlot.Axes.First(a => a.Title == "Time [s]"));
            double min;
            double max;
            bool noMore = true;

            //double windowsSize = _windowSize * _scalingPlotValue;
            double windowsSize = _maxSeriesTime;
            if (slide == 0)
            {
                min = 0;
                max = _windowSize;
            }
            else if (slide > 0.9)
            {
                max = windowsSize;
                min = max - _windowSize;
                if (slide == 1)
                {
                    System.Windows.MessageBoxResult msgR = System.Windows.MessageBox.Show("Do you want to visualise more data?", "", System.Windows.MessageBoxButton.YesNo);
                    if(msgR == System.Windows.MessageBoxResult.Yes)
                    {
                        //CalculateAmoutOfProcessingSamplesWhenAskedToReadMore();
                        //noMore = false;

                    }
                }
            }
            else
            {
                min = windowsSize * slide;
                max = min + _windowSize;
            }
            if (noMore)
            {
                var lineraXAxis = new LinearAxis();
                lineraXAxis.Position = AxisPosition.Bottom;
                lineraXAxis.Minimum = min;
                lineraXAxis.Maximum = max;
                lineraXAxis.MajorGridlineStyle = LineStyle.Solid;
                lineraXAxis.MinorGridlineStyle = LineStyle.Dot;
                lineraXAxis.Title = "Time [s]";

                CurrentPlot.Axes.Add(lineraXAxis);
            }
            RefreshPlot();
        }



        public void SavePlot()
        {
            try {
                string filename;
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                //dlg.DefaultExt = ".svg";
                //dlg.Filter = "SVG documents (.svg)|*.svg";
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "PDF documents (.pdf)|*.pdf";
                if (dlg.ShowDialog() == true)
                {
                    filename = dlg.FileName;

                    //using (var stream = System.IO.File.Create(filename))
                    //{
                    //    var exporter = new SvgExporter() { Width = 600, Height = 400 };
                    //    exporter.Export(CurrentPlot, stream);
                    //}                  

                    using (var stream = System.IO.File.Create(filename))
                    {
                        var pdfExporter = new PdfExporter() { Width = 600, Height = 400 };
                        pdfExporter.Export(CurrentPlot, stream);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void SavePlotToPdf()
        {
            try
            {
                    string automaticFilePath = System.IO.Directory.GetCurrentDirectory();
                    //System.Windows.MessageBox.Show(automaticFilePath);
                    //System.Windows.MessageBox.Show(automaticFilePath.IndexOf("DadmProject").ToString());
                    //System.Windows.MessageBox.Show(automaticFilePath.Remove(automaticFilePath.IndexOf("DadmProject")+12));

                    automaticFilePath = automaticFilePath.Remove(automaticFilePath.IndexOf("DadmProject") + 12) + @"IO\temp";
                    //System.Windows.MessageBox.Show(automaticFilePath);

                    string automaticFileName = @_currentAnalysisName + "_" + CurrentPlot.Title + _otherTabs + "_" + _currentSavedPlotNumber.ToString() + ".pdf";
                    string combinedPath = System.IO.Path.Combine(automaticFilePath, automaticFileName);
                    _currentSavedPlotNumber++;

                    using (var stream = System.IO.File.Create(combinedPath))
                    {
                        var pdfExporter = new PdfExporter() { Width = 600, Height = 400 };
                        pdfExporter.Export(CurrentPlot, stream);
                    }
                System.Windows.MessageBox.Show("Current Plot saved in Pdf file." + System.Environment.NewLine + "It will be joined to PDF Raport.");
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void SavePlot(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string filename;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.DefaultExt = ".svg";
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF documents (.pdf)|*.pdf";
            if (dlg.ShowDialog() == true)
            {
                filename = dlg.FileName;

                //using (var stream = System.IO.File.Create(filename))
                //{
                //    var exporter = new SvgExporter() { Width = 600, Height = 400 };
                //    exporter.Export(CurrentPlot, stream);
                //}

                using (var stream = System.IO.File.Create(filename))
                {
                    var pdfExporter = new PdfExporter() { Width = 600, Height = 400 };
                    pdfExporter.Export(CurrentPlot, stream);
                }

            }



        }

        //ostatnio developowana wersja end















        public void UploadControler(string whatToDisplay, List<Tuple<string, Vector<double>>> dataToDisplay)
        {
            switch (whatToDisplay)
            {
                case "ecgBaseline":
                    //DisplayEcgBaseline(dataToDisplay);
                    break;

                case "ecgBasic":
                    //DisplayBasicData(dataToDisplay);
                    break;

                case "r_Peaks":
                    _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
                    _ecg_Baseline_Data_worker.Load();
                    _ecg_Baseline_Data_worker.Data.SignalsFiltered = dataToDisplay;
                    //_ecg_Baseline_Data_worker.Data.SignalsFiltered = dataToDisplay; 
                    //DisplayR_Peaks(dataToDisplay);
                    break;

                default:
                    break;
            }

        }

        //public void UploadControler(string whatToDisplay, List<Tuple<string, Vector<double>>> dataToDisplay)
        //{
        //    switch (whatToDisplay)
        //    {
        //        case "ecgBaseline":
        //            //DisplayEcgBaseline(dataToDisplay);
        //            break;

        //        case "ecgBasic":
        //            //DisplayBasicData(dataToDisplay);
        //            break;

        //        case "r_Peaks":
        //            _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
        //            _ecg_Baseline_Data_worker.Load();
        //            _ecg_Baseline_Data_worker.Data.SignalsFiltered = dataToDisplay;
        //            //_ecg_Baseline_Data_worker.Data.SignalsFiltered = dataToDisplay; 
        //            //DisplayR_Peaks(dataToDisplay);
        //            break;

        //        default:
        //            break;
        //    }

        //}

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
                Fill = OxyColors.WhiteSmoke,
                
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