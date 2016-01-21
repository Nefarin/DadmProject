using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV2;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that saves and loads HRV2_Data from internal XML file
    /// </summary>
    public class HRV2_Data_Worker
    {
        //FIELDS
        /// <summary>
        /// Stores internal XML file directory
        /// </summary>
        string directory;

        /// <summary>
        /// Stores analysis name
        /// </summary>
        string analysisName;
        private HRV2_Data _data;

        /// <summary>
        /// Gets or sets HRV2_Data
        /// </summary>
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

        //METHODS
        /// <summary>
        /// Saves HRV2_Data
        /// </summary>
        /// <param name="data">HRV2_Data</param>
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
                List<Tuple<string, HRV2.Histogram2>> hist = basicData.HistogramData;
                foreach (var tuple in hist)
                {
                    XmlElement histogramData = file.CreateElement(string.Empty, "HistogramData", string.Empty);
                    module.AppendChild(histogramData);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    histogramData.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    ObservableCollection<HRV2.Sample> values = tuple.Item2.Samples;
                    StringBuilder builder = new StringBuilder();
                    foreach (var value in values)
                    {
                        builder.Append(value.ToString());
                    }
                    string samplesText = builder.ToString();

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    histogramData.AppendChild(samples);
                }

                
                object[] Properties = {basicData.PoincarePlotData_x, basicData.PoincarePlotData_y};
                string[] Names = { "PoincarePlotData_x", "PoincarePlotData_y"};
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<Tuple<string, Vector<double>>> list = (List<Tuple<string, Vector<double>>>)property;
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

                object[] sProperties = { basicData.Tinn, basicData.TriangleIndex, basicData.SD1, basicData.SD2, basicData.ElipseCenter };
                string[] sNames = { "Tinn", "TriangleIndex", "SD1", "SD2", "ElipseCenter" };
                int slicznik = 0;

                foreach (var property in sProperties)
                {
                    List<double> value = (List<double>)property;
                    foreach(var v in value)
                    {
                        XmlElement moduleNode = file.CreateElement(string.Empty, sNames[slicznik], string.Empty);
                        XmlText textValue = file.CreateTextNode(v.ToString());
                        moduleNode.AppendChild(textValue);
                        module.AppendChild(moduleNode);
                    }

                    slicznik++;

                }
                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        /// <summary>
        /// Loads and sets HRV2_Data
        /// </summary>
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
                    /*
                    List<Tuple<string, HRV2.Histogram2>> list = new List<Tuple<string, HRV2.Histogram2>>();
                    XmlNodeList histogramData = module.SelectNodes("HistogramData");
                    foreach (XmlNode node in histogramData)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        List<string> readDigits = new List<string>();

                        Regex regex = new Regex(@"^\d$");
                        Match match = regex.Match(readSamples);
                        if (match.Success)
                        {
                            readDigits.Add(match.Value);
                        }

                        HRV2.Sample readSample = new HRV2.Sample();
                        float lowest = float.Parse(readDigits[1]);
                        readSample.LowestValue = lowest;

                        float highest = float.Parse(readDigits[2]);
                        readSample.HighestValue = highest;

                        int count = Convert.ToInt32(readDigits[3]);
                        readSample.Count = count;

                        ObservableCollection<HRV2.Sample> histogramValues = new ObservableCollection<HRV2.Sample>();
                        histogramValues.Add(readSample);
                        HRV2.Histogram2 readHistogram = new HRV2.Histogram2();
                        readHistogram.Samples = histogramValues;

                        Tuple<string, HRV2.Histogram2> readHistogramData = Tuple.Create(readLead, readHistogram);
                        list.Add(readHistogramData);
                    }
                     * */
                    //basicData.HistogramData = list;
                    basicData.HistogramData = null;

                    List<Tuple<string, Vector<double>>> PoincarePlotData_x = new List<Tuple<string,Vector<double>>>();
                    XmlNodeList nodes = module.SelectNodes("PoincarePlotData_x");
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readPoincarePlotData_x = Tuple.Create(readLead, readDigits);
                        PoincarePlotData_x.Add(readPoincarePlotData_x);
                    }
                    basicData.PoincarePlotData_x = PoincarePlotData_x;

                    List<Tuple<string, Vector<double>>> PoincarePlotData_y = new List<Tuple<string,Vector<double>>>();
                    XmlNodeList nodes1 = module.SelectNodes("PoincarePlotData_y");
                    foreach (XmlNode node in nodes1)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readPoincarePlotData_y = Tuple.Create(readLead, readDigits);
                        PoincarePlotData_y.Add(readPoincarePlotData_y);
                    }
                    basicData.PoincarePlotData_y = PoincarePlotData_y;
                    
                    List<double> list1 = new List<double>();
                    XmlNodeList nodes2 = module.SelectNodes("Tinn");
                    foreach (XmlNode node in nodes2)
                    {
                        string readVariable = node.InnerText;
                        double convertedVariable = Convert.ToDouble(readVariable);

                        list1.Add(convertedVariable);
                    }
                    basicData.Tinn = list1;

                    List<double> list2 = new List<double>();
                    XmlNodeList nodes3 = module.SelectNodes("TriangleIndex");
                    foreach (XmlNode node in nodes3)
                    {
                        string readVariable = node.InnerText;
                        double convertedVariable = Convert.ToDouble(readVariable);

                        list2.Add(convertedVariable);
                    }
                    basicData.TriangleIndex = list2;

                    List<double> list3 = new List<double>();
                    XmlNodeList nodes4 = module.SelectNodes("SD1");
                    foreach (XmlNode node in nodes4)
                    {
                        string readVariable = node.InnerText;
                        double convertedVariable = Convert.ToDouble(readVariable);

                        list3.Add(convertedVariable);
                    }
                    basicData.SD1 = list3;

                    List<double> list4 = new List<double>();
                    XmlNodeList nodes5 = module.SelectNodes("SD2");
                    foreach (XmlNode node in nodes5)
                    {
                        string readVariable = node.InnerText;
                        double convertedVariable = Convert.ToDouble(readVariable);

                        list4.Add(convertedVariable);
                    }
                    basicData.SD2 = list4;

                    List<double> list5 = new List<double>();
                    XmlNodeList nodes6 = module.SelectNodes("ElipseCenter");
                    foreach (XmlNode node in nodes6)
                    {
                        string readVariable = node.InnerText;
                        double convertedVariable = Convert.ToDouble(readVariable);

                        list5.Add(convertedVariable);
                    }
                    basicData.ElipseCenter = list5;
                }
            }
            this.Data = basicData;
        }
    }
}
