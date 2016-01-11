using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV2;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class HRV2_Data_Worker
    {
        string directory;
        string analysisName;
        private HRV2_Data _data;

        public HRV2_Data Data
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

        public HRV2_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public HRV2_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is HRV2_Data)
            {
                HRV2_Data basicData = data as HRV2_Data;

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

                List<Tuple<string, Vector<double>>> list = basicData.HistogramData;
                foreach (var tuple in list)
                {
                    XmlElement histogramData = file.CreateElement(string.Empty, "HistogramData", string.Empty);
                    module.AppendChild(histogramData);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    histogramData.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    histogramData.AppendChild(samples);
                }

                object[] Properties = {basicData.PoincarePlotData_x, basicData.PoincarePlotData_y};
                string[] Names = { "PoincarePlotData_x", "PoincarePlotData_y"};
                int licznik = 0;

                foreach (var property in Properties)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, Names[licznik], string.Empty);
                    module.AppendChild(moduleNode);

                    Tuple<string, Vector<double>> tuple = (Tuple<string, Vector<double>>)property;
                    XmlElement lead = file.CreateElement(string.Empty, "item1", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    moduleNode.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    moduleNode.AppendChild(samples);

                    licznik++;
                    
                }

                object[] sProperties = { basicData.Tinn, basicData.TriangleIndex, basicData.SD1, basicData.SD2 };
                string[] sNames = { "Tinn", "TriangleIndex", "SD1", "SD2"};
                int slicznik = 0;

                foreach (var property in sProperties)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, sNames[slicznik], string.Empty);
                    
                    double value = (double)property;
                    XmlText textValue = file.CreateTextNode(value.ToString());
                    
                    moduleNode.AppendChild(textValue);
                    module.AppendChild(moduleNode);

                    slicznik++;

                }
                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            HRV2_Data basicData = new HRV2_Data();
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
                    XmlNodeList histogramData = module.SelectNodes("HistogramData");
                    foreach (XmlNode node in histogramData)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readHistogramData = Tuple.Create(readLead, readDigits);
                        list.Add(readHistogramData);
                    }
                    basicData.HistogramData = list;

                    XmlNode PoincarePlotData_x = module["PoincarePlotData_x"];

                    XmlNode item1X = PoincarePlotData_x["item1"];
                    string readitem1X = item1X.InnerText;

                    XmlNode samplesX = PoincarePlotData_x["samples"];
                    string readSamplesX = samplesX.InnerText;
                    Vector<double> readDigitsX = converter.stringToVector(readSamplesX);

                    Tuple<string, Vector<double>> readPoincarePlotData_x = Tuple.Create(readitem1X, readDigitsX);
                    basicData.PoincarePlotData_x = readPoincarePlotData_x;

                    XmlNode PoincarePlotData_y = module["PoincarePlotData_y"];

                    XmlNode item1Y = PoincarePlotData_y["item1"];
                    string readitem1Y = item1Y.InnerText;

                    XmlNode samplesY = PoincarePlotData_y["samples"];
                    string readSamplesY = samplesY.InnerText;
                    Vector<double> readDigitsY = converter.stringToVector(readSamplesY);

                    Tuple<string, Vector<double>> readPoincarePlotData_y = Tuple.Create(readitem1Y, readDigitsY);
                    basicData.PoincarePlotData_y = readPoincarePlotData_y;

                    XmlNode sNode = module["Tinn"];
                    basicData.Tinn = Convert.ToDouble(sNode.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode1 = module["TriangleIndex"];
                    basicData.TriangleIndex = Convert.ToDouble(sNode1.InnerText, new System.Globalization.NumberFormatInfo());
                    
                    XmlNode sNode2 = module["SD1"];
                    basicData.SD1 = Convert.ToDouble(sNode2.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode3 = module["SD2"];
                    basicData.SD2 = Convert.ToDouble(sNode3.InnerText, new System.Globalization.NumberFormatInfo());
                    
                }
            }
            this.Data = basicData;
        }
    }
}
