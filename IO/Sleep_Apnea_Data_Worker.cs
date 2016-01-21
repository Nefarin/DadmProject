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
    /// <summary>
    /// Class that saves and loads Sleep_Apnea_Data from internal XML file
    /// </summary>
    public class Sleep_Apnea_Data_Worker
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
        private Sleep_Apnea_Data _data;

        /// <summary>
        /// Gets or sets Sleep_Apnea_Data
        /// </summary>
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

        //METHODS
        /// <summary>
        /// Saves Sleep_Apnea_Data
        /// </summary>
        /// <param name="data">Sleep_Apnea_Data</param>
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
                        
                        StringBuilder builder = new StringBuilder();
                        foreach (var value in secondInternalList)
                        {
                            builder.Append(value.ToString());
                            builder.Append(" ");
                        }
                        string samplesText = builder.ToString();

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

        /// <summary>
        /// Loads Sleep_Apnea_Data
        /// </summary>
         public void Load()
        {
            Sleep_Apnea_Data basicData = new Sleep_Apnea_Data();
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
                    List<Tuple<string, List<Tuple<int, int>>>> Detected_Apnea_List = new List<Tuple<string, List<Tuple<int, int>>>>();
                    XmlNodeList nodeList = module.SelectNodes("Detected_Apnea");
                    foreach (XmlNode node in nodeList)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNodeList tupleList = node.SelectNodes("tupleList/tuple");
                        List<Tuple<int, int>> readTupleList = new List<Tuple<int,int>>();
                        foreach (XmlNode tuple in tupleList)
                        {
                            XmlNode item1 = tuple["item1"];
                            int convertedItem1 = Convert.ToInt32(item1.InnerText);

                            XmlNode item2 = tuple["item2"];
                            int convertedItem2 = Convert.ToInt32(item2.InnerText);

                            Tuple<int, int> readTuple = Tuple.Create(convertedItem1, convertedItem2);
                            readTupleList.Add(readTuple);
                        }
                        Tuple<string, List<Tuple<int, int>>> readDetected_Apnea = Tuple.Create(readLead, readTupleList);
                        Detected_Apnea_List.Add(readDetected_Apnea);
                    }
                    basicData.Detected_Apnea = Detected_Apnea_List;

                    List<Tuple<string, List<List<double>>>> h_amp_List = new List<Tuple<string, List<List<double>>>>();
                    XmlNodeList nodeList1 = module.SelectNodes("h_amp");
                    foreach (XmlNode node in nodeList1)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNodeList samplesList = node.SelectNodes("samplesList/samples");
                        List<List<double>> readSamplesList = new List<List<double>>();
                        foreach (XmlNode samples in samplesList)
                        {
                            string readSamples = samples.InnerText;
                            List<double> readDigits = stringToList(readSamples);
                            readSamplesList.Add(readDigits);
                        }
                        Tuple<string, List<List<double>>> readTuple = Tuple.Create(readLead, readSamplesList);
                        h_amp_List.Add(readTuple);
                    }
                    basicData.h_amp = h_amp_List;

                    List<Tuple<string, double>> il_Apnea_List = new List<Tuple<string, double>>();
                    XmlNodeList nodeList2 = module.SelectNodes("il_Apnea");
                    foreach (XmlNode node in nodeList2)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode item2 = node["item2"];
                        double convertedItem2 = Convert.ToDouble(item2.InnerText);

                        Tuple<string, double> readTuple = Tuple.Create(readLead, convertedItem2);
                        il_Apnea_List.Add(readTuple);
                    }
                    basicData.il_Apnea = il_Apnea_List;

                }
            }
            this.Data = basicData;
        }

        /// <summary>
        /// Converts string to list
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>list</returns>
        public static List<double> stringToList(string input)
        {
            double[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => Convert.ToDouble(digit))
                              .ToArray();
            List<double> list = new List<double>();
            for (int i = 0; i < digits.Length; i++ )
                list.Add(digits[i]);
            return list;
        }
    }
}
