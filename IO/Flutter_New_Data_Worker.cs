using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EKG_Project.IO
{
    class Flutter_New_Data_Worker
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

        public Flutter_New_Data_Worker() 
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        public Flutter_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        /// <summary>
        /// Saves FlutterAnnotations in txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <param name="mode">true:append, false:overwrite file</param>
        /// <param name="result">flutter annotations</param>
        public void SaveSignal(string lead, bool mode, Tuple<int, int> result)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);

            sw.WriteLine(result.Item1);
            sw.WriteLine(result.Item2);

            sw.Close();
        }

        /// <summary>
        /// Loads FlutterAnnotations from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>flutter annotations tuple</returns>
        public Tuple<int, int> LoadSignal(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathIn = Path.Combine(directory, fileName);

            StreamReader sr = new StreamReader(pathIn);

            string item1 = sr.ReadLine();
            int convertedItem1 = Convert.ToInt32(item1);

            string item2 = sr.ReadLine();
            int convertedItem2 = Convert.ToInt32(item2);

            sr.Close();

            Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
            return tuple;
        }
    }
}
