using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WfdbCsharpWrapper;


namespace EKG_Project.IO
{

    public interface IEleganckiTestDLL
    {
        //Tutaj definiujecie metody zgodne z DLL
        string Name { get; set; }
        
        
    }
    public class WFDBTest
    {
        public static void Main()
        {
            WfdbCsharpWrapper.
            IECGPath pathBuilder = new DebugECGPath();
            string datFileName = "100.dat";
            string heaFileName = "100.hea";
            string atrFileName = "100.atr";
            string recordName = "100";
            string directory = pathBuilder.getDataPath();

            Console.WriteLine(".dat file exists: " + File.Exists(System.IO.Path.Combine(directory, datFileName)));
            Console.WriteLine(".hea file exists: " + File.Exists(System.IO.Path.Combine(directory, heaFileName)));
            Console.WriteLine(".atr file exists: " + File.Exists(System.IO.Path.Combine(directory, atrFileName)));

            Record record = new Record(System.IO.Path.Combine(directory, recordName));
            record.Open();

            Console.WriteLine("Record Name : " + record.Name);
            Console.WriteLine("Record Info : " + record.Info);
            Console.WriteLine("Record's Sampling Frequency : " + record.SamplingFrequency);

            Console.WriteLine("Available signals.");

            foreach (Signal signal in record.Signals)
            {

                Console.WriteLine("=====================================");
                Console.WriteLine("Signal's Name : " + signal.FileName);
                Console.WriteLine("Signal's Description : " + signal.Description);
                Console.WriteLine("Signal's Number of samples : " + signal.NumberOfSamples);
                Console.WriteLine("Signal's First Sample : " + signal.InitValue);

                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Showing the first 10 samples of the signal");
                Console.WriteLine("------------------------------------------");

                List<Sample> samples = signal.ReadNext(10);

                for (int i = 0; i < samples.Count; i++)
                {
                    Console.WriteLine("Sample " + i + " Value (adu) = " + samples[i].Adu);
                    Console.WriteLine("             Value (microvolt) = " + samples[i].ToMicrovolts());
                    Console.WriteLine("             Value (millivolt) = " + samples[i].ToPhys());
                }

                Console.WriteLine("--------------------------------------");

                Console.WriteLine("=====================================");
            }


            record.Dispose();

            Console.ReadLine();
           
        }
    }
}
