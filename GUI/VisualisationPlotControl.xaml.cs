﻿using System;
using System.Collections.Generic;
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

        public VisualisationPlotControl()
        {
            InitializeComponent();
            ecgPlot = new ECGPlot("ECG_BASELINE");
            DataContext = ecgPlot;
            //ecgPlot.DisplayBasicData();
            ecgPlot.DisplayEcgBaseline();
        }

        public VisualisationPlotControl(string moduleName)
        {
            InitializeComponent();
            ecgPlot = new ECGPlot(moduleName);
            DataContext = ecgPlot;           
            ecgPlot.DisplayEcgBaseline();
        }

    
    }
}
