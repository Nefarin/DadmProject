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

    class Basic_Test_Data_Worker
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
        /// Stores Basic_Attributes
        /// </summary>
        private Basic_Attributes attributes;

        /// <summary>
        /// Stores current number of samples
        /// </summary>
        private uint currentNumberOfSamples;

        /// <summary>
        /// Gets or sets Basic_Attributes
        /// </summary>
        public Basic_Attributes Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public Basic_Test_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        public Basic_Test_Data_Worker(String analysisName) : this()
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
            // true to append data to the file; false to overwrite the file.
            // If the file does not exist, this constructor creates a new file.
            if(mode == false)
            {
                currentNumberOfSamples = 0;
            }
            else
            {
                currentNumberOfSamples = LoadAttribute(Basic_Attributes.NumberOfSamples);
            }

            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
                currentNumberOfSamples++;
            }
            SaveAttribute(Basic_Attributes.NumberOfSamples, currentNumberOfSamples);
            sw.Close();

        }

        /// <summary>
        /// Loads signal from txt file
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
        
        public static void Main()
        {
            Basic_Test_Data_Worker worker = new Basic_Test_Data_Worker("TestAnalysis");
            Vector<double> vector = worker.LoadSignal("V5", 500000, 100);
            worker.SaveSignal("III", false, vector);
            Console.WriteLine("Current number of samples in file: " + worker.currentNumberOfSamples);
            worker.SaveAttribute(Basic_Attributes.Frequency, 360);
            uint frequency = worker.LoadAttribute(Basic_Attributes.Frequency);
            Console.WriteLine("Frequency: " + frequency);
            Console.Read();
        }
    }
}
