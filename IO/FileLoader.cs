using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace EKG_Project.IO
{
    public class FileLoader
    {
        private IECGConverter converter;
        private string extension;
        private string analysisName;

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

            //converter.ConvertFile(path);
            //converter.SaveResult();


        }

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
                        throw new FileNotFoundException(); // < - robić coś takiego?
                    }
                     */
            }
            catch (Exception e)
            {
                throw e;
            }
                
        }

        
        static void Main()
        {
            FileLoader fl = new FileLoader();
            fl.Validate(@"C:\temp\234.txt");
            Console.WriteLine("Rozszerzenie: " + fl.extension);
            fl.Load(@"C:\temp\234.txt");
            Console.WriteLine("Konwerter: " + fl.converter.GetType().Name);
            Console.Read();
        }
    }
}
