using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    class XMLConverter : FileLoader
    {
        string pathIn;
        XmlNodeList sequences;

        public XMLConverter(string XMLPath) 
        {
            pathIn = XMLPath;
        }
        
        public void loadXMLFile(string pathIn)
        {
            XmlDocument XMLFile = new XmlDocument();
            XMLFile.Load(pathIn);

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

        public List<Tuple<string, Vector<double>>> getSignal()
        {
            List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string,Vector<double>>>();

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
            uint sampleAmount = 0;
            if(signal != null)
                sampleAmount = (uint) signal.Count;
            return sampleAmount;

        }

        /*
        static void Main()
        {
            XMLConverter xml = new XMLConverter(@"C:\temp\2.xml");
            xml.loadXMLFile(xml.pathIn);
            uint f = xml.getFrequency();
            Console.WriteLine("Frequency: " + f + " Hz");

            List<Tuple<string, Vector<double>>> signals = xml.getSignal();
            foreach (var tuple in signals)
            {
                Console.WriteLine("Lead name: " + tuple.Item1);
                Console.WriteLine("Signal Vector in uV: " + tuple.Item2);
                uint sampleAmount = xml.getSampleAmount(tuple.Item2);
                Console.WriteLine("Sample amount: " + sampleAmount.ToString());
                Console.WriteLine();

            }
            Console.Read();
        }*/
    }
}
