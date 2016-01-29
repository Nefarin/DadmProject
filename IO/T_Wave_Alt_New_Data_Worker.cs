using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads T_Wave_Alt_Data in txt files
    /// </summary>
    #endregion
    public class T_Wave_Alt_New_Data_Worker
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
        public T_Wave_Alt_New_Data_Worker() 
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
        public T_Wave_Alt_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves AlternansDetectedList in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">AlternansDetectedList result</param>
        #endregion
        public void SaveAlternansDetectedList(string lead, bool mode, List<Tuple<int, int>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_AlternansDetectedList" + "_Item1" + ".txt";
            string pathOut1 = Path.Combine(directory, fileName1);

            StreamWriter sw1 = new StreamWriter(pathOut1, mode);
            foreach (var result in results)
            {
                sw1.WriteLine(result.Item1);
            }

            sw1.Close();

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_AlternansDetectedList" + "_Item2" + ".txt";
            string pathOut2 = Path.Combine(directory, fileName2);

            StreamWriter sw2 = new StreamWriter(pathOut2, mode);
            foreach (var result in results)
            {
                sw2.WriteLine(result.Item2);
            }

            sw2.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads AlternansDetectedList from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>AlternansDetectedList result list</returns>
        #endregion
        public List<Tuple<int, int>> LoadAlternansDetectedList(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName1 = analysisName + "_" + moduleName + "_" + lead + "_AlternansDetectedList" + "_Item1" + ".txt";
            string pathIn1 = Path.Combine(directory, fileName1);

            StreamReader sr1 = new StreamReader(pathIn1);

            string fileName2 = analysisName + "_" + moduleName + "_" + lead + "_AlternansDetectedList" + "_Item2" + ".txt";
            string pathIn2 = Path.Combine(directory, fileName2);

            StreamReader sr2 = new StreamReader(pathIn2);
            //pomijane linie ...
            int iterator = 0;
            while (iterator < startIndex && !sr1.EndOfStream)
            {
                sr1.ReadLine();
                sr2.ReadLine();
                iterator++;
            }

            iterator = 0;
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            while (iterator < length)
            {
                if (sr1.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr1.ReadLine();
                int convertedItem1 = Convert.ToInt32(item1);

                string item2 = sr2.ReadLine();
                int convertedItem2 = Convert.ToInt32(item2);


                Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
                list.Add(tuple);
                iterator++;
            }
            sr1.Close();
            sr2.Close();

            return list;
        }

        #region Documentation
        /// <summary>
        /// Gets number of AlternansDetectedList samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_AlternansDetectedList" + "_Item1" + ".txt";
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

        #region Documentation
        /// <summary>
        /// Deletes all analysis files with T_Wave_Alt_Data
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
