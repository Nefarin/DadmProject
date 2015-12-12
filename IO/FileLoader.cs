using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class FileLoader : IECGConverter
    {
        public FileLoader() { }

        void Load(string path)
        {
            string extension = Path.GetExtension(path);
            Console.WriteLine("GetExtension('{0}') returns '{1}'",
                path, extension);
        }

        public Basic_Data SaveResult() 
        {
            Basic_Data data = new Basic_Data();
            return data;
        }

        public void ConvertFile(string path)
        {

        }


        
        static void Main()
        {
            FileLoader fl = new FileLoader();
            fl.Load(@"C:\temp\2.xml");
            Console.Read();
        }
    }
}
