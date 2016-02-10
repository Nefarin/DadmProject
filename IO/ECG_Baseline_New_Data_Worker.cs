using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using System.Diagnostics;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads ECG_Baseline_Data from internal file
    /// </summary>
    #endregion
    public class ECG_Baseline_New_Data_Worker
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
        public ECG_Baseline_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public ECG_Baseline_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves part of filtered signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        #endregion
        public void SaveSignal(string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            if(mode == true)
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
        /// Loads filtered signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>signal</returns>
        #endregion
        public Vector<double> LoadSignal(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            FileStream stream = new FileStream(pathIn, FileMode.Open);
            stream.Seek(startIndex*sizeof(double), SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(stream);
            
            if (startIndex*sizeof(double) + length*sizeof(double) > br.BaseStream.Length)
            {
                throw new IndexOutOfRangeException();
            }

            double[] readSamples = new double[length];
            byte[] readSampless = new byte[length * sizeof(double)]; 
            br.Read(readSampless, 0, length * sizeof(double));

            unsafe
            {
                fixed (double* target = readSamples) {
                    fixed(byte* source = readSampless)
                    {
                        double* dbl = target;
                        double* src = (double*) source;
                        for (int i = 0; i < length; i++)
                        {
                            *dbl = *src;
                            dbl++;
                            src++;
                        }
                    }
                }
            }

            br.Close();
            stream.Close();

            Vector<double> vector = Vector<double>.Build.Dense(readSamples.Length);
            vector.SetValues(readSamples);
            return vector;
        }

        #region Documentation
        /// <summary>
        /// Gets number of filtered signal samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns> 
        #endregion
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string path = Path.Combine(directory, fileName);

            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(stream);

            uint count = (uint) br.BaseStream.Length / 8;

            br.Close();
            stream.Close();

            return count;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with ECG_Baseline_Data
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
