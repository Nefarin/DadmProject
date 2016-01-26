using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Waves;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads Waves_Data chunks from internal text file
    /// </summary>
    #endregion
    class Waves_New_Data_Worker
    {
        //FIELDS
        #region Documentation
        /// <summary>
        /// Stores internal XML file directory
        /// </summary>
        #endregion
        string directory;

        #region Documentation
        /// <summary>
        /// Stores analysis name
        /// </summary>
        #endregion
        string analysisName;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public Waves_New_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="analysisName"></param>
        #endregion
        public Waves_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        //METHODS
        #region Documentation
        /// <summary>
        /// Saves part of filtered signal in txt file
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="mode"></param>
        /// <param name="signal"></param>
        #endregion
        public void SaveSignal(string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
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
        /// Loads signal chunk from txt file
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        #endregion
        public Vector<double> LoadSignal(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
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
    }
}
