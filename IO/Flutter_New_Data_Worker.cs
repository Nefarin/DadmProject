using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EKG_Project.IO
{
    public class Flutter_New_Data_Worker
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
        public void SaveSignal(string lead, bool mode, List<Tuple<int, int>> results)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string pathOut = Path.Combine(directory, fileName);

            StreamWriter sw = new StreamWriter(pathOut, mode);

            foreach (var result in results)
            {
                sw.WriteLine(result.Item1);
                sw.WriteLine(result.Item2);
                sw.WriteLine("---");
            }

            sw.Close();
        }

        /// <summary>
        /// Loads FlutterAnnotations from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>flutter annotations list</returns>
        public List<Tuple<int, int>> LoadSignal(string lead, int startIndex, int length)
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
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item1 = sr.ReadLine();
                int convertedItem1 = Convert.ToInt32(item1);

                string item2 = sr.ReadLine();
                int convertedItem2 = Convert.ToInt32(item2);

                Tuple<int, int> tuple = Tuple.Create(convertedItem1, convertedItem2);
                list.Add(tuple);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();

            return list;
        }

        /// <summary>
        /// Gets number of FlutterAnnotations samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        public uint getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 3;
            return count;
        }

        /// <summary>
        /// Deletes all analysis files with Flutter_Data
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
