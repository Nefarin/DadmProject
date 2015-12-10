using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace EKG_Project.IO
{
    class WFDBInput
    {
        private const string _WFDB_Path = "../../DLL/WfdbCsharpLib.dll";
        private const string _record = "WfdbCsharpWrapper.Record";
        /*public static void Main()
        {
            Assembly wfdb = Assembly.LoadFrom(_WFDB_Path);
            Type[] types = wfdb.GetTypes();
            
            Type recordType = wfdb.GetType(_record);
            dynamic recordInstance = Activator.CreateInstance(recordType, "100"); //można wrzucić ścieżkę?
            Console.Write(recordInstance.Name);
            Console.Read();
        }*/
    }
}

