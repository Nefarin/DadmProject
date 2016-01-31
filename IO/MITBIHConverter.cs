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
    #region Documentation
    /// <summary>
    /// Class that converts MIT BIH files
    /// </summary> 
    #endregion
    class MITBIHConverter : IECGConverter
    {
        //FIELDS
        #region Documentation
        /// <summary>
        /// Stores txt files directory
        /// </summary> 
        #endregion
        private string directory;

        #region Documentation
        /// <summary>
        /// Stores analysis name
        /// </summary> 
        #endregion
        string analysisName;

        #region Documentation
        /// <summary>
        /// Stores sampling frequency
        /// </summary> 
        #endregion
        uint frequency;

        #region Documentation
        /// <summary>
        /// Stores list of leads
        /// </summary> 
        #endregion
        List<string> leads;

        #region Documentation
        /// <summary>
        /// Stores reference to loaded record
        /// </summary> 
        #endregion
        Record record;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public MITBIHConverter()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="MITBIHAnalysisName">analysis name</param>
        #endregion
        public MITBIHConverter(string MITBIHAnalysisName) : this()
        {
            analysisName = MITBIHAnalysisName;
        }

        //METHODS
        #region Documentation
        /// <summary>
        /// Saves freauency and lead names in txt files
        /// </summary> 
        #endregion
        public void SaveResult()
        {
            Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
            dataWorker.SaveAttribute(Basic_Attributes.Frequency, frequency);
            dataWorker.SaveLeads(leads);
        }

        #region Documentation
        /// <summary>
        /// Calls method loadMITBIHFile, get lead names and frequency from file
        /// </summary>
        /// <param name="path">input file path</param> 
        #endregion
        public void ConvertFile(string path)
        {
            loadMITBIHFile(path);
            getLeads();
            getFrequency();
        }

        #region Documentation
        /// <summary>
        /// Loads MIT BIH input file
        /// </summary>
        /// <param name="path">input file path</param> 
        #endregion
        public void loadMITBIHFile(string path)
        {
            string recordName = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            Wfdb.WfdbPath = directory;
            record = new Record(recordName);
        }

        #region Documentation
        /// <summary>
        /// Gets part of signal from input file
        /// </summary>
        /// <param name="lead">lead name</param>
        /// <param name="startIndex">start index</param>
        /// <param name="length">length</param>
        /// <returns>vector of samples</returns> 
        #endregion
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
                        {}
                        
                    }
                }
                record.Dispose();
            }
            return vector;
        }

        #region Documentation
        /// <summary>
        /// Gets lead names from input file
        /// </summary> 
        #endregion
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

        #region Documentation
        /// <summary>
        /// Gets frequency from input file
        /// </summary> 
        #endregion
        public uint getFrequency()
        {
            record.Open();
            frequency = (uint)record.SamplingFrequency;
            record.Dispose();

            return frequency;

        }

        #region Documentation
        /// <summary>
        /// Gets number of signal samples 
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
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

        #region Documentation
        /// <summary>
        /// Deletes all analysis files
        /// </summary> 
        #endregion
        public void DeleteFiles()
        {
            string fileNamePattern = analysisName + "*";
            string[] analysisFiles = Directory.GetFiles(directory, fileNamePattern);

            foreach (string file in analysisFiles)
            {
                File.Delete(file);
            }
        }
    }
}
