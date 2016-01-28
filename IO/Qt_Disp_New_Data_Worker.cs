﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using System.Diagnostics;

namespace EKG_Project.IO
{
    public enum Qt_Disp_Attributes { QT_disp_local, QT_mean, QT_std };
    public enum Qt_Disp_Signal { T_End_Local, QT_Intervals};
    class Qt_Disp_New_Data_Worker
    {
        //FIELDS
        /// <summary>
        /// Stores txt files directory
        /// </summary>
        private string directory;

        /// <summary>
        /// Stores analysis name
        /// </summary>
        private string analysisName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Qt_Disp_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="analysisName">analysis name</param>
        public Qt_Disp_New_Data_Worker(String analysisName): this()
        {
            this.analysisName = analysisName;
        }

        /// <summary>
        /// Saves parts of T_End_Local signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        public void SaveTEndLocal(string lead, bool mode, List<int> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_T_End_Local" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }
                
            sw.Close();
        }

        /// <summary>
        /// Loads parts of T_End_Local signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns>
        public List<int> LoadTEndLocal(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_T_End_Local" + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);

            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr.EndOfStream)
            {
                string readLine = sr.ReadLine();
                iterator++;
            }

            iterator = 0;

            List<int> list = new List<int>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLine = sr.ReadLine();
                int readValue = Convert.ToInt32(readLine);
                list.Add(readValue);
                iterator++;
            }

            sr.Close();

            return list;
        }

        /// <summary>
        /// Saves parts of QT_Intervals signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        public void SaveQTIntervals(string lead, bool mode, List<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_QT_Intervals" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }
            sw.Close();
        }

        /// <summary>
        /// Loads parts of QT_Intervals signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>QT_Intervals list</returns>
        public List<double> LoadQTIntervals(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_QT_Intervals" + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);

            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr.EndOfStream)
            {
                string readLine = sr.ReadLine();
                iterator++;
            }

            iterator = 0;

            List<double> list = new List<double>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLine = sr.ReadLine();
                double readValue = Convert.ToDouble(readLine);
                list.Add(readValue);
                iterator++;
            }

            sr.Close();

            return list;
        }

        /// <summary>
        /// Gets number of Qt_Disp_Signal samples 
        /// </summary>
        /// <param name="atr">Qt_Disp_Signal</param>
        /// <param name="lead">lead</param>
        /// <returns>samples</returns>
        public uint getNumberOfSamples(Qt_Disp_Signal atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Saves Qt_Disp_Attributes in txt file
        /// </summary>
        /// <param name="atr">Qt_Disp_Attributes</param>
        /// <param name="value">value</param>
        public void SaveAttribute(Qt_Disp_Attributes atr, double value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        /// <summary>
        /// Loads Qt_Disp_Attributes from txt file
        /// </summary>
        /// <param name="atr">Qt_Disp_Attributes</param>
        /// <returns>value</returns>
        public double LoadAttribute(Qt_Disp_Attributes atr)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            double readValue = Convert.ToDouble(readLine);
            return readValue;
        }

        /// <summary>
        /// Deletes all analysis files with Qt_Disp_Data
        /// </summary>
        public void DeleteFiles()
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNamePattern = analysisName + "_" + moduleName + "*";
            string[] analysisFiles = Directory.GetFiles(directory, fileNamePattern);

            foreach (string file in analysisFiles)
            {
                File.Delete(file);
            }

        }
    }
}
