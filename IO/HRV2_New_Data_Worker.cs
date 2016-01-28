using System;
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
    public enum HRV2_Attributes { Tinn, TriangleIndex, SD1, SD2, ElipseCenter };
    public enum HRV2_Signal { PoincarePlotData_x, PoincarePlotData_y };

    class HRV2_New_Data_Worker
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

        public HRV2_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        public HRV2_New_Data_Worker(String analysisName)  : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves part of HRV2_Signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">HRV2_Signal</param>
        #endregion
        public void SaveSignal(HRV2_Signal atr, string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }
            sw.Close();
        }

        /// <summary>
        /// Loads parts of HRV2_Signal from txt file
        /// </summary>
        /// <param name="atr">HRV2_Signal</param>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>HRV2_Signal</returns>
        public Vector<double> LoadSignal(HRV2_Signal atr, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
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
            double[] readSamples = new double[length];
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLine = sr.ReadLine();
                readSamples[iterator] = Convert.ToDouble(readLine);
                iterator++;
            }

            sr.Close();

            Vector<double> vector = Vector<double>.Build.Dense(readSamples.Length);
            vector.SetValues(readSamples);
            return vector;
        }

        /// <summary>
        /// Gets number of HRV2_Signal samples 
        /// </summary>
        /// <param name="atr">HRV2_Signal</param>
        /// <param name="lead">lead</param>
        /// <returns>samples</returns>
        public uint getNumberOfSamples(HRV2_Signal atr, string lead)
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

        #region Documentation
        /// <summary>
        /// Saves Histogram in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">histogram data</param>
        #endregion
        public void SaveHistogram(string lead, bool mode, List<Tuple<double, double>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Histogram" + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);
            foreach (var result in results)
            {
                sw.WriteLine(result.Item1);
                sw.WriteLine(result.Item2);
                sw.WriteLine("---");
            }

            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Histogram from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>histogram data list</returns>
        #endregion
        public List<Tuple<double, double>> LoadHistogram(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Histogram" + ".txt";
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
            List<Tuple<double, double>> list = new List<Tuple<double, double>>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr.ReadLine();
                double convertedItem1 = Convert.ToDouble(item1);

                string item2 = sr.ReadLine();
                double convertedItem2 = Convert.ToDouble(item2);

                Tuple<double, double> tuple = Tuple.Create(convertedItem1, convertedItem2);
                list.Add(tuple);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();


            return list;
        }

        /// <summary>
        /// Loads Histogram number of samples
        /// </summary>
        /// <param name="lead"></param>
        /// <returns></returns>
        public uint getHistogramNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_Histogram" + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 3;
            return count;
        }

        /// <summary>
        /// Saves HRV2_Attributes
        /// </summary>
        /// <param name="atr">attribute</param>
        /// <param name="value">value</param>
        public void SaveAttribute(HRV2_Attributes atr, string lead, double value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        /// <summary>
        /// Loads HRV2_Attributes
        /// </summary>
        /// <param name="atr">atribute</param>
        /// <returns>value</returns>
        public double LoadAttribute(HRV2_Attributes atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            double readValue = Convert.ToDouble(readLine);
            return readValue;
        }

        /// <summary>
        /// Deletes all analysis files with HRV2_Data
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
