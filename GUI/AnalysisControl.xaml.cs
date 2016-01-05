using System;
using System.Windows;
using System.Windows.Controls;
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

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            _communication.SendGUIMessage(new Abort());
        }

        private void loadFileButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFileDialogBox loadFileDialogBox = new LoadFileDialogBox();
            loadFileDialogBox.ShowDialog();

        }
        
        

        private void startAnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var option in modulePanel.Options)
            {
                if (option.Set)
                {
                    if (option.ModuleParam == null) //tylko tymczasowo dopoki nie jest przez was zaimplementowane
                    {
                        //option.ModuleParam = new ModuleParams();
                        //moduleParams[option.Code] = option.ModuleParam;
                        Console.WriteLine(option.Name + " is set.");
                    }
                }
            }
            MessageBox.Show("Starting Analyses");
            //VisualisationPanelUserControl.DataContext = new VisualisationPanelControl();
            //VisualisationPanelUserControl.UpdateLayout();

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

    }
}
