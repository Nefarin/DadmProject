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
        public FileLoader() { }

        void Load(string path)
        {
            string extension = Path.GetExtension(path);
            Console.WriteLine("GetExtension('{0}') returns '{1}'",
                path, extension);
        }

        /*
        static void Main()
        {
            FileLoader fl = new FileLoader();
            fl.Load(@"C:\temp\2.xml");
            Console.Read();
        }*/
    }
}
