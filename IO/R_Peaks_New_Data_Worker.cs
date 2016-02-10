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
    #region Documentation
    /// <summary>
    /// Stores R_Peaks_Attributes
    /// </summary>
    #endregion
    public enum R_Peaks_Attributes { RPeaks, RRInterval };
    #region Documentation
    /// <summary>
    /// Class that saves and loads R_Peaks_Data
    /// </summary>
    #endregion
    public class R_Peaks_New_Data_Worker
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
        public R_Peaks_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="analysisName">analysis name</param>
        #endregion
        public R_Peaks_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves part of RPeaks/RRInterval signal in txt file
        /// </summary>
        /// <param name="atr">RPeaks/RRInterval</param>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param> 
        #endregion
        public void SaveSignal(R_Peaks_Attributes atr, string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            if (mode == true)
            {
                FileStream stream = new FileStream(pathOut, FileMode.Append);
                BinaryWriter bw = new BinaryWriter(stream);
                foreach (var sample in signal)
                {
                    bw.Write(sample);
                }
                bw.Close();
                stream.Close();
            }
            else
            {
                FileStream stream = new FileStream(pathOut, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(stream);
                foreach (var sample in signal)
                {
                    bw.Write(sample);
                }
                bw.Close();
                stream.Close();
            }

        }

        #region Documentation
        /// <summary>
        /// Loads part of RPeaks/RRInterval signal from txt file
        /// </summary>
        /// <param name="atr">RPeaks/RRInterval</param>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>signal</returns> 
        #endregion
        public Vector<double> LoadSignal(R_Peaks_Attributes atr, string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            FileStream stream = new FileStream(pathIn, FileMode.Open);
            stream.Seek(startIndex * 8, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(stream);

            if (startIndex * 8 + length * 8 > br.BaseStream.Length)
            {
                throw new IndexOutOfRangeException();
            }

            int iterator = 0;
            double[] readSamples = new double[length];
            while (iterator < length)
            {
                readSamples[iterator] = br.ReadDouble();
                iterator++;
            }

            br.Close();
            stream.Close();

            Vector<double> vector = Vector<double>.Build.Dense(readSamples.Length);
            vector.SetValues(readSamples);
            return vector;
        }

        #region Documentation
        /// <summary>
        /// Gets number of RPeaks/RRInterval vector samples 
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns> 
        #endregion
        public uint getNumberOfSamples(R_Peaks_Attributes atr, string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_" + atr + ".txt";
            string path = Path.Combine(directory, fileName);

            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(stream);

            uint count = (uint)br.BaseStream.Length / 8;

            br.Close();
            stream.Close();

            return count;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with R_Peaks Data
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
}
