using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    #region Documentation
    /// <summary>
    /// Class that converts XML files
    /// </summary> 
    #endregion
    public class XMLConverter : IECGConverter
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
        /// Stores list of sequence nodes
        /// </summary> 
        #endregion
        XmlNodeList sequences;

        #region Documentation
        /// <summary>
        /// Stores list of leads
        /// </summary> 
        #endregion
        List<string> leads;

        #region Documentation
        /// <summary>
        /// Default constructor
        /// </summary>
        #endregion
        public XMLConverter()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
        }

        #region Documentation
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="XMLAnalysisName">analysis name</param>
        #endregion
        public XMLConverter(string XMLAnalysisName) : this()
        {
            analysisName = XMLAnalysisName;
        }

        // METHODS
        #region Documentation
        /// <summary>
        /// Saves Basic Data in txt files
        /// </summary> 
        #endregion
        public void SaveResult()
        {
            Basic_New_Data_Worker dataWorker = new Basic_New_Data_Worker(analysisName);
            dataWorker.SaveAttribute(Basic_Attributes.Frequency, getFrequency());
            dataWorker.SaveLeads(leads);
        }

        #region Documentation
        /// <summary>
        /// Calls method loadXMLFile and sets Basic Data
        /// </summary>
        /// <param name="path">input file path</param> 
        #endregion
        public void ConvertFile(string path)
        {
            loadXMLFile(path);
            getLeads();
        }

        #region Documentation
        /// <summary>
        /// Loads XML input file and gets its list of sequence nodes
        /// </summary>
        /// <param name="path">input file path</param> 
        #endregion
        public void loadXMLFile(string path)
        {
            XmlDocument XMLFile = new XmlDocument();
            XMLFile.Load(path);

            XmlNamespaceManager manager = new XmlNamespaceManager(XMLFile.NameTable);
            manager.AddNamespace("hl7", "urn:hl7-org:v3");

            sequences = XMLFile.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence", manager);
            
        }

        #region Documentation
        /// <summary>
        /// Gets sampling frequency from input file
        /// </summary>
        /// <returns>sampling frequency</returns> 
        #endregion
        public uint getFrequency()
        {
            uint frequency = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode value = sequence["value"];
                if (value.Attributes["xsi:type"].Value == "GLIST_PQ")
                {
                    XmlNode increment = value["increment"];

                    string incrementValue = increment.Attributes["value"].Value;
                    double readIncrement = Convert.ToDouble(incrementValue, new System.Globalization.NumberFormatInfo());
                    frequency = (uint) (1 / readIncrement); //Hz

                    string incrementUnit = increment.Attributes["unit"].Value; //s
                }
            }
            return frequency;
        }

        #region Documentation
        /// <summary>
        /// Gets signal origin from input file
        /// </summary>
        /// <returns>signal origin</returns> 
        #endregion
        public double getOrigin()
        {
            double readOrigin = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode value = sequence["value"];
                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    XmlNode origin = value["origin"];

                    string originValue = origin.Attributes["value"].Value;
                    readOrigin = Convert.ToDouble(originValue, new System.Globalization.NumberFormatInfo());

                    string originUnit = origin.Attributes["unit"].Value; //zwykle uV

                }
            }
            return readOrigin;
        }

        #region Documentation
        /// <summary>
        /// Gets signal scale from input file
        /// </summary>
        /// <returns>signal scale</returns> 
        #endregion
        public double getScale()
        {
            double readScale = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode value = sequence["value"];
                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {

                    XmlNode scale = value["scale"];

                    string scaleValue = scale.Attributes["value"].Value;
                    readScale = Convert.ToDouble(scaleValue, new System.Globalization.NumberFormatInfo());
                    string scaleUnit = scale.Attributes["unit"].Value; //uV
                }
            }
            return readScale;
        }

        #region Documentation
        /// <summary>
        /// Converts string to array
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>output array</returns> 
        #endregion
        public double[] stringToArray(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => double.Parse(digit))
                              .ToArray();
            return digits;

        }

        #region Documentation
        /// <summary>
        /// Gets physical signal quantities basing on origin and scale
        /// </summary>
        /// <param name="signal">normalized signal</param>
        /// <returns>physical signal</returns> 
        #endregion
        double[] getPhysSignal(double[] signal)
        {
            double origin = getOrigin();
            double scale = getScale();
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] = signal[i] * scale + origin;
                signal[i] = signal[i] / 1000; //uV na mV
            }
            return signal;
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
        public Vector<double> getSignal(string lead, int startIndex, int length)
        {
            Vector<double> vector = null;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode code = sequence["code"];
                string readCode = null;

                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    readCode = code.Attributes["code"].Value;
                    readCode = readCode.Replace("MDC_ECG_LEAD_", "");

                    if(readCode == lead)
                    {
                        XmlNode value = code.NextSibling;
                        string digits = value["digits"].InnerText;
                        double[] readDigits = stringToArray(digits);
                        readDigits = getPhysSignal(readDigits);
                        int samples = readDigits.Length;

                        if((startIndex + length) <= samples)
                        {
                            vector =  Vector<double>.Build.Dense(length);
                            int iterator = 0;
                            for (int i = startIndex; i < startIndex + length; i++)
                            {
                                vector.At(iterator, readDigits[i]);
                                iterator++;
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException();
                        }
                    }
                }
                
            }
            return vector;
        }

        #region Documentation
        /// <summary>
        /// Gets lead names from input file
        /// </summary>
        /// <returns>lead names</returns> 
        #endregion
        public List<string> getLeads()
        {
            leads = new List<string>();
            foreach (XmlNode sequence in sequences)
            {
                XmlNode code = sequence["code"];
                string readCode = null;

                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    readCode = code.Attributes["code"].Value;
                    readCode = readCode.Replace("MDC_ECG_LEAD_", "");
                    if(readCode != null)
                    {
                        leads.Add(readCode);
                    }
                }
            }
            return leads;
        }

        #region Documentation
        /// <summary>
        /// Gets number of samples in signal
        /// </summary>
        /// <param name="lead">lead</param>
        /// <returns>number of samples</returns>
        #endregion
        public uint getNumberOfSamples(string lead)
        {
            uint numberOfSamples = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode code = sequence["code"];
                string readCode = null;

                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    readCode = code.Attributes["code"].Value;
                    readCode = readCode.Replace("MDC_ECG_LEAD_", "");

                    if (readCode == lead)
                    {
                        XmlNode value = code.NextSibling;
                        string digits = value["digits"].InnerText;
                        double[] readDigits = stringToArray(digits);
                        numberOfSamples = (uint) readDigits.Length;
                    }
                }

            }
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

        //To delete 
        #region Documentation
        /// <summary>
        /// Converts string to vector
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>output vector</returns> 
        #endregion
        public Vector<double> stringToVector(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => double.Parse(digit))
                              .ToArray();
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length);
            vector.SetValues(digits);
            return vector;

        }
    }
}
