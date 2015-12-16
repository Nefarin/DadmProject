using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WfdbCsharpWrapper;

using System.Reflection;


namespace EKG_Project.IO
{

    public interface IEleganckiTestDLL
    {
        //Tutaj definiujecie metody zgodne z DLL
        string Name { get; set; }
        
        
    }
    public class WFDBTest
    {
        private const string _WFDG_Path = "../../DLL/WfdbCsharpLib.dll";
        private const string _type_Name = "WfdbCsharpWrapper.Record";
        public static void Main(String[] args)
        {
            //Assembly wdfb = Assembly.LoadFrom(_WFDG_Path);
            //Type[] types = wdfb.GetTypes();
            //Type type = wdfb.GetType(_type_Name);

            Record record = new Record("100");
            Console.WriteLine(record.Name);
            Console.ReadLine();
           
        }
    }
}
