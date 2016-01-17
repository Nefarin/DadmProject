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
    public class Waves_Data_Worker
    {
        string directory;
        string analysisName;
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

                XmlNodeList existingModules = file.SelectNodes("EKG/module");
                foreach (XmlNode existingModule in existingModules)
                {
                    if (existingModule.Attributes["name"].Value == moduleName)
                    {
                        root.RemoveChild(existingModule);
                    }
                }

                module.SetAttribute("name", moduleName);
                root.AppendChild(module);

                object[] Properties = { basicData.QRSOnsets, basicData.QRSEnds, basicData.POnsets, basicData.PEnds, basicData.TEnds };
                string[] Names = { "QRSOnsets", "QRSEnds", "POnsets", "PEnds", "TEnds" };
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<Tuple<string, List<int>>> list = (List<Tuple<string, List<int>>>)property;
                    foreach (var tuple in list)
                    {
                        XmlElement moduleNode = file.CreateElement(string.Empty, Names[licznik], string.Empty);
                        module.AppendChild(moduleNode);

                        XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                        XmlText leadValue = file.CreateTextNode(tuple.Item1);
                        lead.AppendChild(leadValue);
                        moduleNode.AppendChild(lead);

                        XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                        
                        StringBuilder builder = new StringBuilder();
                        foreach (var value in tuple.Item2)
                        {
                            builder.Append(value.ToString());
                            builder.Append(" ");
                        }
                        string samplesText = builder.ToString();

                        XmlText samplesValue = file.CreateTextNode(samplesText);
                        samples.AppendChild(samplesValue);
                        moduleNode.AppendChild(samples);
                    }

                    licznik++;
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
                    List<Tuple<string, List<int>>> QRSOnsets = new List<Tuple<string, List<int>>>();
                    XmlNodeList qrsOnsets = module.SelectNodes("QRSOnsets");
                    foreach (XmlNode node in qrsOnsets)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<int> readDigits = stringToList(readSamples);

                        Tuple<string, List<int>> readQRSOnsets = Tuple.Create(readLead, readDigits);
                        QRSOnsets.Add(readQRSOnsets);
                    }
                    basicData.QRSOnsets = QRSOnsets;

                    List<Tuple<string, List<int>>> QRSEnds = new List<Tuple<string, List<int>>>();
                    XmlNodeList qrsEnds = module.SelectNodes("QRSEnds");
                    foreach (XmlNode node in qrsEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<int> readDigits = stringToList(readSamples);

                        Tuple<string, List<int>> readQRSEnds = Tuple.Create(readLead, readDigits);
                        QRSEnds.Add(readQRSEnds);
                    }
                    basicData.QRSEnds = QRSEnds;

                    List<Tuple<string, List<int>>> POnsets = new List<Tuple<string, List<int>>>();
                    XmlNodeList pOnsets = module.SelectNodes("POnsets");
                    foreach (XmlNode node in pOnsets)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<int> readDigits = stringToList(readSamples);

                        Tuple<string, List<int>> readPOnsets = Tuple.Create(readLead, readDigits);
                        POnsets.Add(readPOnsets);
                    }
                    basicData.POnsets = POnsets;

                    List<Tuple<string, List<int>>> PEnds = new List<Tuple<string, List<int>>>();
                    XmlNodeList pEnds = module.SelectNodes("PEnds");
                    foreach (XmlNode node in pEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<int> readDigits = stringToList(readSamples);

                        Tuple<string, List<int>> readPEnds = Tuple.Create(readLead, readDigits);
                        PEnds.Add(readPEnds);
                    }
                    basicData.PEnds = PEnds;

                    List<Tuple<string, List<int>>> TEnds = new List<Tuple<string, List<int>>>();
                    XmlNodeList tEnds = module.SelectNodes("TEnds");
                    foreach (XmlNode node in tEnds)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<int> readDigits = stringToList(readSamples);

                        Tuple<string, List<int>> readTEnds = Tuple.Create(readLead, readDigits);
                        TEnds.Add(readTEnds);
                    }
                    basicData.TEnds = TEnds;
                }
            }
            this.Data = basicData;
        }

        public static List<int> stringToList(string input)
        {
            int[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => Convert.ToInt32(digit))
                              .ToArray();
            List<int> list = new List<int>();
            for (int i = 0; i < digits.Length; i++ )
                list.Add(digits[i]);
            return list;
        }
    }
}
