﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Solvers;

namespace EKG_Project.Modules.Heart_Class
{
    public class Heart_Class_Params : ModuleParams
    {
        public Heart_Class_Params()
        {
            //WCZYTANIE ZBIORU TRENINGOWEGO19
            //List<Vector<double>> trainDataList = loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d.txt");

            //WCZYTANIE ETYKIET ZBIORU TRENINGOWEGO: 0-V, 1-NV
            //List<Vector<double>> trainClassList = loadFile(@"C:\Users\Kamillo\Desktop\Kasia\DADM proj\train_d_label.txt");

            AnalysisName = "Analysis6";
        }

        public Heart_Class_Params(string analysisName)
        {
            AnalysisName = analysisName;
        }

        public string AnalysisName { get; set; }

        //potencjalna mozliwosc zmiany danych treningowych

        public List<Vector<double>> TrainSamples { get; set; }
        public List<int> TrainClasses { get; set; }
    }
}
