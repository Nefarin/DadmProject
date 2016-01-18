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
                checkPlayButton();
                Communication.SendGUIMessage(new LoadFile(inputFilePath));
            }

        }

        private void startAnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var option in modulePanel.getAllOptions())
            {
                //modulePa
                Console.WriteLine("#");
                Console.WriteLine(option.Name);
                Console.WriteLine("#");

                if (option.Set)
                {
                    moduleParams[option.Code] = modulePanel.ModuleOptionAndParams(option.Code).Item2;
                    Console.WriteLine(moduleParams.Count);
                    Console.WriteLine(option.Code);
                    Console.WriteLine(moduleParams);
                    Console.WriteLine(modulePanel.AnalysisName);

                    //Console.WriteLine(modulePanel.ModuleOptionAndParams(option.Code).Item2);
                    Console.WriteLine(option.Name + " is set.");
                }

            }

            if (moduleParams.Count > 0)
            {
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

        private void pdfButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Title = "Specify output pdf file";
            fileDialog.Filter = "pdf file|*.pdf";
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = outputPdfPath;

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPdfPath = fileDialog.FileName;
                checkPlayButton();
            }
        }

        public void processingStarted()
        {
            this.AnalysisInProgress = true;
            Console.WriteLine("Analysis Started");
        }

        public void processingEnded()
        {
            this.AnalysisInProgress = false;
            Console.WriteLine("Analysis Ended");
            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            foreach (var option in modulePanel.getAllOptions())
            {
                
                if (option.Set)
                {
                    tempList.Add(option.Name);
                }
            }
            //tempList.Add("ecgBaseline");
            //tempList.Add("ecgBasic");
            //tempList.Add("r_Peaks");
            //tempList.Add("waves");
            //tempList.Add("whole");
            //moduleParams.
            VisualisationPanelUserControl.DataContext = new VisualisationPanelControl(modulePanel.AnalysisName, tempList);
        }

        public void updateProgress(AvailableOptions module, double progress)
        {
            Console.WriteLine(module.ToString() + " progress: " + progress);
        }

        public void moduleEnded(AvailableOptions module, bool aborted)
        {
            if (aborted)
            {
                Console.WriteLine(module.ToString() + " aborted.");
            }
            else
            {
                isComputed[module] = true;
                Console.WriteLine(module.ToString() + " completed.");
            }
        }

        public void fileLoaded()
        {
            startAnalyseButton.IsEnabled = true;
            Console.WriteLine("File loaded sucessfully.");
        }

        public void fileError()
        {
            startAnalyseButton.IsEnabled = false;
            Console.WriteLine("File could not be loaded sucessfully.");
        }

        public void fileNotLoaded()
        {
            startAnalyseButton.IsEnabled = false;
            Console.WriteLine("File is not loaded.");
        }

        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="message"></param>
        private void analyzeEvent(IGUIMessage message)
        {
            message.Read(this);
        }

        private void checkPlayButton()
        {
            // this method does not make sense - analysis should be performed even if the user does not want to save pdf.
            //startAnalyseButton.IsEnabled = File.Exists(inputFilePath) && Directory.Exists(Path.GetDirectoryName(outputPdfPath));
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Analysis aborted.");
        }
    }
}
