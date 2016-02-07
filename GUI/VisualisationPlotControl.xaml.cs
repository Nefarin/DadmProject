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
        private List<string> _chosenModules;
        private List<CheckBox> _seriesChecbox;
        private uint _analysisFreq;
        private uint _analysisSampleAmount;

        //
        //private List<IECG_Worker> _moduleWorkerList;
        //private Dictionary<string, IECG_Worker> _moduleWork;
        //private ECG_Worker ecg_Worker;
        //

        private ECG_Baseline_Data_Worker _ecg_Baseline_Data_worker;
        private Basic_Data_Worker _ecg_Basic_Data_Worker;
        private R_Peaks_Data_Worker _r_Peaks_Data_Worker;
        private Waves_Data_Worker _waves_Data_Worker;
        private Heart_Class_Data_Worker _hear_Class_Data_Worker;
        private Sleep_Apnea_Data_Worker _sleep_Apnea_Data_Worker;
        private Atrial_Fibr_Data_Worker _atrial_Fibr_Data_Worker;
        private Flutter_Data_Worker _flutter_Data_Worker;
        private QT_Disp_Data_Worker _qt_Disp_Data_Worker;
        private Heart_Axis_Data_Worker _hear_Axis_Data_Worker;
        private Dictionary<string, List<Tuple<string, Vector<double>>>> _wholeDataToDisplay;
        private Dictionary<string, List<Tuple<string, List<int>>>> _wholeDataToDisplayList;
        private bool first;

        //ver 2.0 
        private List<string> leadsNameList;
        private string firstLead; 
        //private 


        public VisualisationPlotControl()
        {
            //InitializeComponent();
            //ecgPlot = new ECGPlot("ECG_BASELINE");
            //DataContext = ecgPlot;
            //ecgPlot.DisplayBasicData();
            ////ecgPlot.DisplayEcgBaseline();
            ////ecgPlot.DisplayR_Peaks();
        }

        public VisualisationPlotControl(string analyseName ,string moduleName, KeyValuePair<string, int> moduleInfo)
        {
            InitializeComponent();
            _plotType = moduleName;
            _seriesName = new List<string>();
            _seriesChecbox = new List<CheckBox>();

            _wholeDataToDisplay = new Dictionary<string, List<Tuple<string, Vector<double>>>>();
            _wholeDataToDisplayList = new Dictionary<string, List<Tuple<string, List<int>>>>();
            first = true;


            ecgPlot = new ECGPlot(moduleName);
            DataContext = ecgPlot;


            switch (moduleInfo.Value)
            {
                case 0:
                    //MessageBox.Show("Analise " + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;

                case 1:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 2:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 3:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 4:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    Get_HEART_CLASS_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 5:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    Get_SLEEP_APNEA_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 6:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    Get_HEART_CLASS_Data(analyseName);
                    Get_SLEEP_APNEA_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 7:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    //ATRIAL_FIBER
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 8:
                    Get_ECG_BASELINE_Data(analyseName);
                    Get_ECG_BASIC_Data(analyseName);
                    Get_R_PEAKS_Data(analyseName);
                    Get_WAVES_Data(analyseName);
                    Get_QT_DISP_Data(analyseName);
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;
                case 9 :
                    Get_HEART_AXIS_Data(analyseName);
                    //this.PlotForwardButton.Visibility = Visibility.Collapsed;
                    //this.PlotBackwardButton.Visibility = Visibility.Collapsed;
                    this.PlotSlider.Visibility = Visibility.Collapsed;
                    this.CheckBoxList.Visibility = Visibility.Collapsed;
                   //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;


                default:
                    //MessageBox.Show("analyseName=" + analyseName + ", moduleName=" + moduleName + ", moduleInfoKey=" + moduleInfo.Key + "=" + moduleInfo.Value);
                    break;

            }



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


            //ecgPlot.DisplayControler(_plotType);



            //switch (moduleName)
            //{
            //    case "ecgBaseline":
            //        _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
            //        _ecg_Baseline_Data_worker.Load();
            //        foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            //        {
            //            _seriesName.Add(signal.Item1);
            //            CheckBox cB = new CheckBox();
            //            cB.IsChecked = true;
            //            cB.Name = signal.Item1;
            //            cB.Content = signal.Item1;
            //            cB.Checked += CheckBox_Checked;
            //            cB.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(cB);
            //            //Debug.WriteLine(signal.Item1);
            //        }
            //        ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
            //        break;

            //    case "ecgBasic":
            //        _ecg_Basic_Data_Worker = new Basic_Data_Worker();
            //        _ecg_Basic_Data_Worker.Load();
            //        foreach (var signal in _ecg_Basic_Data_Worker.BasicData.Signals)
            //        {
            //            _seriesName.Add(signal.Item1);
            //            CheckBox cB = new CheckBox();
            //            cB.IsChecked = true;
            //            cB.Name = signal.Item1;
            //            cB.Content = signal.Item1;
            //            cB.Checked += CheckBox_Checked;
            //            cB.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(cB);
            //            //Debug.WriteLine(signal.Item1);
            //        }
            //        ecgPlot.DisplayControler(_plotType, _ecg_Basic_Data_Worker.BasicData.Signals);
            //        break;
            //    case "r_Peaks":
            //        _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
            //        _ecg_Baseline_Data_worker.Load();
            //        foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            //        {
            //            _seriesName.Add(signal.Item1);
            //            CheckBox cB = new CheckBox();
            //            cB.IsChecked = true;
            //            cB.Name = "R_Peak_"+signal.Item1;
            //            cB.Content = "R_Peak_"+signal.Item1;
            //            cB.Checked += CheckBox_Checked;
            //            cB.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(cB);
            //            //Debug.WriteLine(signal.Item1);
            //        }
            //        _r_Peaks_Data_Worker = new R_Peaks_Data_Worker();
            //        _r_Peaks_Data_Worker.Load();
            //        foreach (var signal in _r_Peaks_Data_Worker.Data.RPeaks)
            //        {
            //            _seriesName.Add(signal.Item1);
            //            CheckBox cB = new CheckBox();
            //            cB.IsChecked = true;
            //            cB.Name = signal.Item1;
            //            cB.Content = signal.Item1;
            //            cB.Checked += CheckBox_Checked;
            //            cB.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(cB);
            //            //Debug.WriteLine(signal.Item1);
            //        }
            //        ecgPlot.UploadControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);
            //        ecgPlot.DisplayControler(_plotType, _r_Peaks_Data_Worker.Data.RPeaks);
            //        break;
            //    //case "waves":
            //    //    _waves_Data_Worker = new Waves_Data_Worker();
            //    //    _waves_Data_Worker.Load();
            //    //    foreach (var a in _waves_Data_Worker.Data.PEnds)
            //    //    {

            //    //    }
            //    //    foreach (var sig in _waves_Data_Worker.Data.)
            //    //        break;
            //    default:
            //        {
            //            first = true; 
            //            _wholeDataToDisplay = new Dictionary<string, List<Tuple<string, Vector<double>>>>();
            //            _wholeDataToDisplayList = new Dictionary<string, List<Tuple<string, List<int>>>>();

            //            _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker();
            //            _ecg_Baseline_Data_worker.Load();
            //            foreach (var signal in _ecg_Baseline_Data_worker.Data.SignalsFiltered)
            //            {
            //                _seriesName.Add(signal.Item1);
            //                CheckBox cB = new CheckBox();
            //                cB.IsChecked = first;
            //                first = false;
            //                cB.Name = signal.Item1;
            //                cB.Content = signal.Item1;
            //                cB.Checked += CheckBox_Checked;
            //                cB.Unchecked += CheckBox_Unchecked;
            //                _seriesChecbox.Add(cB);
            //                //Debug.WriteLine(signal.Item1);
            //            }
            //            _wholeDataToDisplay.Add("ecgBaseline", _ecg_Baseline_Data_worker.Data.SignalsFiltered);
            //            //ecgPlot.DisplayControler(_plotType, _ecg_Baseline_Data_worker.Data.SignalsFiltered);

            //            _r_Peaks_Data_Worker = new R_Peaks_Data_Worker();
            //            _r_Peaks_Data_Worker.Load();
            //            CheckBox rPCB = new CheckBox();
            //            rPCB.IsChecked = first;
            //            rPCB.Name = "R_Peaks";
            //            rPCB.Content = "R_Peaks";
            //            rPCB.Checked += CheckBox_Checked;
            //            rPCB.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(rPCB);
            //            _wholeDataToDisplay.Add("r_Peaks", _r_Peaks_Data_Worker.Data.RPeaks);


            //            _waves_Data_Worker = new Waves_Data_Worker();
            //            _waves_Data_Worker.Load();
            //            CheckBox qRSOnsets = new CheckBox();
            //            qRSOnsets.IsChecked = first;
            //            qRSOnsets.Name = "QRSOnsets";
            //            qRSOnsets.Content = "QRSOnsets";
            //            qRSOnsets.Checked += CheckBox_Checked;
            //            qRSOnsets.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(qRSOnsets);

            //            _wholeDataToDisplayList.Add("QRSOnsets", _waves_Data_Worker.Data.QRSOnsets);


            //            CheckBox qRSEnds = new CheckBox();
            //            qRSEnds.IsChecked = first;
            //            qRSEnds.Name = "QRSEnds";
            //            qRSEnds.Content = "QRSEnds";
            //            qRSEnds.Checked += CheckBox_Checked;
            //            qRSEnds.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(qRSEnds);
            //            _wholeDataToDisplayList.Add("QRSEnds", _waves_Data_Worker.Data.QRSEnds);





            //            CheckBox pOnsets = new CheckBox();
            //            pOnsets.IsChecked = first;
            //            pOnsets.Name = "POnsets";
            //            pOnsets.Content = "POnsets";
            //            pOnsets.Checked += CheckBox_Checked;
            //            pOnsets.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(pOnsets);
            //            _wholeDataToDisplayList.Add("POnsets", _waves_Data_Worker.Data.POnsets);



            //            CheckBox pEnds = new CheckBox();
            //            pEnds.IsChecked = first;
            //            pEnds.Name = "PEnds";
            //            pEnds.Content = "PEnds";
            //            pEnds.Checked += CheckBox_Checked;
            //            pEnds.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(pEnds);
            //            _wholeDataToDisplayList.Add("PEnds", _waves_Data_Worker.Data.PEnds);

            //            CheckBox tEnds = new CheckBox();
            //            tEnds.IsChecked = first;
            //            tEnds.Name = "TEnds";
            //            tEnds.Content = "TEnds";
            //            tEnds.Checked += CheckBox_Checked;
            //            tEnds.Unchecked += CheckBox_Unchecked;
            //            _seriesChecbox.Add(tEnds);
            //            _wholeDataToDisplayList.Add("TEnds", _waves_Data_Worker.Data.TEnds);



            //            ecgPlot.DisplayControler(_wholeDataToDisplay);




            //            break;
            //        }
            //}


            if (moduleInfo.Value != 9)
            {
                this.CheckBoxList.DataContext = _seriesChecbox;
                ecgPlot.DisplayControler(_wholeDataToDisplay, _wholeDataToDisplayList, _analysisFreq,_analysisSampleAmount);
            }

        }


        //Program ver 2.0 

        public VisualisationPlotControl(string analyseName, string moduleName, KeyValuePair<string, int> moduleInfo, List<string> modulesList)
        {
            InitializeComponent();
            _plotType = moduleName;
            _seriesName = new List<string>();
            _seriesChecbox = new List<CheckBox>();

            first = true;


            ecgPlot = new ECGPlot(analyseName, moduleName);
            DataContext = ecgPlot;
            

            CreateAllCheckBoxesInCurrentAnalyse(analyseName, modulesList);
            if (_plotType == "ECG_BASELINE")
            {
                ecgPlot.DisplayBaselineLeads(firstLead);
            }
            if (_plotType == "HRV2")
            {
                ecgPlot.DisplayHRV2Leads(firstLead);
            }


            this.CheckBoxList.DataContext = _seriesChecbox;

        }


        //methodes used to configurate Checboxes

        private void CreateAllCheckBoxesInCurrentAnalyse(string currentAnalyseName, List<string> currentModulesList)
        {
            try
            {
                Basic_New_Data_Worker basicDataForLeads = new Basic_New_Data_Worker(currentAnalyseName);
                leadsNameList = basicDataForLeads.LoadLeads();
                firstLead = leadsNameList.First();

                foreach (string lead in leadsNameList)
                {
                    CheckBox cB = new CheckBox();
                    cB.IsChecked = first;
                    first = false;
                    cB.Name = lead;
                    cB.Content = lead;                   
                    cB.Checked += CheckBox_Checked;
                    cB.Unchecked += CheckBox_Unchecked;
                    _seriesChecbox.Add(cB);
                }

                //for ecgBasic
                if (_plotType == "ECG_BASELINE")
                {
                    if (true)
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "Basic";
                        cB.Content = "Basic";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }

                    if (currentModulesList.Contains("R_PEAKS"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "RPeaks";
                        cB.Content = "RPeaks";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }
                    if (currentModulesList.Contains("WAVES"))
                    {
                        CheckBox qRSOnsets = new CheckBox();
                        qRSOnsets.IsChecked = first;
                        qRSOnsets.Name = "QRSOnsets";
                        qRSOnsets.Content = "QRSOnsets";
                        qRSOnsets.Checked += CheckBox_Checked;
                        qRSOnsets.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(qRSOnsets);

                        CheckBox qRSEnds = new CheckBox();
                        qRSEnds.IsChecked = first;
                        qRSEnds.Name = "QRSEnds";
                        qRSEnds.Content = "QRSEnds";
                        qRSEnds.Checked += CheckBox_Checked;
                        qRSEnds.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(qRSEnds);

                        CheckBox pOnsets = new CheckBox();
                        pOnsets.IsChecked = first;
                        pOnsets.Name = "POnsets";
                        pOnsets.Content = "POnsets";
                        pOnsets.Checked += CheckBox_Checked;
                        pOnsets.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(pOnsets);

                        CheckBox pEnds = new CheckBox();
                        pEnds.IsChecked = first;
                        pEnds.Name = "PEnds";
                        pEnds.Content = "PEnds";
                        pEnds.Checked += CheckBox_Checked;
                        pEnds.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(pEnds);

                        CheckBox tEnds = new CheckBox();
                        tEnds.IsChecked = first;
                        tEnds.Name = "TEnds";
                        tEnds.Content = "TEnds";
                        tEnds.Checked += CheckBox_Checked;
                        tEnds.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(tEnds);
                    }

                    if (currentModulesList.Contains("HEART_CLASS"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "HeartClass";
                        cB.Content = "HeartClass";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }

                    if (currentModulesList.Contains("QT_DISP"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "QTDisp";
                        cB.Content = "QTDisp";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }


                    if (currentModulesList.Contains("ATRIAL_FIBER"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "AtrialFiber";
                        cB.Content = "AtrialFiber";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }

                    if (currentModulesList.Contains("T_WAVE_ALT"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "TWaveAlt";
                        cB.Content = "TWaveAlt";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }

                    if (currentModulesList.Contains("FLUTTER"))
                    {
                        CheckBox cB = new CheckBox();
                        cB.IsChecked = first;
                        first = false;
                        cB.Name = "Flutter";
                        cB.Content = "Flutter";
                        cB.Checked += CheckBox_Checked;
                        cB.Unchecked += CheckBox_Unchecked;
                        _seriesChecbox.Add(cB);
                    }
                }
                else if(_plotType == "HRV2")
                {

                }


            }
            catch
            {

            }
        }
























        //methodes to get data for plotting
        private void Get_ECG_BASELINE_Data(string currentAnalyseName)
        {
            _ecg_Baseline_Data_worker = new ECG_Baseline_Data_Worker(currentAnalyseName);
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
            }
            _wholeDataToDisplay.Add("ecgBaseline", _ecg_Baseline_Data_worker.Data.SignalsFiltered);
        }

        private void Get_ECG_BASIC_Data(string currentAnalyseName)
        {
            _ecg_Basic_Data_Worker = new Basic_Data_Worker(currentAnalyseName);
            _ecg_Basic_Data_Worker.Load();
            _analysisFreq = _ecg_Basic_Data_Worker.BasicData.Frequency;
            _analysisSampleAmount = _ecg_Basic_Data_Worker.BasicData.SampleAmount; 
            

            //System.Windows.MessageBox.Show("frequency=" + _ecg_Basic_Data_Worker.BasicData.Frequency);
            //System.Windows.MessageBox.Show("samples=" + _ecg_Basic_Data_Worker.BasicData.SampleAmount);

            CheckBox cB = new CheckBox();
            cB.IsChecked = first;
            first = false;
            cB.Name = "Basic";
            cB.Content = "Basic";
            cB.Checked += CheckBox_Checked;
            cB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(cB);

            _wholeDataToDisplay.Add("ecgBasic", _ecg_Basic_Data_Worker.BasicData.Signals);
        }

        private void Get_R_PEAKS_Data(string currentAnalyseName)
        {
            _r_Peaks_Data_Worker = new R_Peaks_Data_Worker(currentAnalyseName);
            _r_Peaks_Data_Worker.Load();
            CheckBox rPCB = new CheckBox();
            rPCB.IsChecked = first;
            rPCB.Name = "R_Peaks";
            rPCB.Content = "R_Peaks";
            rPCB.Checked += CheckBox_Checked;
            rPCB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(rPCB);
            _wholeDataToDisplay.Add("r_Peaks", _r_Peaks_Data_Worker.Data.RPeaks);
        }

        private void Get_WAVES_Data(string currentAnalyseName)
        {
            _waves_Data_Worker = new Waves_Data_Worker(currentAnalyseName);
            _waves_Data_Worker.Load();
            CheckBox qRSOnsets = new CheckBox();
            qRSOnsets.IsChecked = first;
            qRSOnsets.Name = "QRSOnsets";
            qRSOnsets.Content = "QRSOnsets";
            qRSOnsets.Checked += CheckBox_Checked;
            qRSOnsets.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(qRSOnsets);
            _wholeDataToDisplayList.Add("QRSOnsets", _waves_Data_Worker.Data.QRSOnsets);


            CheckBox qRSEnds = new CheckBox();
            qRSEnds.IsChecked = first;
            qRSEnds.Name = "QRSEnds";
            qRSEnds.Content = "QRSEnds";
            qRSEnds.Checked += CheckBox_Checked;
            qRSEnds.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(qRSEnds);
            _wholeDataToDisplayList.Add("QRSEnds", _waves_Data_Worker.Data.QRSEnds);


            CheckBox pOnsets = new CheckBox();
            pOnsets.IsChecked = first;
            pOnsets.Name = "POnsets";
            pOnsets.Content = "POnsets";
            pOnsets.Checked += CheckBox_Checked;
            pOnsets.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(pOnsets);
            _wholeDataToDisplayList.Add("POnsets", _waves_Data_Worker.Data.POnsets);

            CheckBox pEnds = new CheckBox();
            pEnds.IsChecked = first;
            pEnds.Name = "PEnds";
            pEnds.Content = "PEnds";
            pEnds.Checked += CheckBox_Checked;
            pEnds.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(pEnds);
            _wholeDataToDisplayList.Add("PEnds", _waves_Data_Worker.Data.PEnds);

            CheckBox tEnds = new CheckBox();
            tEnds.IsChecked = first;
            tEnds.Name = "TEnds";
            tEnds.Content = "TEnds";
            tEnds.Checked += CheckBox_Checked;
            tEnds.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(tEnds);
            _wholeDataToDisplayList.Add("TEnds", _waves_Data_Worker.Data.TEnds);
        }
     
        private void Get_HEART_CLASS_Data(string currentAnalyseName)
        {
            _hear_Class_Data_Worker = new Heart_Class_Data_Worker(currentAnalyseName);
            _hear_Class_Data_Worker.Load();
            List<Tuple<string, List<int>>> outList = new List<Tuple<string, List<int>>>();
            List<Tuple<int, int>> tempList = _hear_Class_Data_Worker.Data.ClassificationResult;
            //foreach (var t in _hear_Class_Data_Worker.Data.ClassificationResult)
            //{
            //    MessageBox.Show(t.Item1.ToString());
            //    MessageBox.Show(t.Item2.ToString());
            //}
            foreach (var t in tempList)
            {
                List<int> tempIntList = new List<int>();
                if (t.Item2==0)
                {
                    //MessageBox.Show("V");
                    tempIntList.Add(t.Item1);
                    outList.Add(new Tuple<string, List<int>>("V", tempIntList));
                }
                else
                {
                    //MessageBox.Show("SV");
                    tempIntList.Add(t.Item1);
                    outList.Add(new Tuple<string, List<int>>("SV", tempIntList));
                }
            }

  
            CheckBox rPCB = new CheckBox();
            rPCB.IsChecked = first;
            rPCB.Name = "HeartClass";
            rPCB.Content = "HeartClass";
            rPCB.Checked += CheckBox_Checked;
            rPCB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(rPCB);
            _wholeDataToDisplayList.Add("HeartClass",outList);
        }

        private void Get_SLEEP_APNEA_Data(string currentAnalyseName)
        {
            _sleep_Apnea_Data_Worker = new Sleep_Apnea_Data_Worker(currentAnalyseName);
            _sleep_Apnea_Data_Worker.Load();

            CheckBox cB = new CheckBox();
            cB.IsChecked = first;
            first = false;
            cB.Name = "SleepApnea";
            cB.Content = "SleepApnea";
            cB.Checked += CheckBox_Checked;
            cB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(cB);

        }

        private void Get_ATRIAL_FIBER_Data(string currentAnalyseName)
        {
            _ecg_Basic_Data_Worker = new Basic_Data_Worker(currentAnalyseName);
            _ecg_Basic_Data_Worker.Load();
            _atrial_Fibr_Data_Worker = new Atrial_Fibr_Data_Worker();
            _atrial_Fibr_Data_Worker.Load();

            //_atrial_Fibr_Data_Worker.Data.
            CheckBox cB = new CheckBox();
            cB.IsChecked = first;
            first = false;
            cB.Name = "AtrialF";
            cB.Content = "AtrialF";
            cB.Checked += CheckBox_Checked;
            cB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(cB);

            //_wholeDataToDisplay.Add("ecgBasic", _ecg_Basic_Data_Worker.BasicData.Signals);
        }

        private void Get_FLUTTER_Data(string currentAnalyseName)
        {
            _flutter_Data_Worker = new Flutter_Data_Worker(currentAnalyseName);
            _flutter_Data_Worker.Load();

            //_flutter_Data_Worker.Data.

            //_atrial_Fibr_Data_Worker.Data.
            CheckBox cB = new CheckBox();
            cB.IsChecked = first;
            first = false;
            cB.Name = "Flutter";
            cB.Content = "Flutter";
            cB.Checked += CheckBox_Checked;
            cB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(cB);

            //_wholeDataToDisplay.Add("ecgBasic", _ecg_Basic_Data_Worker.BasicData.Signals);
        }

        private void Get_QT_DISP_Data(string currentAnalyseName)
        {
            _qt_Disp_Data_Worker = new QT_Disp_Data_Worker(currentAnalyseName);
            _qt_Disp_Data_Worker.Load();

            CheckBox rPCB = new CheckBox();
            rPCB.IsChecked = first;
            rPCB.Name = "TEnd_local";
            rPCB.Content = "TEnd_local";
            rPCB.Checked += CheckBox_Checked;
            rPCB.Unchecked += CheckBox_Unchecked;
            _seriesChecbox.Add(rPCB);

            _wholeDataToDisplayList.Add("TEnd_local", _qt_Disp_Data_Worker.Data.T_End_Local);
        }

        private void Get_HEART_AXIS_Data(string currentAnalyseName)
        {
            _hear_Axis_Data_Worker = new Heart_Axis_Data_Worker(currentAnalyseName);
            _hear_Axis_Data_Worker.Load();
            ecgPlot.DisplayHeartAxis(_hear_Axis_Data_Worker.Data.HeartAxis);

        }



        private void Slider_ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e)
        {

            var slider = sender as Slider;
            double value = slider.Value;
            ecgPlot.XAxesControl(value / 10);
            //MessageBox.Show(value.ToString());
      
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
            //MessageBox.Show("Checked=" + c.Name);
            if (leadsNameList.Contains(c.Name))
            {
                foreach (var cB in this.CheckBoxList.Items)
                {
                    var cc = cB as CheckBox;
                    if (cc.Name != c.Name)
                    {
                        cc.Checked -= CheckBox_Checked;
                        cc.Unchecked -= CheckBox_Unchecked;
                        cc.IsChecked = false;
                        cc.Checked += CheckBox_Checked;
                        cc.Unchecked += CheckBox_Unchecked;
                    }

                }
                ecgPlot.RemoveAllPlotSeries();
                //wyswietlenie żadnego leadu
                if (_plotType == "ECG_BASELINE")
                {
                    ecgPlot.DisplayBaselineLeads(c.Name);
                }
                if (_plotType == "HRV2")
                {
                    ecgPlot.DisplayHRV2Leads(c.Name);
                    this.PlotSlider.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                ecgPlot.ControlOtherModulesSeries(c.Name, true);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var c = sender as CheckBox;
            //MessageBox.Show("Unchecked=" + c.Name);
            if (leadsNameList.Contains(c.Name))
            {
                foreach (var cB in this.CheckBoxList.Items)
                {
                    var cc = cB as CheckBox;
                    cc.Checked -= CheckBox_Checked;
                    cc.Unchecked -= CheckBox_Unchecked;
                    cc.IsChecked = false;
                    cc.Checked += CheckBox_Checked;
                    cc.Unchecked += CheckBox_Unchecked;

                    ecgPlot.RemoveAllPlotSeries();           
                }
            }
            else
            {
                ecgPlot.ControlOtherModulesSeries(c.Name, false);
            }
        }


        private void SavePlotButton_Click(object sender, RoutedEventArgs e)
        {
            ecgPlot.SavePlot();
        }

        //private void ClearPlotButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //ListBox checkBoxes = this.CheckBoxList.DataContext;
        //    bool first = true;
        //    foreach(var cB in this.CheckBoxList.Items)
        //    {
        //        var c = cB as CheckBox;
        //        if (first)
        //        {
        //            ecgPlot.SeriesControler(c.Name, true);
                    
        //        }
        //        else
        //        {
        //            ecgPlot.SeriesControler(c.Name, false);
        //        }
                             
        //    }

        //}
    }
}
