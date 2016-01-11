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
            }

        }
        
        

        private void startAnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var option in modulePanel.getAllOptions())
            {

                // Tuple<ModuleOption, ModuleParams> test = modulePanel.ModuleOptionAndParams(option.Code);
                if (option.Set)
                {

                    if (option.ModuleParam == null) //tylko tymczasowo dopoki nie jest przez was zaimplementowane
                    {

                        //option.ModuleParam = new ModuleParams();
                        //moduleParams(option.Code)= option.ModuleParam;
                        Console.WriteLine(option.Name + " is set.");
                    }
                }
            }
            MessageBox.Show("Starting Analyses");
            System.Collections.Generic.List<string> tempList = new System.Collections.Generic.List<string>();
            tempList.Add("ecgBaseline");
            tempList.Add("ecgBasic");
            tempList.Add("r_Peaks");
            VisualisationPanelUserControl.DataContext = new VisualisationPanelControl(tempList);
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
           startAnalyseButton.IsEnabled = File.Exists(inputFilePath) && Directory.Exists(Path.GetDirectoryName(outputPdfPath));
        }


    }
}
