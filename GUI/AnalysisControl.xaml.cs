using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using EKG_Project.Architecture;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Architecture.GUIMessages;
using EKG_Project.Modules;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for AnalysisControl.xaml
    /// Handling buttons (Play, Load file, Pdf path and Close)
    /// </summary>
    public partial class AnalysisControl : UserControl, INotifyPropertyChanged
    {
        private enum ABORT_MODE { ABORT_ANALYSIS, ABORT_LOADING_FILE, ABORT_STATS_CALC};
        private bool _analysisInProgress = false;

        public string outputPdfPath;
        public string inputFilePath;

        private ABORT_MODE _abortMode;

        /// <summary>
        /// Set bool parameter if analysis is in progress
        /// </summary>
        public bool AnalysisInProgress
        {
            
            get
            {
                return this._analysisInProgress;
            }
            set
            {
                this._analysisInProgress = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("AnalysisNotRunning");
            }
        }

        /// <summary>
        /// set bool value if analysis is running or not (opposite to bool param AnalysisInProgress)
        /// </summary>
        public bool AnalysisNotRunning { get { return !this.AnalysisInProgress; } }

        /// <summary>
        /// Handle event changes (has anything changed?) 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method which handles changes (has anything changed?) 
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method which handles exitButton, create new Abort method
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.SendGUIMessage(new Abort());
        }

        /// <summary>
        /// Allows user to load correct file from custom destination
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void loadFileButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Specify input file";
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = inputFilePath;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputFilePath = fileDialog.FileName;
                Communication.SendGUIMessage(new LoadFile(inputFilePath));
                panel.Visibility = Visibility.Visible;
                progressBar.IsIndeterminate = true;
                analysisLabel.Content = "Loading file.. Please wait";
                buttonAbort.Content = "Cancel";
                _abortMode = ABORT_MODE.ABORT_LOADING_FILE;
            }

        }

        /// <summary>
        /// Begin an analysis (if file is loaded) and user checked any analysis
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void startAnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var option in modulePanel.getAllOptions())
            {
                if (option.Set)
                {
                    moduleParams[option.Code] = modulePanel.ModuleOptionAndParams(option.Code).Item2;
                }

            }

            if (moduleParams.Count > 0)
            {
                isComputed = new Dictionary<AvailableOptions, bool>();
                BeginAnalysis analysisParams = new BeginAnalysis(moduleParams);
                Communication.SendGUIMessage(analysisParams);
                moduleParams = new Dictionary<AvailableOptions, ModuleParams>();
                _abortMode = ABORT_MODE.ABORT_ANALYSIS;
            }
            else
            {
                MessageBox.Show("No analysis selected. Please select at least one analysis.", "Cannot start calculations", MessageBoxButton.OK);
            }

        }

        /// <summary>
        /// Allows user to choose a destination folder of summary pdf file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pdfButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Title = "Specify output pdf file";
            fileDialog.Filter = "pdf file|*.pdf";
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = outputPdfPath;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Communication.SendGUIMessage(new BeginStatsCalculation(this.isComputed));
                outputPdfPath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Inform user if analysis has begun, start processing
        /// </summary>
        public void processingStarted()
        {
            this.VisualisationPanelUserControl.Visibility = Visibility.Hidden;
            panel.Visibility = Visibility.Visible;
            this.AnalysisInProgress = true;
            loadFileButton.IsEnabled = false;
            pdfButton.IsEnabled = false;
            modulePanel.IsEnabled = false;
            startAnalyseButton.IsEnabled = false;
            analysisLabel.Content = "Analysis in progress..";
            buttonAbort.Content = "Abort analysis";
        }

        /// <summary>
        /// Close analysis progress bar, inform that analysis has ended
        /// </summary>
        public void processingEnded()
        {
            this.AnalysisInProgress = false;
            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            foreach (var option in modulePanel.getAllOptions())
            {
                //Console.WriteLine(option.Name);
                if (option.Set)
                {
                    tempList.Add(option.Name);
                }
            }

            loadFileButton.IsEnabled = true;
            pdfButton.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            modulePanel.IsEnabled = true;
            VisualisationPanelUserControl.DataContext = new VisualisationPanelControl(modulePanel.AnalysisName, tempList);
            panel.Visibility = Visibility.Hidden;
            this.VisualisationPanelUserControl.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Update a progress bar with current analysis
        /// </summary>
        /// <param name="module">Name of module which is analysing</param>
        /// <param name="progress">% of progress in calculation</param>
        public void updateProgress(AvailableOptions module, double progress)
        {
            analysisLabel.Content = "Analysis in progress..\nCurrent module: " + module.ToString();
            try
            {
                progressBar.Value = progress;
            }
            catch (ArgumentException e)
            {
                progressBar.Value = 0;
            }

        }

        /// <summary>
        /// Inform that module calculations has ended (including a case in which it has been aborted)
        /// </summary>
        /// <param name="module">Module name</param>
        /// <param name="aborted">Bool vale shows if module has been abortet</param>
        public void moduleEnded(AvailableOptions module, bool aborted)
        {
            Console.WriteLine(module + " Ended");
            if (aborted)
            {

            }
            else
            {
                try
                {
                    isComputed.Add(module, true);
                }            
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// Handle Play Button
        /// </summary>
        public void fileLoaded()
        {
            startAnalyseButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File loaded successfully.");
        }
    
        /// <summary>
        /// Shows error in file loading - file error
        /// </summary>
        public void fileError()
        {
            startAnalyseButton.IsEnabled = false;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File could not be loaded successfully.");

        }

        /// <summary>
        /// Shows error in file loading - file not loaded
        /// </summary>
        public void fileNotLoaded()
        {
            startAnalyseButton.IsEnabled = false;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File not found during analysis.");

        }

        /// <summary>
        /// Starts generating a pdf summary file
        /// </summary>
        IO.PDFGenerator pdf;
        public void statsCalculationStarted()
        {
            MessageBox.Show("Started generating PDF");

            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            foreach (var option in modulePanel.getAllOptions())
            {

                if (option.Set)
                {
                    tempList.Add(option.Name);
                }
            }

            IO.PDF.StoreDataPDF data = new IO.PDF.StoreDataPDF();
            data.AnalisysName = modulePanel.AnalysisName;
            data.ModuleList = tempList;
            data.Filename = this.inputFilePath;

            pdf = new IO.PDFGenerator(outputPdfPath);
            pdf.GeneratePDF(data, true);
        }

        
        /// <summary>
        /// Ends generating a pdf summary file
        /// </summary>
        /// <param name="results"></param>
        public void statsCalculationEnded(Dictionary<AvailableOptions, Dictionary<String, String>> results)
        {
            foreach (var result in results)
            {
                Dictionary<String, String> temp = result.Value;
                foreach(var smth in temp)
                {
                    Console.WriteLine(smth.Key + " " + smth.Value);
                }
            }

            IO.PDF.StoreDataPDF data = new IO.PDF.StoreDataPDF();

            foreach (var element in results)
            {
                data.ModuleOption = element.Key;
                data.statsDictionary = element.Value;

                pdf.GeneratePDF(data, false);
            }
      
            MessageBox.Show("PDF generated");
            pdf.SaveDocument();
            pdf.ProcessStart();
        }


        /// <summary>
        /// Abort an analysis (by user or event)
        /// </summary>
        public void analysisAborted()
        {
            progressBar.Value = 0;
            MessageBox.Show("Analysis aborted.");
            loadFileButton.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows that loading a file has been aborted
        /// </summary>
        public void loadingAborted()
        {
            panel.Visibility = Visibility.Hidden;
            loadFileButton.IsEnabled = true;
            MessageBox.Show("Loading file aborted.");
        }

        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="message"></param>
        private void analyzeEvent(IGUIMessage message)
        {
            message.Read(this);
        }

        /// <summary>
        /// Method which handle abort button click (in which case it has been aborted)
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            switch(_abortMode)
            {
                case (ABORT_MODE.ABORT_ANALYSIS):
                    Communication.SendGUIMessage(new AbortAnalysis());
                    break;
                case (ABORT_MODE.ABORT_LOADING_FILE):
                    Communication.SendGUIMessage(new AbortLoadingFile());
                    break;
                case (ABORT_MODE.ABORT_STATS_CALC):
                    break;
            }       
        }
    }
}
