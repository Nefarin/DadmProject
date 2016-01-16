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
    public class XMLConverter : IECGConverter
    {
        string analysisName;
        XmlNodeList sequences;
        uint sampleAmount;
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

        public XMLConverter(string XMLAnalysisName) 
        {
            analysisName = XMLAnalysisName;
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
            loadXMLFile(path);
            Data = new Basic_Data();
            Data.Frequency = getFrequency();
            Data.Signals = getSignals();
            Data.SampleAmount = sampleAmount;
        }
        
        public void loadXMLFile(string path)
        {
            XmlDocument XMLFile = new XmlDocument();
            XMLFile.Load(path);

            XmlNamespaceManager manager = new XmlNamespaceManager(XMLFile.NameTable);
            manager.AddNamespace("hl7", "urn:hl7-org:v3");

            sequences = XMLFile.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence", manager);
            
        }

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

        public uint getSampleAmount(Vector<double> signal)
        {
            if (signal != null)
                sampleAmount = (uint)signal.Count;
            return sampleAmount;
        }

        
        public static void Main()
        {
            IECGPath pathBuilder = new DebugECGPath();
            XMLConverter xml = new XMLConverter("TestAnalysis8");
            xml.ConvertFile(System.IO.Path.Combine(pathBuilder.getDataPath(), "8.xml"));
            xml.SaveResult();

            //xml.loadXMLFile(@"C:\temp\6.xml");

            uint f = xml.getFrequency();
            Console.WriteLine("Frequency: " + f + " Hz");

            uint samples = xml.sampleAmount;
            Console.WriteLine("Sample amount: " + samples.ToString());
            Console.WriteLine();

            List<Tuple<string, Vector<double>>> signals = xml.getSignals();
            foreach (var tuple in signals)
            {
                Console.WriteLine("Lead name: " + tuple.Item1);
                Console.WriteLine("Signal Vector in uV: " + tuple.Item2);
                Console.WriteLine();
           
            }

            Console.Read();
        }
    }
}
