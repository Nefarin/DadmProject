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
    class XMLConverter : IECGConverter
    {
        string analysisName;
        XmlNodeList sequences;
        uint sampleAmount;

        public XMLConverter(string XMLAnalysisName) 
        {
            analysisName = XMLAnalysisName;
        }

        public Basic_Data SaveResult()
        {
            Basic_Data data = new Basic_Data();
            return data;
        }

        public void ConvertFile(string path)
        {
            loadXMLFile(path);
            Basic_Data data = new Basic_Data();
            data.Frequency = getFrequency();
            data.Signals = getSignals();
            data.SampleAmount = sampleAmount;
            
            foreach(var property in data.GetType().GetProperties()) 
            {

                if (property.GetValue(data, null) == null)
                {
                    //throw new Exception(); // < - robić coś takiego?
                    //Console.WriteLine("Właściwość jest pusta");

                }
                //else
                    //Console.WriteLine("Właściwość jest wypełniona");
            }

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
                    double readedIncrement = Convert.ToDouble(incrementValue, new System.Globalization.NumberFormatInfo());
                    frequency = (uint) (1 / readedIncrement); //Hz

                    string incrementUnit = increment.Attributes["unit"].Value; //s
                }
            }
            return frequency;
        }

        double getOrigin()
        {
            double readedOrigin = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode value = sequence["value"];
                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    XmlNode origin = value["origin"];

                    string originValue = origin.Attributes["value"].Value;
                    readedOrigin = Convert.ToDouble(originValue);

                    string originUnit = origin.Attributes["unit"].Value; //zwykle uV

                }
            }
            return readedOrigin;
        }

        double getScale()
        {
            double readedScale = 0;
            foreach (XmlNode sequence in sequences)
            {
                XmlNode value = sequence["value"];
                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {

                    XmlNode scale = value["scale"];

                    string scaleValue = scale.Attributes["value"].Value;
                    readedScale = Convert.ToDouble(scaleValue);
                    string scaleUnit = scale.Attributes["unit"].Value; //uV
                }
            }
            return readedScale;
        }

        public List<Tuple<string, Vector<double>>> getSignals()
        {
            List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();

            foreach (XmlNode sequence in sequences)
            {
                XmlNode code = sequence["code"];
                string readedCode = null;

                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    readedCode = code.Attributes["code"].Value;
                    readedCode = readedCode.Replace("MDC_ECG_LEAD_", ""); //usunięcie z nazwy odprowadzenia dodatkowego kodu standardu HL7 aECG
                }

                XmlNode value = sequence["value"];
                Vector<double> readedDigits = null;

                if (value.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    string digits = value["digits"].InnerText;
                    readedDigits = stringToVector(digits);
                    readedDigits = normalizeSignal(readedDigits);
                    sampleAmount = getSampleAmount(readedDigits);
                }

                Tuple<string, Vector<double>> readedSignal = Tuple.Create(readedCode, readedDigits);
                Signals.Add(readedSignal);
            }
            return Signals;
        }

        Vector<double> stringToVector(string input)
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

        
        static void Main()
        {
            IECGPath pathBuilder = new DebugECGPath();
            Console.Out.WriteLine(pathBuilder.getDataPath());
            XMLConverter xml = new XMLConverter("Analysis1");

            //xml.ConvertFile(@"C:\temp\2.xml");
            /*
            xml.loadXMLFile(@"C:\temp\2.xml");
            uint f = xml.getFrequency();
            Console.WriteLine("Frequency: " + f + " Hz");

            List<Tuple<string, Vector<double>>> signals = xml.getSignals();
            foreach (var tuple in signals)
            {
                Console.WriteLine("Lead name: " + tuple.Item1);
                Console.WriteLine("Signal Vector in uV: " + tuple.Item2);
                uint samples = xml.sampleAmount;
                Console.WriteLine("Sample amount: " + samples.ToString());
                Console.WriteLine();

            }*/
            
            Console.Read();
        }
    }
}
