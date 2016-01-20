using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace EKG_Project.IO
{
    /// <summary>
    /// Class that sets converter for input file format
    /// </summary>
    public class FileLoader
    {
        //FIELDS
        /// <summary>
        /// Stores converter
        /// </summary>
        private IECGConverter converter;

        /// <summary>
        /// Stores file extension
        /// </summary>
        private string extension;

        /// <summary>
        /// Stores analysis name
        /// </summary>
        private string analysisName;

        /// <summary>
        /// Gets or sets converter
        /// </summary>
        public IECGConverter Converter
        {
            get
            {
                return converter;
            }

            set
            {
                converter = value;
            }
        }

        /// <summary>
        /// Gets or sets analysis name
        /// </summary>
        public string AnalysisName
        {
            get
            {
                return analysisName;
            }

            set
            {
                analysisName = value;
            }
        }

        public FileLoader() { }

        //METHODS
        /// <summary>
        /// Sets converter for input file format
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            switch (extension)
            {
                case ".xml":
                    converter = new XMLConverter(AnalysisName);
                    break;
                case ".txt":
                    converter = new ASCIIConverter(AnalysisName);
                    break;
                case ".dat":
                    converter = new MITBIHConverter(AnalysisName);
                    break;
            }
        }

        /// <summary>
        /// Validates input path
        /// </summary>
        /// <param name="path">input path</param>
        public void Validate(string path)
        {
            try
            {
                Directory.Exists(Path.GetDirectoryName(path));
                
                    if (File.Exists(path))
                    {
                        extension = Path.GetExtension(path);
                    }
                    /*
                    else
                    {
                        throw new FileNotFoundException(); 
                    }
                     */
            }
            catch (Exception e)
            {
                throw e;
            }
                
        }
    }
}
