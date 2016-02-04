using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using EKG_Project.Modules.HRT;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads HRT_Data chunks from internal text file
    /// </summary>
    #endregion
    class HRT_New_Data_Worker
    {
        //FIELDS
        #region Documentation
        /// <summary>
        /// Stores txt files directory
        /// </summary>
        #endregion
        private string directory;

        #region Documentation
        /// <summary>
        /// Stores analysis name
        /// </summary>
        #endregion
        private string analysisName;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public HRT_New_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public HRT_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves VPC in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="value">value</param>
        #endregion
        public void SaveVPC(string lead, VPC value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_VPC" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads VPC from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>VPCenum value</returns>
        #endregion
        public VPC LoadVPC(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_VPC" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            VPC readValue = (VPC)Enum.Parse(typeof(VPC), sr.ReadLine());
            sr.Close();

            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Saves X Axis tachogram in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveXAxisTachogramGUI(string lead, bool mode, int[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XAxis" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads X Axis tachogram from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public int[] LoadXAxisTachogramGUI(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XAxis" + ".txt";
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

            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves List with tachograms for each VPC detected in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveTachogramGUI(string lead, bool mode, List<List<double>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");

            int iterator = 0;
            foreach (var result in results)
            {
                string fileName = analysisName + "_" + moduleName + "_" + lead + "_Tachogram_" + iterator.ToString() + ".txt";
                string pathOut = Path.Combine(directory, fileName);
                StreamWriter sw = new StreamWriter(pathOut, mode);
                foreach (var sample in result)
                {
                    sw.WriteLine(sample.ToString());
                }
                sw.Close();
                iterator++;
            }
        }

        #region Documentation
        /// <summary>
        /// Loads List with tachograms for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public List<List<double>> LoadTachogramGUI(string lead, int[] startIndex, int[] length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNamePattern = analysisName + "_" + moduleName + "_" + lead + "_" + "_Tachogram_" + "*";
            string[] TachogramFiles = Directory.GetFiles(directory, fileNamePattern);

            int filesIndex = 0;
            List<List<double>> exList = new List<List<double>>();
            foreach (var file in TachogramFiles)
            {
                StreamReader sr = new StreamReader(file);
                //pomijane linie ...
                int iterator = 0;
                while (iterator < startIndex[filesIndex] && !sr.EndOfStream)
                {
                    string readLine = sr.ReadLine();
                    iterator++;
                }

                iterator = 0;
                List<double> inList = new List<double>();
                while (iterator < length[filesIndex])
                {
                    if (sr.EndOfStream)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    string readLine = sr.ReadLine();
                    double value = Convert.ToDouble(readLine);

                    inList.Add(value);
                    iterator++;
                }
                sr.Close();
                exList.Add(inList);
                filesIndex++;
            }

            return exList;
        }

        #region Documentation
        /// <summary>
        /// Saves List with average tachogram for each VPC detected  in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveMeanTachogramGUI(string lead, bool mode, double[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Mean" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads List with average tachogram for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public double[] LoadMeanTachogramGUI(string lead, int startIndex)//, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Mean" + ".txt";
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
            //while (iterator < length)
            while (!sr.EndOfStream)
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
            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves Point of x axis to plot mean turbulence onset in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveXPointsMeanOnsetGUI(string lead, bool mode, int[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XPointsMeanOnset" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Point of x axis to plot max turbulence slope from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>x points mean onset</returns> 
        #endregion
        public int[] LoadXPointsMeanOnsetGUI(string lead, int startIndex)//, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XPointsMeanOnset" + ".txt";
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
            //while (iterator < length)
            while (!sr.EndOfStream)
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

            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves List with values of average value of Turbulence Onset for each VPC detected in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveTurbulenceOnsetMeanGUI(string lead, bool mode, double[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceOnset" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads List with values of average value of Turbulence Onset for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public double[] LoadTurbulenceOnsetMeanGUI(string lead, int startIndex)//, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceOnset" + ".txt";
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
            //while (iterator < length)
            while (!sr.EndOfStream)
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
            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves Point of x axis to plot max turbulence slope in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveXPointsMaxSlopeGUI(string lead, bool mode, int[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XPointsMaxSlope" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Point of x axis to plot max turbulence slope from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public int[] LoadXPointsMaxSlopeGUI(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_XPointsMaxSlope" + ".txt";
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

            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves Listwith coordinates of largest slope (regression lines) for each VPC detected in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveTurbulenceSlopeMaxGUI(string lead, bool mode, double[] signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceSlopeMax" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Listwith coordinates of largest slope (regression lines) for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public double[] LoadTurbulenceSlopeMaxGUI(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceSlopeMax" + ".txt";
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
            return list.ToArray();
        }

        #region Documentation
        /// <summary>
        /// Saves List with values of Turbulence Onset for each VPC detected in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveTurbulenceOnsetPDF(string lead, bool mode, List<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceOnsetPDF" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads List with values of Turbulence Onset for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public List<double> LoadTurbulenceOnsetPDF(string lead, int startIndex)//, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceOnsetPDF" + ".txt";
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
            //while (iterator < length)
            while (!sr.EndOfStream)
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

        #region Documentation
        /// <summary>
        /// Saves List with values of Turbulence Slope for each VPC detected in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveTurbulenceSlopePDF(string lead, bool mode, List<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceSlopePDF" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads List with values of Turbulence Slope for each VPC detected from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>T_End_Local list</returns> 
        #endregion
        public List<double> LoadTurbulenceSlopePDF(string lead, int startIndex)//, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_TurbulenceSlopePDF" + ".txt";
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
            //while (iterator < length)
            while(!sr.EndOfStream)
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

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with HRT_New_Data_Worker
        /// </summary>
        #endregion
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

    //<-- Ten enum jest do usunięcia po zmergowaniu z modułem HRT
    #region Documentation
    /// <summary>
    /// HRT plot status enum 
    /// </summary>
    #endregion
    public enum VPC
    {
        NOT_DETECTED,
        NO_VENTRICULAR,
        DETECTED_BUT_IMPOSSIBLE_TO_PLOT,
        LETS_PLOT
    }
    //-->

}