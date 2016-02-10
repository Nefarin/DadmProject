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
    /// Stores Basic_Attributes
    /// </summary> 
    #endregion
    public enum Basic_Attributes { Frequency, NumberOfSamples };

    #region Documentation
    /// <summary>
    /// Class that saves and loads Basic Data from internal txt file
    /// </summary> 
    #endregion
    public class Basic_New_Data_Worker
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
        /// Stores number of leads
        /// </summary> 
        #endregion
        private uint numberOfLeads;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public Basic_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="analysisName">analysis name</param>
        #endregion
        public Basic_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves part of signal in txt file
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
            string pathOut = System.IO.Path.Combine(directory, fileName);

            if (mode == true)
            {
                FileStream stream = new FileStream(pathOut, FileMode.Append);
                BinaryWriter bw = new BinaryWriter(stream);

                try
                {
                    foreach (var sample in signal)
                    {
                        bw.Write(sample);
                    }
                }
                catch (System.NullReferenceException e)
                {}
                
                bw.Close();
                stream.Close();
            }
            else
            {
                FileStream stream = new FileStream(pathOut, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(stream);

                try
                {
                    foreach (var sample in signal)
                    {
                        bw.Write(sample);
                    }
                }
                catch (System.NullReferenceException e)
                { }

                bw.Close();
                stream.Close();
            }

        }

        #region Documentation
        /// <summary>
        /// Loads part of signal from txt file
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
            string pathIn = System.IO.Path.Combine(directory, fileName);

            FileStream stream = new FileStream(pathIn, FileMode.Open);
            stream.Seek(startIndex * sizeof(double), SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(stream);

            if (startIndex * sizeof(double) + length * sizeof(double) > br.BaseStream.Length)
            {
                throw new IndexOutOfRangeException();
            }

            
            double[] readSamples = new double[length];
            byte[] readSampless = new byte[length * sizeof(double)];
            br.Read(readSampless, 0, length * sizeof(double));

            unsafe
            {
                fixed (double* target = readSamples)
                {
                    fixed (byte* source = readSampless)
                    {
                        double* dbl = target;
                        double* src = (double*)source;
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
        /// Gets number of signal samples 
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

            uint count = (uint)br.BaseStream.Length / sizeof(double);

            br.Close();
            stream.Close();

            return count;
        }

        #region Documentation
        /// <summary>
        /// Saves Basic_Attributes
        /// </summary>
        /// <param name="atr">attribute</param>
        /// <param name="value">value</param> 
        #endregion
        public void SaveAttribute(Basic_Attributes atr, uint value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads Basic_Attributes
        /// </summary>
        /// <param name="atr">atribute</param>
        /// <returns>value</returns> 
        #endregion
        public uint LoadAttribute(Basic_Attributes atr)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + atr + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);
            string readLine = sr.ReadLine();
            sr.Close();

            uint readValue = Convert.ToUInt32(readLine);
            return readValue;
        }

        #region Documentation
        /// <summary>
        /// Saves lead names and gets its number
        /// </summary>
        /// <param name="leads">list of leads</param> 
        #endregion
        public void SaveLeads(List<string> leads)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_Leads" + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            foreach (var lead in leads)
            {
                sw.WriteLine(lead);
            }
            sw.Close();

            numberOfLeads = (uint) leads.Count();
        }

        #region Documentation
        /// <summary>
        /// Loads lead names and gets its number
        /// </summary>
        /// <returns>list of leads</returns> 
        #endregion
        public List<string> LoadLeads()
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_Leads" + ".txt";
            string pathIn = System.IO.Path.Combine(directory, fileName);

            List<string> leads = new List<string>();
            StreamReader sr = new StreamReader(pathIn);
            while(!sr.EndOfStream)
            {
                string readLine = sr.ReadLine();
                leads.Add(readLine);
            }
            sr.Close();

            numberOfLeads = (uint)leads.Count();

            return leads;
        }

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with Basic_Data
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
