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
    class MITBIHConverter : IECGConverter
    {
        string analysisName;
        uint frequency;
        uint sampleAmount;
        List<Tuple<string, Vector<double>>> signals;
        Basic_Data _data;

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

        public void SaveResult()
        {
            foreach (var property in Data.GetType().GetProperties())
            {

                if (property.GetValue(Data, null) == null)
                {
                    //throw new Exception(); // < - robić coś takiego?

                }
                else
                {
                    Basic_Data_Worker dataWorker = new Basic_Data_Worker(analysisName);
                    dataWorker.Save(Data);
                }
            }
        }

        public void ConvertFile(string path)
        {
            loadMITBIHFile(path);
            Data = new Basic_Data();
            Data.Frequency = frequency;
            Data.Signals = signals;
            Data.SampleAmount = sampleAmount;
        }

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
