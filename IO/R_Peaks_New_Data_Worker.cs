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
    public enum R_Peaks_Attributes { RPeaks, RRInterval };
    public class R_Peaks_New_Data_Worker
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
        /// Stores R_Peaks_Attributes
        /// </summary>
        private R_Peaks_Attributes attributes;

        /// <summary>
        /// Gets or sets R_Peaks_Attributes
        /// </summary>
        public R_Peaks_Attributes Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public R_Peaks_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        public R_Peaks_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        /// <summary>
        /// Saves part of RPeaks/RRInterval signal in txt file
        /// </summary>
        /// <param name="atr">RPeaks/RRInterval</param>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        public void SaveSignal(R_Peaks_Attributes atr, string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);

            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }
            sw.Close();

        }

        /// <summary>
        /// Loads part of RPeaks/RRInterval signal from txt file
        /// </summary>
        /// <param name="atr">RPeaks/RRInterval</param>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>signal</returns>
        public Vector<double> LoadSignal(R_Peaks_Attributes atr, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

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
        /// Deletes all analysis files with Basic_Data
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
