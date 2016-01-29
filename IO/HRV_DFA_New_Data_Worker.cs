using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public enum HRV_DFA_Signals { DfaNumberN, DfaValueFn, ParamAlpha, Fluctuations };
    public class HRV_DFA_New_Data_Worker
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
        public HRV_DFA_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public HRV_DFA_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        /// <summary>
        /// Saves parts of HRV_DFA_Signals in txt file
        /// </summary>
        /// <param name="atr">HRV_DFA_Signals</param>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="results">results</param>
        public void SaveSignal(HRV_DFA_Signals atr, string lead, bool mode, Tuple<Vector<double>, Vector<double>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNameX = analysisName + "_" + moduleName + "_" + lead + "_" + atr + "_X" + ".txt";
            string pathOutX = Path.Combine(directory, fileNameX);

            StreamWriter sw = new StreamWriter(pathOutX, mode);
            foreach (var sample in results.Item1)
            {
                sw.WriteLine(sample.ToString());
            }
            sw.Close();

            string fileNameY = analysisName + "_" + moduleName + "_" + lead + "_" + atr + "_Y" + ".txt";
            string pathOutY = Path.Combine(directory, fileNameY);

            StreamWriter swY = new StreamWriter(pathOutY, mode);
            foreach (var sample in results.Item2)
            {
                swY.WriteLine(sample.ToString());
            }
            swY.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads part of AfDetection signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">vector start index</param>
        /// <param name="length">vector length</param>
        /// <returns>AfDetection tuple</returns>
        #endregion
        public Tuple<Vector<double>, Vector<double>> LoadSignal(HRV_DFA_Signals atr, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileNameX = analysisName + "_" + moduleName + "_" + lead + "_" + atr + "_X" + ".txt";
            string pathInX = Path.Combine(directory, fileNameX);

            StreamReader srX = new StreamReader(pathInX);

            string fileNameY = analysisName + "_" + moduleName + "_" + lead + "_" + atr + "_Y" + ".txt";
            string pathInY = Path.Combine(directory, fileNameY);

            StreamReader srY = new StreamReader(pathInY);

            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !srX.EndOfStream)
            {
                srX.ReadLine();
                srY.ReadLine();
                iterator++;
            }

            iterator = 0;
            double[] readSamplesX = new double[length];
            double[] readSamplesY = new double[length];
            while (iterator < length)
            {
                if (srX.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLineX = srX.ReadLine();
                readSamplesX[iterator] = Convert.ToDouble(readLineX);

                string readLineY = srY.ReadLine();
                readSamplesY[iterator] = Convert.ToDouble(readLineY);

                iterator++;
            }

            srX.Close();
            srY.Close();

            Vector<double> vectorX = Vector<double>.Build.Dense(readSamplesX.Length);
            vectorX.SetValues(readSamplesX);

            Vector<double> vectorY = Vector<double>.Build.Dense(readSamplesY.Length);
            vectorY.SetValues(readSamplesY);

            Tuple<Vector<double>, Vector<double>> tuple = Tuple.Create(vectorX, vectorY);

            return tuple;
        }

        /// <summary>
        /// Gets number of HRV_DFA_Signals vector samples 
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        public uint getNumberOfSamples(HRV_DFA_Signals atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + "_X" + ".txt";
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
        /// Deletes all analysis files with HRV_DFA_Data
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
