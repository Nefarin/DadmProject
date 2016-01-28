using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that converts XML files
    /// </summary>
    public class XMLConverter : IECGConverter
    {
        //FIELDS
        /// <summary>
        /// Stores analysis name
        /// </summary>
        string analysisName;

        /// <summary>
        /// Stores list of sequence nodes
        /// </summary>
        XmlNodeList sequences;

        /// <summary>
        /// Stores number of samples in signal
        /// </summary>
        uint sampleAmount;

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

        public XMLConverter(string XMLAnalysisName) 
        {
            analysisName = XMLAnalysisName;
        }

        // METHODS
        /// <summary>
        /// Saves Basic Data in internal XML file
        /// </summary>
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

        /// <summary>
        /// Calls method loadXMLFile and sets Basic Data
        /// </summary>
        /// <param name="path">input file path</param>
        public void ConvertFile(string path)
        {
            loadXMLFile(path);
            Data = new Basic_Data();
            Data.Frequency = getFrequency();
            Data.Signals = getSignals();
            Data.SampleAmount = sampleAmount;
        }
        
        /// <summary>
        /// Loads XML input file and gets its list of sequence nodes
        /// </summary>
        /// <param name="path">input file path</param>
        public void loadXMLFile(string path)
        {
            XmlDocument XMLFile = new XmlDocument();
            XMLFile.Load(path);

            XmlNamespaceManager manager = new XmlNamespaceManager(XMLFile.NameTable);
            manager.AddNamespace("hl7", "urn:hl7-org:v3");

            sequences = XMLFile.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence", manager);
            
        }

        /// <summary>
        /// Gets sampling frequency from input file
        /// </summary>
        /// <returns>sampling frequency</returns>
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

        /// <summary>
        /// Gets signal origin from input file
        /// </summary>
        /// <returns>signal origin</returns>
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

        /// <summary>
        /// Gets signal scale from input file
        /// </summary>
        /// <returns>signal scale</returns>
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

        /// <summary>
        /// Gets signals from input file
        /// </summary>
        /// <returns>signals</returns>
        public List<Tuple<string, Vector<double>>> getSignals()
        {
            List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();

            foreach (XmlNode sequence in sequences)
            {
                XmlNode code = sequence["code"];
                string readCode = null;

                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    readCode = code.Attributes["code"].Value;
                    readCode = readCode.Replace("MDC_ECG_LEAD_", ""); //usunięcie z nazwy odprowadzenia dodatkowego kodu standardu HL7 aECG
                }

                XmlNode value = sequence["value"];
                Vector<double> readDigits = null;

                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    string digits = value["digits"].InnerText;
                    readDigits = stringToVector(digits);
                    readDigits = normalizeSignal(readDigits);
                    getSampleAmount(readDigits);
                }

                if (readCode != null && readDigits != null)
                {
                    Tuple<string, Vector<double>> readSignal = Tuple.Create(readCode, readDigits);
                    Signals.Add(readSignal);
                }
            }
            return Signals;
        }

        /// <summary>
        /// Converts string to vector
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>output vector</returns>
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

        /// <summary>
        /// Normalizes signal basing on origin and scale
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        Vector<double> normalizeSignal(Vector<double> signal)
        {
            double origin = getOrigin();
            double scale = getScale();
            for (int i = 0; i < signal.Count; i++)
            {
                signal[i] = (signal[i] - origin) / scale;
                //signal[i] = signal[i] / 1000; //uV na mV?
            }
            return signal;
        }

        /// <summary>
        /// Gets number of samples in signal
        /// </summary>
        /// <param name="signal">signal</param>
        /// <returns>number of samples</returns>
        public uint getSampleAmount(Vector<double> signal)
        {
            if (signal != null)
                sampleAmount = (uint)signal.Count;
            return sampleAmount;
        }

        public Vector<double> getSignal(string lead, int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public string[] getLeads()
        {
            throw new NotImplementedException();
        }

        public uint getNumberOfSamples(string lead)
        {
            throw new NotImplementedException();
        }
    }
}
