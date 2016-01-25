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
        /// Stores number of samples
        /// </summary>
        uint sampleAmount;

        /// <summary>
        /// Stores array of leads
        /// </summary>
        string[] leads;

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
            Basic_Test_Data_Worker dataWorker = new Basic_Test_Data_Worker(analysisName);
            dataWorker.SaveAttribute(Attributes.Frequency, frequency);
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
            record.Open();
            Vector<Double> vector = null;
            foreach (Signal signal in record.Signals)
            {
                if (signal.Description == lead)
                {
                    signal.Seek(startIndex);
                    List<Sample> samples = signal.ReadNext(length);

                    if (signal.IsEof)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    double[] convertedSamples = samples.Select(sample => sample.ToPhys()).ToArray();
                    vector = Vector<double>.Build.Dense(convertedSamples.Length);
                    vector.SetValues(convertedSamples);
                }
            }
            record.Dispose();
            return vector;
        }

        /// <summary>
        /// Gets lead names from input file
        /// </summary>
        public void getLeads()
        {
            record.Open();
            leads = new string[Signal.GetSignalsCount(record)];
            int i = 0;
            foreach (Signal signal in record.Signals)
            {
                leads[i] = signal.Description;
                i++;
            }
            record.Dispose();
        }

        /// <summary>
        /// Gets frequency from input file
        /// </summary>
        public void getFrequency()
        {
            record.Open();
            frequency = (uint)record.SamplingFrequency;
            record.Dispose();

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
            Vector<Double> v = mitbih.getSignal(mitbih.leads[0], 649990, 9);
            foreach (var val in v)
                Console.WriteLine(val);

            Basic_Test_Data_Worker worker = new Basic_Test_Data_Worker("TestAnalysis");
            worker.SaveSignal(mitbih.leads[0], false, v);
            Console.Read();
        }
    }
}
