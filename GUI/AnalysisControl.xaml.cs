using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using EKG_Project.Architecture;
using EKG_Project.Architecture.ProcessingStates;
using EKG_Project.Architecture.GUIMessages;
using EKG_Project.Modules;


namespace EKG_Project.GUI
{
    #region Documentation
    /// <summary>
    /// Interaction logic for AnalysisControl.xaml - class for GUI developers
    /// </summary>
    /// 
    #endregion


    public partial class AnalysisControl : UserControl
    {
        public string outputPdfPath;
        public string inputFilePath;

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
                if (option.Set)
                {
                    moduleParams[option.Code] = modulePanel.ModuleOptionAndParams(option.Code).Item2;
                    Console.WriteLine(moduleParams.Count);
                    Console.WriteLine(option.Code);
                    Console.WriteLine(moduleParams);
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
                // Nothing is checked - let user know somehow.
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
            Console.WriteLine("Analysis Started");
        }

        public void processingEnded()
        {
            Console.WriteLine("Analysis Ended");
            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            //tempList.Add("ecgBaseline");
            //tempList.Add("ecgBasic");
            //tempList.Add("r_Peaks");
            tempList.Add("waves");
            //tempList.Add("whole");
            VisualisationPanelUserControl.DataContext = new VisualisationPanelControl(tempList);
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

        #region Documentation
        /// <summary>
        /// analyzeEvent - do not delete - just develop - will be used by both GUI and Architects
        /// </summary>
        /// <param name="message"></param>
        ///
        #endregion
        private void analyzeEvent(IGUIMessage message)
        {
            message.Read(this);
        }

        private void checkPlayButton()
        {
           // this method does not make sense - analysis should be performed even if the user does not want to save pdf.
           //startAnalyseButton.IsEnabled = File.Exists(inputFilePath) && Directory.Exists(Path.GetDirectoryName(outputPdfPath));
        }


    }
}
