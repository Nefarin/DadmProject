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
        /// Stores signals
        /// </summary>
        List<Tuple<string, Vector<double>>> signals;
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
        /// <summary>
        /// Saves Basic Data in internal XML file
        /// </summary>
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

        /// <summary>
        /// Calls method loadMITBIHFile and sets Basic Data
        /// </summary>
        /// <param name="path">input file path</param>
        public void ConvertFile(string path)
        {
            loadMITBIHFile(path);
            Data = new Basic_Data();
            Data.Frequency = frequency;
            Data.Signals = signals;
            Data.SampleAmount = sampleAmount;
        }

        /// <summary>
        /// Loads MIT BIH input file and gets data from it
        /// </summary>
        /// <param name="path">input file path</param>
        public void loadMITBIHFile(string path)
        {
            string recordName = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            Wfdb.WfdbPath = directory;
            Record record = new Record(recordName);
            record.Open();

            frequency = (uint) record.SamplingFrequency;

            signals = new List<Tuple<string, Vector<double>>>();
            foreach (Signal signal in record.Signals)
            {
                sampleAmount = (uint) signal.NumberOfSamples;

                string lead = signal.Description;

                List<Sample> samples = signal.ReadAll();
                double[] convertedSamples = samples.Select(sample => sample.ToPhys()).ToArray();
                Vector<double> vector = Vector<double>.Build.Dense(convertedSamples.Length);
                vector.SetValues(convertedSamples);

                Tuple<string, Vector<double>> readSignal = Tuple.Create(lead, vector);
                signals.Add(readSignal);
            }

            record.Dispose();
        }
    }
}
