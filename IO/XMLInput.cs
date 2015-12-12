using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

namespace EKG_Project.IO
{
    public class XMLInput
    {
        static XmlDocument ecgFile = new XmlDocument();
        static XmlNamespaceManager manager = new XmlNamespaceManager(ecgFile.NameTable);
        static XmlNodeList sequenceValues;
        static XmlNodeList sequenceCodes;

        static string[] getLeadCodes()
        {
            string[] leadCodes = new string[sequenceCodes.Count - 1];
            int i = 0;

            foreach (XmlNode code in sequenceCodes)
            {
                if (code.Attributes["codeSystemName"].Value == "MDC")
                {
                    leadCodes[i] = code.Attributes["code"].Value;
                    i++;
                }
            }
            return leadCodes;
        }

        static uint getFrequency()
        {
            uint frequency = 0;
            foreach (XmlNode sequenceValue in sequenceValues)
            {
                if (sequenceValue.Attributes["xsi:type"].Value == "GLIST_TS")
                {
                    XmlNode increment = sequenceValue["increment"];

                    string incrementValue = increment.Attributes["value"].Value;
                    uint readedIncrement = Convert.ToUInt32(incrementValue);
                    frequency = 1 / readedIncrement; //Hz

                    string incrementUnit = increment.Attributes["unit"].Value; //s
                }
            }
            return frequency;
        }

        static double getOrigin()
        {
            double readedOrigin = 0;
            foreach (XmlNode sequenceValue in sequenceValues)
            {
                if (sequenceValue.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    XmlNode origin = sequenceValue["origin"];

                    string originValue = origin.Attributes["value"].Value;
                    readedOrigin = Convert.ToDouble(originValue);
                    string originUnit = origin.Attributes["unit"].Value; //zwykle uV

                }
            }
            return readedOrigin;
        }

        static double getScale()
        {
            double readedScale = 0;
            foreach (XmlNode sequenceValue in sequenceValues)
            {
                if (sequenceValue.Attributes["xsi:type"].Value == "SLIST_PQ")
                {

                    XmlNode scale = sequenceValue["scale"];

                    string scaleValue = scale.Attributes["value"].Value;
                    readedScale = Convert.ToDouble(scaleValue);
                    string scaleUnit = scale.Attributes["unit"].Value; //uV
                }
            }
            return readedScale;
        }

        static Vector<double> getSignal()
        {
            Vector<double> readedDigits = null;
            foreach (XmlNode sequenceValue in sequenceValues)
            {
                if (sequenceValue.Attributes["xsi:type"].Value == "SLIST_PQ")
                {
                    string digits = sequenceValue["digits"].InnerText;
                    readedDigits = stringToVector(digits);
                }
            }
            readedDigits = normalizeSignal(readedDigits);
            return readedDigits;
        }

        static Vector<double> stringToVector(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => double.Parse(digit))
                              .ToArray();
            Vector<double> vector = Vector<double>.Build.Dense(digits.Length);
            vector.SetValues(digits);
            return vector;

        }

        static Vector<double> normalizeSignal(Vector<double> signal)
        {
            double origin = getOrigin();
            double scale = getScale();
            for (int i = 0; i < signal.Count; i++)
            {
                signal[i] = (signal[i] - origin) / scale;
            }
            return signal;
        }


        /*
        static void Main()
        {
            ecgFile.Load(@"C:\temp\6.xml");
            manager.AddNamespace("hl7", "urn:hl7-org:v3");
            sequenceValues = ecgFile.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence/hl7:value", manager);
            sequenceCodes = ecgFile.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence/hl7:code", manager);

            string[] leadCodes = getLeadCodes();
            foreach (var code in leadCodes)
                Console.WriteLine(code);
            
            Vector<double> signal = getSignal();
            foreach (var value in signal)
                Console.Write(" " + value);
            Console.Read();
        }*/
    }
}
