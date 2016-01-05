using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Waves;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class Waves_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Waves_Data _data;

        public Waves_Data Data
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

        public Waves_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Waves_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Waves_Data)
            {
                Waves_Data basicData = data as Waves_Data;

                ECG_Worker ew = new ECG_Worker();
                XmlDocument file = new XmlDocument();
                string fileName = analysisName + "_Data.xml";
                file.Load(System.IO.Path.Combine(directory, fileName));
                XmlNode root = file.SelectSingleNode("EKG");

                XmlElement module = file.CreateElement(string.Empty, "module", string.Empty);
                string moduleName = this.GetType().Name;
                moduleName = moduleName.Replace("_Data_Worker", "");
                module.SetAttribute("name", moduleName);
                root.AppendChild(module);

                List<Tuple<string, Vector<uint>>> qrsOnsets = basicData.QRSOnsets;
                foreach (var tuple in qrsOnsets)
                {
                    XmlElement QRSOnsets = file.CreateElement(string.Empty, "QRSOnsets", string.Empty);
                    module.AppendChild(QRSOnsets);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    QRSOnsets.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    QRSOnsets.AppendChild(samples);
                }

                List<Tuple<string, Vector<uint>>> qrsEnds = basicData.QRSEnds;
                foreach (var tuple in qrsEnds)
                {
                    XmlElement QRSEnds = file.CreateElement(string.Empty, "QRSEnds", string.Empty);
                    module.AppendChild(QRSEnds);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    QRSEnds.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    QRSEnds.AppendChild(samples);
                }

                List<Tuple<string, Vector<uint>>> pOnsets = basicData.POnsets;
                foreach (var tuple in pOnsets)
                {
                    XmlElement POnsets = file.CreateElement(string.Empty, "POnsets", string.Empty);
                    module.AppendChild(POnsets);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    POnsets.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    POnsets.AppendChild(samples);
                }

                List<Tuple<string, Vector<uint>>> pEnds = basicData.PEnds;
                foreach (var tuple in pEnds)
                {
                    XmlElement PEnds = file.CreateElement(string.Empty, "PEnds", string.Empty);
                    module.AppendChild(PEnds);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    PEnds.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    PEnds.AppendChild(samples);
                }

                List<Tuple<string, Vector<uint>>> tEnds = basicData.TEnds;
                foreach (var tuple in tEnds)
                {
                    XmlElement TEnds = file.CreateElement(string.Empty, "TEnds", string.Empty);
                    module.AppendChild(TEnds);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    TEnds.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    TEnds.AppendChild(samples);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            Waves_Data basicData = new Waves_Data();

            XmlDocument file = new XmlDocument();
            string fileName = analysisName + "_Data.xml";
            file.Load(System.IO.Path.Combine(directory, fileName));

            XmlNodeList modules = file.SelectNodes("EKG/module");

            string moduleName = this.GetType().Name;
            moduleName = moduleName.Replace("_Data_Worker", "");

            foreach (XmlNode module in modules)
            {
                if (module.Attributes["name"].Value == moduleName)
                {
                    List<Tuple<string, Vector<uint>>> QRSOnsets = new List<Tuple<string, Vector<uint>>>();
                    XmlNodeList qrsOnsets = module.SelectNodes("QRSOnsets");
                    foreach (XmlNode node in qrsOnsets)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<uint> readDigits = stringToVector(readSamples);

                        Tuple<string, Vector<uint>> readQRSOnsets = Tuple.Create(readLead, readDigits);
                        QRSOnsets.Add(readQRSOnsets);
                    }
                    basicData.QRSOnsets = QRSOnsets;

                    List<Tuple<string, Vector<uint>>> QRSEnds = new List<Tuple<string, Vector<uint>>>();
                    XmlNodeList qrsEnds = module.SelectNodes("QRSEnds");
                    foreach (XmlNode node in qrsEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<uint> readDigits = stringToVector(readSamples);

                        Tuple<string, Vector<uint>> readQRSEnds = Tuple.Create(readLead, readDigits);
                        QRSEnds.Add(readQRSEnds);
                    }
                    basicData.QRSEnds = QRSEnds;

                    List<Tuple<string, Vector<uint>>> POnsets = new List<Tuple<string, Vector<uint>>>();
                    XmlNodeList pOnsets = module.SelectNodes("POnsets");
                    foreach (XmlNode node in pOnsets)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<uint> readDigits = stringToVector(readSamples);

                        Tuple<string, Vector<uint>> readPOnsets = Tuple.Create(readLead, readDigits);
                        POnsets.Add(readPOnsets);
                    }
                    basicData.POnsets = POnsets;

                    List<Tuple<string, Vector<uint>>> PEnds = new List<Tuple<string, Vector<uint>>>();
                    XmlNodeList pEnds = module.SelectNodes("PEnds");
                    foreach (XmlNode node in pEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<uint> readDigits = stringToVector(readSamples);

                        Tuple<string, Vector<uint>> readPEnds = Tuple.Create(readLead, readDigits);
                        PEnds.Add(readPEnds);
                    }
                    basicData.PEnds = PEnds;

                    List<Tuple<string, Vector<uint>>> TEnds = new List<Tuple<string, Vector<uint>>>();
                    XmlNodeList tEnds = module.SelectNodes("TEnds");
                    foreach (XmlNode node in tEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;
                        Console.Write(readLead);

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<uint> readDigits = stringToVector(readSamples);

                        Tuple<string, Vector<uint>> readTEnds = Tuple.Create(readLead, readDigits);
                        TEnds.Add(readTEnds);
                    }
                    basicData.TEnds = TEnds;
                }
            }
            this.Data = basicData;
        }

        public static Vector<uint> stringToVector(string input)
        {
            uint[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => Convert.ToUInt32(digit, new System.Globalization.NumberFormatInfo()))
                              .ToArray();
            Vector<uint> vector = Vector<uint>.Build.Dense(digits.Length);
            vector.SetValues(digits);
            return vector;
        }
    }
}
