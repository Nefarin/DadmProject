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
    /// Handling buttons
    /// </summary>
    public partial class AnalysisControl : UserControl, INotifyPropertyChanged
    {
        private bool _analysisInProgress = false;

        public string outputPdfPath;
        public string inputFilePath;

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
            }
            else
            {
                MessageBox.Show("No analysis selected. Please select at least one analysis.", "Cannot start calculations", MessageBoxButton.OK);
            }


            //MessageBox.Show("Starting Analyses");
            //VisualisationPanelUserControl.DataContext = new VisualisationPanelControl();

        }

        IO.PDFGenerator pdf;

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

        public void processingStarted()
        {
            this.VisualisationPanelUserControl.Visibility = Visibility.Hidden;
            panel.Visibility = Visibility.Visible;
            this.AnalysisInProgress = true;
            loadFileButton.IsEnabled = false;
            pdfButton.IsEnabled = false;
            startAnalyseButton.IsEnabled = false;
        }

        public void processingEnded()
        {
            this.AnalysisInProgress = false;
            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            foreach (var option in modulePanel.getAllOptions())
            {
                
                if (option.Set)
                {
                    tempList.Add(option.Name);
                }
            }

            loadFileButton.IsEnabled = true;
            pdfButton.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            VisualisationPanelUserControl.DataContext = new VisualisationPanelControl(modulePanel.AnalysisName, tempList);
            panel.Visibility = Visibility.Hidden;
            this.VisualisationPanelUserControl.Visibility = Visibility.Visible;
        }

        public void updateProgress(AvailableOptions module, double progress)
        {
            analysisLabel.Content = "Analysis in progress..\nCurrent module: " + module.ToString();
            progressBar.Value = progress;
        }

        public void moduleEnded(AvailableOptions module, bool aborted)
        {
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
            MessageBox.Show("File loaded successfully.");
        }

        public void fileError()
        {
            startAnalyseButton.IsEnabled = false;
            MessageBox.Show("File could not be loaded successfully.");

        }

        public void fileNotLoaded()
        {
            startAnalyseButton.IsEnabled = false;
            MessageBox.Show("File not found during analysis.");

        }

        public void statsCalculationStarted()
        {
            MessageBox.Show("Started generating PDF");
        }

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

            pdf = new IO.PDFGenerator(outputPdfPath);
            MessageBox.Show("PDF generated");
        }

        public void analysisAborted()
        {
            progressBar.Value = 0;
            MessageBox.Show("Analysis aborted.");
            loadFileButton.IsEnabled = true;
            startAnalyseButton.IsEnabled = true;
            panel.Visibility = Visibility.Hidden;
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
            Communication.SendGUIMessage(new AbortAnalysis());
        }
    }
}
