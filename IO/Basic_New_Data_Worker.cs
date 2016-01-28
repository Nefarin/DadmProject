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
    public enum Basic_Attributes { Frequency, NumberOfSamples };

    public class Basic_New_Data_Worker
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
        /// Stores current number of samples
        /// </summary>
        private uint currentNumberOfSamples;

        private uint numberOfLeads;

        public Basic_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        public Basic_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        /// <summary>
        /// Saves part of signal in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        public void SaveSignal(string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode); 
            if(mode == false)
            {
                currentNumberOfSamples = 0;
            }
            else
            {
                currentNumberOfSamples = LoadAttribute(Basic_Attributes.NumberOfSamples);
            }

            try
            {
                foreach (var sample in signal)
                {
                    sw.WriteLine(sample.ToString());
                    currentNumberOfSamples++;
                }
            }catch(System.NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            
            SaveAttribute(Basic_Attributes.NumberOfSamples, currentNumberOfSamples);
            sw.Close();

        }

        /// <summary>
        /// Loads part of signal from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>signal</returns>
        public Vector<double> LoadSignal(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
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
        /// Saves Basic_Attributes
        /// </summary>
        /// <param name="atr">attribute</param>
        /// <param name="value">value</param>
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

        /// <summary>
        /// Loads Basic_Attributes
        /// </summary>
        /// <param name="atr">atribute</param>
        /// <returns>value</returns>
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

        /// <summary>
        /// Saves lead names and gets its number
        /// </summary>
        /// <param name="leads">list of leads</param>
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

        /// <summary>
        /// Loads lead names and gets its number
        /// </summary>
        /// <returns>list of leads</returns>
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
