using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace EKG_Project.IO
{
    class FileLoader
    {
        IECGConverter converter;
        string extension;
        string analysisName;

        public FileLoader() { }

        void Load(string path)
        {
            switch (extension)
            {
                case ".xml":
                    converter = new XMLConverter(analysisName);
                    break;
                case ".txt":
                    converter = new ASCIIConverter(analysisName);
                    break;
                case ".dat":
                    converter = new MITBIHConverter(analysisName);
                    break;
            }

            converter.ConvertFile(path);
            converter.SaveResult();


        }

        void Validate(string path)
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
