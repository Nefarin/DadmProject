using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Sleep_Apnea;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class Sleep_Apnea_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Sleep_Apnea_Data _data;

        public Sleep_Apnea_Data Data
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

        public Sleep_Apnea_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Sleep_Apnea_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Sleep_Apnea_Data)
            {
                Sleep_Apnea_Data basicData = data as Sleep_Apnea_Data;

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

                List<Tuple<string, List<Tuple<int, int>>>> list = basicData.Detected_Apnea;
                foreach (var tuple in list)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, "Detected_Apnea", string.Empty);
                    module.AppendChild(moduleNode);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    moduleNode.AppendChild(lead);

                    List<Tuple<int, int>> internalList = tuple.Item2;
                    XmlElement internalListNode = file.CreateElement(string.Empty, "tupleList", string.Empty);
                    moduleNode.AppendChild(internalListNode);

                    foreach (var internalTuple in internalList)
                    {
                        XmlElement tupleNode = file.CreateElement(string.Empty, "tuple", string.Empty);
                        internalListNode.AppendChild(tupleNode);

                        XmlElement item1 = file.CreateElement(string.Empty, "item1", string.Empty);
                        XmlText item1Value = file.CreateTextNode(internalTuple.Item1.ToString());
                        item1.AppendChild(item1Value);
                        tupleNode.AppendChild(item1);

                        XmlElement item2 = file.CreateElement(string.Empty, "item2", string.Empty);
                        XmlText item2Value = file.CreateTextNode(internalTuple.Item2.ToString());
                        item2.AppendChild(item2Value);
                        tupleNode.AppendChild(item2);
                    }
                    
                }

                List<Tuple<string, List<List<double>>>> list1 = basicData.h_amp;
                foreach (var tuple in list1)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, "h_amp", string.Empty);
                    module.AppendChild(moduleNode);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    moduleNode.AppendChild(lead);

                    List<List<double>> internalList = tuple.Item2;
                    XmlElement internalListNode = file.CreateElement(string.Empty, "samplesList", string.Empty);
                    moduleNode.AppendChild(internalListNode);

                    foreach (var secondInternalList in internalList)
                    {
                        XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                        string samplesText = null;
                        foreach (var value in secondInternalList)
                        {
                            samplesText += value.ToString() + " ";
                        }
                        XmlText samplesValue = file.CreateTextNode(samplesText);
                        samples.AppendChild(samplesValue);
                        internalListNode.AppendChild(samples);
                        
                    }
                }

                List<Tuple<string, double>> list2 = basicData.il_Apnea;
                foreach (var tuple in list2)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, "il_Apnea", string.Empty);
                    module.AppendChild(moduleNode);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    moduleNode.AppendChild(lead);

                    XmlElement item2 = file.CreateElement(string.Empty, "item2", string.Empty);
                    XmlText item2Value = file.CreateTextNode(tuple.Item2.ToString());
                    item2.AppendChild(item2Value);
                    moduleNode.AppendChild(item2);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }
    }
}
