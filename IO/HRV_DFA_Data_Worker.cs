﻿using System;
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
    public class HRV_DFA_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private HRV_DFA_Data _data;

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
                module.SetAttribute("name", moduleName);
                root.AppendChild(module);

                List<Tuple<string, Vector<double>>> dfaNumberN = basicData.DfaNumberN;
                foreach (var tuple in dfaNumberN)
                {
                    XmlElement DfaNumberN = file.CreateElement(string.Empty, "DfaNumberN", string.Empty);
                    module.AppendChild(DfaNumberN);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    DfaNumberN.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    DfaNumberN.AppendChild(samples);
                }

                List<Tuple<string, Vector<double>>> dfaValueFn = basicData.DfaValueFn;
                foreach (var tuple in dfaValueFn)
                {
                    XmlElement DfaValueFn = file.CreateElement(string.Empty, "DfaValueFn", string.Empty);
                    module.AppendChild(DfaValueFn);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    DfaValueFn.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    DfaValueFn.AppendChild(samples);
                }

                List<Tuple<string, Vector<double>>> paramAlpha = basicData.ParamAlpha;
                foreach (var tuple in paramAlpha)
                {
                    XmlElement ParamAlpha = file.CreateElement(string.Empty, "ParamAlpha", string.Empty);
                    module.AppendChild(ParamAlpha);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    ParamAlpha.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    ParamAlpha.AppendChild(samples);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

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
                    List<Tuple<string, Vector<double>>> DfaNumberN = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList dfaNumberN = module.SelectNodes("DfaNumberN");
                    foreach (XmlNode node in dfaNumberN)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readDfaNumberN = Tuple.Create(readLead, readDigits);
                        DfaNumberN.Add(readDfaNumberN);
                    }
                    basicData.DfaNumberN = DfaNumberN;


                    List<Tuple<string, Vector<double>>> DfaValueFn = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList dfaValueFn = module.SelectNodes("DfaValueFn");
                    foreach (XmlNode node in dfaValueFn)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readDfaValueFn = Tuple.Create(readLead, readDigits);
                        DfaValueFn.Add(readDfaValueFn);
                    }
                    basicData.DfaValueFn = DfaValueFn;

                    List<Tuple<string, Vector<double>>> ParamAlpha = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList paramAlpha = module.SelectNodes("ParamAlpha");
                    foreach (XmlNode node in paramAlpha)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readParamAlpha = Tuple.Create(readLead, readDigits);
                        ParamAlpha.Add(readParamAlpha);
                    }
                    basicData.ParamAlpha = ParamAlpha;
                }
                }
                this.Data = basicData;
            }
        }
    }

