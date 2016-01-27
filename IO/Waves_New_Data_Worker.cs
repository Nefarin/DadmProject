﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    public enum Waves_Attributes { QRSOnsets, QRSEnds, POnsets, PEnds, TEnds };

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

        /// <summary>
        /// Stores Waves_Attributes 
        /// </summary>
        private Waves_Attributes attributes;

        /// <summary>
        /// Gets or sets Waves_Attributes
        /// </summary>
        public Waves_Attributes Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

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
        /// Saves part of Waves signal in txt file
        /// </summary>
        /// <param name="atr">Waves_Attributes</param>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="signal">signal</param>
        #endregion
        public void SaveSignal(Waves_Attributes atr, string lead, bool mode, List<int> signal)
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

        #region Documentation
        /// <summary>
        /// Loads parts of Waves signal from txt file
        /// </summary>
        /// <param name="atr">Waves_Attributes</param>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>Waves signal list</returns>
        #endregion
        public List<int> LoadSignal(Waves_Attributes atr, string lead, int startIndex, int length)
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

            List<int> list = new List<int>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }

                string readLine = sr.ReadLine();
                int readValue = Convert.ToInt32(readLine);
                list.Add(readValue);
                iterator++;
            }

            sr.Close();

            return list;
        }
    }
}
