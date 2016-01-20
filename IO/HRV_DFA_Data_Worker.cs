using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRV_DFA;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that saves and loads HRV_DFA_Data from internal XML file
    /// </summary>
    public class HRV_DFA_Data_Worker
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
        private HRV_DFA_Data _data;

        /// <summary>
        /// Gets or sets HRV_DFA_Data
        /// </summary>
        public HRV_DFA_Data Data
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

        public HRV_DFA_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public HRV_DFA_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        //METHODS
        /// <summary>
        /// Saves HRV_DFA_Data
        /// </summary>
        /// <param name="data">HRV_DFA_Data</param>
        public void Save(ECG_Data data)
        {
            if (data is HRV_DFA_Data)
            {
                HRV_DFA_Data basicData = data as HRV_DFA_Data;

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

                object[] Properties = { basicData.DfaNumberN, basicData.DfaValueFn, basicData.ParamAlpha};
                string[] Names = { "DfaNumberN", "DfaValueFn", "ParamAlpha"};
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<Tuple<string, Vector<double>, Vector<double>>> list = (List<Tuple<string, Vector<double>, Vector<double>>>)property;
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

                        XmlElement samples1 = file.CreateElement(string.Empty, "samples1", string.Empty);
                        
                        StringBuilder builder1 = new StringBuilder();
                        foreach (var value in tuple.Item3)
                        {
                            builder1.Append(value.ToString());
                            builder1.Append(" ");
                        }
                        string samplesText1 = builder1.ToString();

                        XmlText samplesValue1 = file.CreateTextNode(samplesText1);
                        samples1.AppendChild(samplesValue1);
                        moduleNode.AppendChild(samples1);
                    }

                    licznik++;
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        /// <summary>
        /// Loads and sets HRV_DFA_Data
        /// </summary>
        public void Load()
        {
            HRV_DFA_Data basicData = new HRV_DFA_Data();
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
                    List<Tuple<string, Vector<double>, Vector<double>>> DfaNumberN = new List<Tuple<string, Vector<double>, Vector<double>>>();
                    XmlNodeList dfaNumberN = module.SelectNodes("DfaNumberN");
                    foreach (XmlNode node in dfaNumberN)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        XmlNode samples1 = node["samples1"];
                        string readSamples1 = samples1.InnerText;
                        Vector<double> readDigits1 = converter.stringToVector(readSamples1);

                        Tuple<string, Vector<double>, Vector<double>> readDfaNumberN = Tuple.Create(readLead, readDigits, readDigits1);
                        DfaNumberN.Add(readDfaNumberN);
                    }
                    basicData.DfaNumberN = DfaNumberN;


                    List<Tuple<string, Vector<double>, Vector<double>>> DfaValueFn = new List<Tuple<string, Vector<double>, Vector<double>>>();
                    XmlNodeList dfaValueFn = module.SelectNodes("DfaValueFn");
                    foreach (XmlNode node in dfaValueFn)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        XmlNode samples1 = node["samples1"];
                        string readSamples1 = samples1.InnerText;
                        Vector<double> readDigits1 = converter.stringToVector(readSamples1);

                        Tuple<string, Vector<double>, Vector<double>> readDfaValueFn = Tuple.Create(readLead, readDigits, readDigits1);
                        DfaValueFn.Add(readDfaValueFn);
                    }
                    basicData.DfaValueFn = DfaValueFn;

                    List<Tuple<string, Vector<double>, Vector<double>>> ParamAlpha = new List<Tuple<string, Vector<double>, Vector<double>>>();
                    XmlNodeList paramAlpha = module.SelectNodes("ParamAlpha");
                    foreach (XmlNode node in paramAlpha)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        XmlNode samples1 = node["samples1"];
                        string readSamples1 = samples1.InnerText;
                        Vector<double> readDigits1 = converter.stringToVector(readSamples1);

                        Tuple<string, Vector<double>, Vector<double>> readParamAlpha = Tuple.Create(readLead, readDigits, readDigits1);
                        ParamAlpha.Add(readParamAlpha);
                    }
                    basicData.ParamAlpha = ParamAlpha;
                }
                }
                this.Data = basicData;
            }
        }
    }

