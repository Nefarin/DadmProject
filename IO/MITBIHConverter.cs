using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WfdbCsharpWrapper;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that converts MIT BIH files
    /// </summary>
    class MITBIHConverter : IECGConverter
    {
        //FIELDS
        /// <summary>
        /// Stores analysis name
        /// </summary>
        string analysisName;

        /// <summary>
        /// Stores sampling frequency
        /// </summary>
        uint frequency;

        /// <summary>
        /// Stores list of leads
        /// </summary>
        List<string> leads;

        /// <summary>
        /// Stores reference to loaded record
        /// </summary>
        Record record;

        Basic_Data _data;

        /// <summary>
        /// Gets or sets Basic Data
        /// </summary>
        public Basic_Data Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public MITBIHConverter(string MITBIHAnalysisName) 
        {
            analysisName = MITBIHAnalysisName;
        }

        //METHODS
        /*
        public void SaveResult()
        {
            foreach (var property in Data.GetType().GetProperties())
            {

                if (property.GetValue(Data, null) == null)
                {
                    //throw new Exception();

                }
                else
                {
                    Basic_Data_Worker dataWorker = new Basic_Data_Worker(analysisName);
                    dataWorker.Save(Data);
                }
            }
        }
         * */

        public void SaveResult()
        {
            Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
            dataWorker.SaveAttribute(Basic_Attributes.Frequency, frequency);
            dataWorker.SaveLeads(leads);
        }

        /// <summary>
        /// Calls method loadMITBIHFile, get lead names and frequency from file
        /// </summary>
        /// <param name="path">input file path</param>
        public void ConvertFile(string path)
        {
            loadMITBIHFile(path);
            getLeads();
            getFrequency();
        }

        /// <summary>
        /// Loads MIT BIH input file
        /// </summary>
        /// <param name="path">input file path</param>
        public void loadMITBIHFile(string path)
        {
            string recordName = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            Wfdb.WfdbPath = directory;
            record = new Record(recordName);
        }

        /// <summary>
        /// Gets part of signal from input file
        /// </summary>
        /// <param name="lead">lead name</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>vector of samples</returns>
        public Vector<Double> getSignal(string lead, int startIndex, int length)
        {
            Vector<Double> vector = null;
            if (startIndex < 0)
            {
                throw new Exception();
            }
            else
            {
                record.Open();
                foreach (Signal signal in record.Signals)
                {
                    if (signal.Description == lead)
                    {
                        double[] convertedSamples = new double[length];
                        signal.Seek(startIndex);
                        for (int i = 0; i < length; i++)
                        {
                            if (signal.IsEof)
                            {
                                throw new IndexOutOfRangeException();
                            }
                            else
                            {
                                Sample sample = signal.ReadNext();
                                convertedSamples[i] = sample.ToPhys();
                            }
                        }

                        try
                        {
                            // obsługa length == 0
                            vector = Vector<double>.Build.Dense(convertedSamples.Length);
                            vector.SetValues(convertedSamples);
                        }
                        catch (System.ArgumentOutOfRangeException e) 
                        {
                            Console.WriteLine(e);
                        }
                        
                    }
                }
                record.Dispose();
            }
            return vector;
        }

        /// <summary>
        /// Gets lead names from input file
        /// </summary>
        public List<string> getLeads()
        {
            record.Open();
            leads = new List<string>();
            foreach (Signal signal in record.Signals)
            {
                leads.Add(signal.Description);
            }
            record.Dispose();

            return leads;
        }

        /// <summary>
        /// Gets frequency from input file
        /// </summary>
        public uint getFrequency()
        {
            record.Open();
            frequency = (uint)record.SamplingFrequency;
            record.Dispose();

            return frequency;

        }

        public uint getNumberOfSamples(string lead)
        {
            uint numberOfSamples = 0;
            record.Open();
            foreach (Signal signal in record.Signals)
            {
                if (signal.Description == lead)
                {
                    numberOfSamples = (uint) signal.NumberOfSamples;
                }
            }
            record.Dispose();
            return numberOfSamples;
        }

        public static void Main()
        {
            IECGPath pathBuilder = new DebugECGPath();
            string directory = pathBuilder.getDataPath();

            MITBIHConverter mitbih = new MITBIHConverter("TestAnalysis");
            mitbih.loadMITBIHFile(System.IO.Path.Combine(directory, "100.dat"));
            mitbih.getLeads();
            Console.WriteLine(mitbih.leads[0]);
            Console.WriteLine(mitbih.leads[1]);
            mitbih.getFrequency();
            Console.WriteLine(mitbih.frequency);
            Vector<Double> v = mitbih.getSignal(mitbih.leads[0], 649990, 10);
            foreach (var val in v)
                Console.WriteLine(val);

            Basic_New_Data_Worker worker = new Basic_New_Data_Worker("TestAnalysis");
            worker.SaveSignal(mitbih.leads[0], false, v);
            Console.Read();
        }


    }
}
