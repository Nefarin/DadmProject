using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV1;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class HRV1_Data_Worker
    {
        string directory;
        string analysisName;
        private HRV1_Data _data;

        public HRV1_Data Data
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

        public HRV1_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public HRV1_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is HRV1_Data)
            {
                HRV1_Data basicData = data as HRV1_Data;

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

                object[] Properties = {basicData.FreqBasedParams, basicData.TimeBasedParams, basicData.RInstants, basicData.RRIntervals};
                string[] Names = { "FreqBasedParams", "TimeBasedParams", "RInstants", "RRIntervals" };
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<Tuple<string, Vector<double>>> list = (List<Tuple<string, Vector<double>>>) property;
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
            HRV1_Data basicData = new HRV1_Data();
            XMLConverter converter = new XMLConverter(analysisName);

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
                    List<Tuple<string, Vector<double>>> list = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList nodes = module.SelectNodes("TimeBasedParams");
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> read = Tuple.Create(readLead, readDigits);
                        list.Add(read);
                    }
                    basicData.TimeBasedParams = list;

                    List<Tuple<string, Vector<double>>> FreqBasedParams = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList freqBasedParams = module.SelectNodes("FreqBasedParams");
                    foreach (XmlNode node in freqBasedParams)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> read = Tuple.Create(readLead, readDigits);
                        FreqBasedParams.Add(read);
                    }
                    basicData.FreqBasedParams = FreqBasedParams;

                    List<Tuple<string, Vector<double>>> RInstants = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList rInstants = module.SelectNodes("RInstants");
                    foreach (XmlNode node in rInstants)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> read = Tuple.Create(readLead, readDigits);
                        RInstants.Add(read);
                    }
                    basicData.RInstants = RInstants;

                    List<Tuple<string, Vector<double>>> RRIntervals = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList rrIntervals = module.SelectNodes("RRIntervals");
                    foreach (XmlNode node in rrIntervals)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> read = Tuple.Create(readLead, readDigits);
                        RRIntervals.Add(read);
                    }
                    basicData.RRIntervals = RRIntervals;
                }
            }
            this.Data = basicData;
        }
    }
}
