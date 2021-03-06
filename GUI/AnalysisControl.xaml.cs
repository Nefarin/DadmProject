﻿using System;
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
using System.Linq;

namespace EKG_Project.GUI
{
    /// <summary>
    /// Interaction logic for AnalysisControl.xaml
    /// Handling buttons
    /// </summary>
    public partial class AnalysisControl : UserControl, INotifyPropertyChanged
    {
        private enum ABORT_MODE { ABORT_ANALYSIS, ABORT_LOADING_FILE, ABORT_STATS_CALC};
        private bool _analysisInProgress = false;

        public string outputPdfPath;
        public string inputFilePath;
        IO.PDFGenerator pdf;

        private ABORT_MODE _abortMode;

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

        public bool AnalysisNotRunning { get { return !this.AnalysisInProgress; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.SendGUIMessage(new Abort());
        }

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
                //progressBar.IsIndeterminate = true;
                analysisLabel.Content = "Loading file.. Please wait";
                buttonAbort.Content = "Cancel";
                _abortMode = ABORT_MODE.ABORT_LOADING_FILE;
                this.VisualisationPanelUserControl.Visibility = Visibility.Hidden;
                pdfButton.IsEnabled = false;
                loadFileButton.IsEnabled = false;
                modulePanel.IsEnabled = true;
                startAnalyseButton.IsEnabled = false;
            }

        }

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

            try
            { 
                pdf = new IO.PDFGenerator(outputPdfPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to setup PDF");
            }
        }

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

        public void updateModuleProgress(AvailableOptions module, double progress)
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

        public void updateFileProgress(double progress)
        {
            analysisLabel.Content = analysisLabel.Content = "Loading file.. Please wait";
            try
            {
                progressBar.Value = progress;
            }
            catch (ArgumentException e)
            {
                progressBar.Value = 0;
            }

        }

        public void updateStatsProgress(double progress)
        {
            analysisLabel.Content = "Generating PDF report.. Please wait";
            try
            {
                progressBar.Value = progress;
            }
            catch (ArgumentException e)
            {
                progressBar.Value = 0;
            }

        }

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

        public void fileLoaded()
        {
            startAnalyseButton.IsEnabled = true;
            loadFileButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File loaded successfully.");
        }

        public void fileError()
        {
            startAnalyseButton.IsEnabled = false;
            loadFileButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File could not be loaded successfully.");

        }

        public void fileNotLoaded()
        {
            startAnalyseButton.IsEnabled = false;
            loadFileButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false;
            MessageBox.Show("File not found during analysis.");

        }

        public void statsCalculationStarted()
        {
            this.AnalysisInProgress = true;
            loadFileButton.IsEnabled = false;
            pdfButton.IsEnabled = false;
            modulePanel.IsEnabled = true;
            startAnalyseButton.IsEnabled = false;
            analysisLabel.Content = "Generating PDF report.. Please wait";
            buttonAbort.Content = "Abort generating";
            progressBar.Value = 0;
            panel.Visibility = Visibility.Visible;
            this.VisualisationPanelUserControl.Visibility = Visibility.Hidden;
            _abortMode = ABORT_MODE.ABORT_STATS_CALC;


            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<AvailableOptions> tempListCode = new System.Collections.Generic.List<AvailableOptions>();

            foreach (var option in modulePanel.getAllOptions())
            {

                if (option.Set)
                {
                    tempListCode.Add(option.Code);
                }
            }

            tempListCode.Sort();

            foreach (var element in tempListCode)
            {
                if (element != AvailableOptions.ECG_BASELINE &&
                   element != AvailableOptions.FLUTTER &&
                   element != AvailableOptions.HEART_AXIS &&
                   element != AvailableOptions.HEART_CLUSTER &&
                   element != AvailableOptions.HRV2 &&
                   element != AvailableOptions.HRV_DFA &&
                   element != AvailableOptions.SIG_EDR &&
                   element != AvailableOptions.ST_SEGMENT &&
                   element != AvailableOptions.SLEEP_APNEA)
                {
                    tempList.Add(element.ToString());
                }
            }

            try
            {
                IO.PDF.StoreDataPDF data = new IO.PDF.StoreDataPDF();
                data.AnalisysName = modulePanel.AnalysisName;
                System.Console.WriteLine("INIT" + data.AnalisysName);
                data.ModuleList = tempList;
                data.Filename = this.inputFilePath;

                pdf.GeneratePDF(data, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("One of modules does not work properly", "Failed to setup PDF", MessageBoxButton.OK);
            }
        }

        public void statsCalculationEnded(Dictionary<AvailableOptions, Dictionary<String, String>> results)
        {
            this.AnalysisInProgress = false;
            loadFileButton.IsEnabled = true;
            pdfButton.IsEnabled = true;
            modulePanel.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            progressBar.Value = 0;
            panel.Visibility = Visibility.Hidden;
            this.VisualisationPanelUserControl.Visibility = Visibility.Visible;

            try {
                Dictionary<AvailableOptions, Dictionary<String, String>> tempResults = new Dictionary<AvailableOptions, Dictionary<string, string>>();
                var keyList = results.Keys.ToList();
                keyList.Sort();

                foreach (var key in keyList)
                {
                    if (key != AvailableOptions.ECG_BASELINE &&
                       key != AvailableOptions.FLUTTER &&
                       key != AvailableOptions.HEART_AXIS &&
                       key != AvailableOptions.HEART_CLUSTER &&
                       key != AvailableOptions.HRV2 &&
                       key != AvailableOptions.SIG_EDR &&
                       key != AvailableOptions.HRV_DFA &&
                       key != AvailableOptions.ST_SEGMENT &&
                       key != AvailableOptions.SLEEP_APNEA)
                    {
                        tempResults.Add(key, results[key]);
                    }
                }
                results = tempResults;

                foreach (var result in results)
                {
                    Dictionary<String, String> temp = result.Value;
                    foreach (var smth in temp)
                    {
                        Console.WriteLine(smth.Key + " " + smth.Value);
                    }
                }

                IO.PDF.StoreDataPDF data = new IO.PDF.StoreDataPDF();

                foreach (var element in results)
                {
                    data.ModuleOption = element.Key;
                    data.AnalisysName = modulePanel.AnalysisName;
                    data.statsDictionary = element.Value;

                    pdf.GeneratePDF(data, false);
                }
                pdf.SaveDocument();


                MessageBox.Show("PDF generated");
                pdf.ProcessStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("One of modules does not work properly", "Failed to setup PDF", MessageBoxButton.OK);
            }

        }

        public void analysisAborted()
        {
            progressBar.Value = 0;
            MessageBox.Show("Analysis aborted.");
            loadFileButton.IsEnabled = true;
            modulePanel.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
        }

        public void loadingAborted()
        {
            this.VisualisationPanelUserControl.Visibility = Visibility.Hidden;
            panel.Visibility = Visibility.Hidden;
            loadFileButton.IsEnabled = true;
            MessageBox.Show("Loading file aborted.");
        }

        public void statsAborted()
        {
            this.VisualisationPanelUserControl.Visibility = Visibility.Visible;
            panel.Visibility = Visibility.Hidden;
            loadFileButton.IsEnabled = true;
            pdfButton.IsEnabled = true;
            modulePanel.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            progressBar.Value = 0;
            MessageBox.Show("PDF generating aborted.");
        }

        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="message"></param>
        private void analyzeEvent(IGUIMessage message)
        {
            message.Read(this);
        }


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
                    Communication.SendGUIMessage(new AbortStats());
                    break;
            }
            
        }
    }
}
