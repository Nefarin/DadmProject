using System;
using System.Collections.Generic;
using System.IO;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that saves and loads ST_Segment_Data chunks from internal text file
    /// </summary>
    #endregion
    class ST_Segment_New_Data_Worker
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
        public ST_Segment_New_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        #endregion
        public ST_Segment_New_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        #region Documentation
        /// <summary>
        /// Saves tJs in txt file
        /// </summary>
        /// <param name="lead">Lead</param>
        /// <param name="mode">StreamWriter mode</param>
        /// <param name="tJsResults">List of tJs</param>
        #endregion
        public void Save_tJs(string lead, bool mode, List<long> tJsResults)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");

            // Zapisane do pierwszego pliku danych tJs
            string tJsFileName = analysisName + "_" + moduleName + "_" + lead + "_tJsResult" + ".txt";
            string tJsPathOut = Path.Combine(directory, tJsFileName);
            StreamWriter sw = new StreamWriter(tJsPathOut, mode);
            foreach (var result in tJsResults)
            {
                sw.WriteLine(result);
                sw.WriteLine("---");
            }
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Saves tSTs in txt file
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="mode"></param>
        /// <param name="tJsResults">List of tJs</param>
        #endregion
        public void Save_tSTs(string lead, bool mode, List<long> tSTsResults)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");

            // Zapisane do drugiego pliku danych tSTs
            string tSTsFileName = analysisName + "_" + moduleName + "_" + lead + "_tSTsResult" + ".txt";
            string tSTsPathOut = Path.Combine(directory, tSTsFileName);
            StreamWriter sw = new StreamWriter(tSTsPathOut, mode);
            foreach (var result in tSTsResults)
            {
                sw.WriteLine(result);
                sw.WriteLine("---");
            }
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads tJs from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>tJs result list</returns>
        #endregion
        public List<long> Load_tJs(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_tJsResult" + ".txt";
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
            List<long> list = new List<long>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item = sr.ReadLine();
                // Parsowanie typu string do long
                long sJsItem = long.Parse(item);
                list.Add(sJsItem);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();
            
            return list;
        }

        #region Documentation
        /// <summary>
        /// Loads tSTs from txt file
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>tSTs result list</returns>
        #endregion
        public List<long> Load_tSTs(string lead, int startIndex, int length)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_tSTsResult" + ".txt";
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
            List<long> list = new List<long>();
            while (iterator < length)
            {
                if (sr.EndOfStream)
                {
                    throw new IndexOutOfRangeException();
                }
                string item = sr.ReadLine();
                // Parsowanie typu string do long
                long sJsItem = long.Parse(item);
                list.Add(sJsItem);

                sr.ReadLine();
                iterator++;
            }
            sr.Close();

            return list;
        }

        #region Documentation
        /// <summary>
        /// Gets number of tJs samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint tJs_getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_tJsResult" + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 2;
            return count;
        }

        #region Documentation
        /// <summary>
        /// Gets number of tSTs samples
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint tSTs_getNumberOfSamples(string lead)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string fileName = analysisName + "_" + moduleName + "_" + lead + "_tSTsResult" + ".txt";
            string path = Path.Combine(directory, fileName);

            uint count = 0;
            using (StreamReader r = new StreamReader(path))
            {
                while (r.ReadLine() != null)
                {
                    count++;
                }
            }

            count = count / 2;
            return count;
        }

        #region Documentation
        /// <summary>
        /// Saves, specified in enum, attribute in txt file
        /// </summary>
        /// <param name="attributes">Attribute name enum</param>
        /// <param name="mode">StreamWriter mode</param>
        /// <param name="lead">Lead</param>
        /// <param name="attribute">Attribute value</param>
        #endregion
        public void SaveAttributes(ST_Segment_Attributes attribute, bool mode, string lead, int attributeValue)
        {
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName;

            switch (attribute)
            {
                case ST_Segment_Attributes.ConcaveCurves:
                    FileName = analysisName + "_" + moduleName + "_" + lead + ST_Segment_Attributes.ConcaveCurves.ToString() + ".txt";
                    break;
                case ST_Segment_Attributes.ConvexCurves:
                    FileName = analysisName + "_" + moduleName + "_" + lead + ST_Segment_Attributes.ConvexCurves.ToString() + ".txt";
                    break;
                case ST_Segment_Attributes.IncreasingLines:
                    FileName = analysisName + "_" + moduleName + "_" + lead + ST_Segment_Attributes.IncreasingLines.ToString() + ".txt";
                    break;
                case ST_Segment_Attributes.HorizontalLines:
                    FileName = analysisName + "_" + moduleName + "_" + lead + ST_Segment_Attributes.HorizontalLines.ToString() + ".txt";
                    break;
                case ST_Segment_Attributes.DecreasingLines:
                    FileName = analysisName + "_" + moduleName + "_" + lead + ST_Segment_Attributes.DecreasingLines.ToString() + ".txt";
                    break;
                default:
                    FileName = analysisName + "_" + moduleName + "_" + lead + "_unknown" + ".txt";
                    break;
            }

            string PathOut = Path.Combine(directory, FileName);
            StreamWriter sw = new StreamWriter(PathOut, mode);
            sw.WriteLine(attributeValue);
            sw.Close();
        }

        #region Documentation
        /// <summary>
        /// Loads, specified in enum, attribute from txt file
        /// </summary>
        /// <param name="attribute">Attribute name enum</param>
        /// <param name="lead">Lead</param>
        /// <returns>Attribute value</returns>
        #endregion
        public int LoadAttribute(ST_Segment_Attributes attribute, string lead)
        {
            int result = 0;
            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");
            string FileName = analysisName + "_" + moduleName + "_" + lead + attribute.ToString() + ".txt";
            string pathIn = Path.Combine(directory, FileName);

            StreamReader sr = new StreamReader(pathIn);
            string item = sr.ReadLine();
            result = int.Parse(item);
            sr.Close();

            return result;
        }
        
        #region Documentation
        /// <summary>
        /// Deletes all analysis files with ST_Segment_New_Data_Worker
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

    #region Documentation
    /// <summary>
    /// ST_Segment_Data attribute enum list
    /// </summary>
    #endregion
    public enum ST_Segment_Attributes
    {
        ConcaveCurves,
        ConvexCurves,
        IncreasingLines,
        HorizontalLines,
        DecreasingLines
    }
}
