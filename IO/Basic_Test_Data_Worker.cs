using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using System.Diagnostics;

namespace EKG_Project.IO
{
    public enum Attributes { Frequency, NumberOfSamples };

    class Basic_Test_Data_Worker
    {
        //FIELDS
        /// <summary>
        /// Stores internal XML file directory
        /// </summary>
        private string directory;

        /// <summary>
        /// Stores analysis name
        /// </summary>
        private string analysisName;

        private Attributes attributes;

        public Attributes Attributes
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

        public void SaveSignal(string lead, bool mode, Vector<double> signal)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode); 
            // true to append data to the file; false to overwrite the file.
            // If the file does not exist, this constructor creates a new file.
            foreach (var sample in signal)
            {
                sw.WriteLine(sample.ToString());
            }

            sw.Close();

        }
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
            while (iterator < length && !sr.EndOfStream)
            {
                string readLine = sr.ReadLine();
                readSamples[iterator] = Convert.ToDouble(readLine);
                iterator++;
            }

            sr.Close();

            Vector<double> vector = Vector<double>.Build.Dense(readSamples.Length);
            vector.SetValues(readSamples);
            return vector;
        }

        public void SaveAttribute(Attributes atr, uint value)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + atr + ".txt";
            string pathOut = System.IO.Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut);
            sw.WriteLine(value);
            sw.Close();
        }

        public uint LoadAttribute(Attributes atr)
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
            worker.SaveAttribute(Attributes.Frequency, 360);
            worker.SaveAttribute(Attributes.NumberOfSamples, 100);
            uint frequency = worker.LoadAttribute(Attributes.Frequency);
            Console.WriteLine(frequency);
            Console.Read();
        }
    }
}
