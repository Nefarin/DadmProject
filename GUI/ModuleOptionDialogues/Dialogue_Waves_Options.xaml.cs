﻿using EKG_Project.Modules.Waves;
using System;
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
using System.Windows.Shapes;

namespace EKG_Project.GUI.ModuleOptionDialogues
{
    /// <summary>
    /// Interaction logic for Dialogue_Waves_Options.xaml
    /// </summary>
    public partial class Dialogue_Waves_Options : Window
    {
        private Waves_Params returnParameters { get; set; }
        public Waves_Params PendingParameters { get; set; }

        public Dialogue_Waves_Options(Waves_Params parameters)
        {
            this.returnParameters = parameters;

            this.PendingParameters = new Waves_Params();
            this.PendingParameters.CopyParametersFrom(parameters);
            this.DataContext = this.PendingParameters;
            InitializeComponent();
        }

        private void ApplyParameterChanges(object sender, RoutedEventArgs e)
        {
            this.returnParameters.CopyParametersFrom(this.PendingParameters);
            this.Close();
        }

        private void RejectParameterChanges(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}