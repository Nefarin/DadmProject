using EKG_Project.IO;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for VisualisationPlotControl.xaml
    /// </summary>
    public partial class VisualisationPlotControl : UserControl
    {
        private ECGPlot ecgPlot;
        private string _plotType;
        private List<string> _seriesName;
        private List<CheckBox> _seriesChecbox;

        //
        //private List<IECG_Worker> _moduleWorkerList;
        //private Dictionary<string, IECG_Worker> _moduleWork;
        //private ECG_Worker ecg_Worker;
        //

        private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        private Basic_Data_Worker _ecg_Basic_Data_Worker;
        private R_Peaks_Data_Worker _r_Peaks_Data_Worker;
        private Waves_Data_Worker _waves_Data_Worker;
        private Dictionary<string, List<Tuple<string, Vector<double>>>> _wholeDataToDisplay; 


        public VisualisationPlotControl()
        {
            //InitializeComponent();
            //ecgPlot = new ECGPlot("ECG_BASELINE");
            //DataContext = ecgPlot;
            //ecgPlot.DisplayBasicData();
            ////ecgPlot.DisplayEcgBaseline();
            ////ecgPlot.DisplayR_Peaks();
        }

        public VisualisationPlotControl(string moduleName)
        {
            InitializeComponent();
            _plotType = moduleName;
            _seriesName = new List<string>();
            _seriesChecbox = new List<CheckBox>();
            //
            //_moduleWorkerList = new List<IECG_Worker>();
            //_moduleWork = new Dictionary<string, IECG_Worker>();

            //switch (moduleName)
            //{
            //    case "ecgBaseline":
            //        _moduleWork.Add(moduleName, new ECG_Baseline_Data_Worker());
            //        break;

            //    case "ecgBasic":
            //        _moduleWork.Add(moduleName, new Basic_Data_Worker());
            //        break;
            //    case "r_Peaks":
            //        _moduleWork.Add(moduleName, new R_Peaks_Data_Worker());
            //        break;

            //    default:
            //        break;
            //}

            //foreach (var mod in _moduleWork)
            //{
            //    mod.Value.Load();
            //}
            //


            ecgPlot = new ECGPlot(moduleName);
            DataContext = ecgPlot;            
            //ecgPlot.DisplayControler(_plotType);



            switch (moduleName)
            {
                case "ecgBaseline":
                    _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
                    _ecg_Baseline_Data_worker.Load();
                    foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
                    {
                        _seriesName.Add(signal.Item1);
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = true;
                        cB.Name = signal.Item1;
                        cB.Content = signal.Item1;
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                        //Debug.WriteLine(signal.Item1);
                    }
                    ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                    break;

                case "ecgBasic":
                    _ecg_Basic_Data_Worker = new Basic_Data_Worker();
                    _ecg_Basic_Data_Worker.Load();
                    foreach (var signal in _ecg_Basic_Data_Worker.BasicData.Signals)
                    {
                        _seriesName.Add(signal.Item1);
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = true;
                        cB.Name = signal.Item1;
                        cB.Content = signal.Item1;
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                        //Debug.WriteLine(signal.Item1);
                    }
                    ecgPlot.DisplayControler(_plotType, _ecg_Basic_Data_Worker.BasicData.Signals);
                    break;
                case "r_Peaks":
                    _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
                    _ecg_Baseline_Data_worker.Load();
                    foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
                    {
                        _seriesName.Add(signal.Item1);
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = true;
                        cB.Name = "R_Peak_"+signal.Item1;
                        cB.Content = "R_Peak_"+signal.Item1;
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                        //Debug.WriteLine(signal.Item1);
                    }
                    _r_Peaks_Data_Worker = new R_Peaks_Data_Worker();
                    _r_Peaks_Data_Worker.Load();
                    foreach (var signal in _r_Peaks_Data_Worker.Data.RPeaks)
                    {
                        _seriesName.Add(signal.Item1);
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = true;
                        cB.Name = signal.Item1;
                        cB.Content = signal.Item1;
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                        //Debug.WriteLine(signal.Item1);
                    }
                    ecgPlot.UploadControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                    ecgPlot.DisplayControler(_plotType, _r_Peaks_Data_Worker.Data.RPeaks);
                    break;
                //case "waves":
                //    _waves_Data_Worker = new Waves_Data_Worker();
                //    _waves_Data_Worker.Load();
                //    foreach (var a in _waves_Data_Worker.Data.PEnds)
                //    {

                //    }
                //    foreach (var sig in _waves_Data_Worker.Data.)
                //        break;
                default:
                    {
                        bool first = true; 
                        _wholeDataToDisplay = new Dictionary<string, List<Tuple<string, Vector<double>>>>();

                        _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
                        _ecg_Baseline_Data_worker.Load();
                        foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
                        {
                            _seriesName.Add(signal.Item1);
                            CheckBox cB = new CheckBox();
                            cB.IsChecked = first;
                            first = false;
                            cB.Name = signal.Item1;
                            cB.Content = signal.Item1;
                            cB.Checked += CheckBox_Checked;
                            cB.Unchecked += CheckBox_Unchecked;
                            _seriesChecbox.Add(cB);
                            //Debug.WriteLine(signal.Item1);
                        }
                        _wholeDataToDisplay.Add("ecgBaseline", _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                        //ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);

                        _r_Peaks_Data_Worker = new R_Peaks_Data_Worker();
                        _r_Peaks_Data_Worker.Load();
                        CheckBox rPCB = new CheckBox();
                        rPCB.IsChecked = first;
                        rPCB.Name = "R_Peaks";
                        rPCB.Content = "R_Peaks";
                        rPCB.Checked += CheckBox_Checked;
                        rPCB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(rPCB);
                        _wholeDataToDisplay.Add("r_Peaks", _r_Peaks_Data_Worker.Data.RPeaks);

                        ecgPlot.DisplayControler(_wholeDataToDisplay);
                 



                        break;
                    }
            }



            this.CheckBoxList.DataContext = _seriesChecbox;



        }

        private void PlotForwardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ecgPlot.MovePlot(500);
                switch(_plotType)
                {
                    case "ecgBaseline":
                        ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                        break;
                    case "ecgBasic":
                        ecgPlot.DisplayControler(_plotType, _ecg_Basic_Data_Worker.BasicData.Signals);
                        break;
                    case "r_Peaks":
                        ecgPlot.UploadControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                        ecgPlot.DisplayControler(_plotType, _r_Peaks_Data_Worker.Data.RPeaks);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void PlotBackwardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ecgPlot.MovePlot(-500);
                switch (_plotType)
                {
                    case "ecgBaseline":
                        ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                        break;
                    case "ecgBasic":
                        ecgPlot.DisplayControler(_plotType, _ecg_Basic_Data_Worker.BasicData.Signals);
                        break;
                    case "r_Peaks":
                        ecgPlot.UploadControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
                        ecgPlot.DisplayControler(_plotType, _r_Peaks_Data_Worker.Data.RPeaks);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var c = sender as CheckBox;
            ecgPlot.SeriesControler(c.Name, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var c = sender as CheckBox;
            ecgPlot.SeriesControler(c.Name, false);

        }
    }
}
