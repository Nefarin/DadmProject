﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;
using EKG_Project.Modules.TestModule3;

namespace EKG_Project.IO
{
    public class TestModule3_Data_Worker : IECG_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private TestModule3_Data _data;

        public TestModule3_Data Data
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

        public TestModule3_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public TestModule3_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Load()
        {
            //TestModule3_Data basicData = new TestModule3_Data();
            //XMLConverter converter = new XMLConverter(analysisName);

            //XmlDocument file = new XmlDocument();
            //string fileName = analysisName + "_TestModule3.xml";
            //file.Load(System.IO.Path.Combine(directory, fileName));

            //XmlNodeList modules = file.SelectNodes("EKG/module");

            //string moduleName = this.GetType().Name;
            //moduleName = moduleName.Replace("_Data_Worker", "");

            //foreach (XmlNode module in modules)
            //{
            //    if (module.Attributes["name"].Value == moduleName)
            //    {
            //        XmlNode frequency = module["frequency"];
            //        basicData.Frequency = Convert.ToUInt32(frequency.InnerText, new System.Globalization.NumberFormatInfo());

            //        XmlNode sampleAmount = module["sampleAmount"];
            //        basicData.SampleAmount = Convert.ToUInt32(sampleAmount.InnerText, new System.Globalization.NumberFormatInfo());

            //        List<Tuple<string, Vector<double>>> Signals = new List<Tuple<string, Vector<double>>>();
            //        XmlNodeList signals = module.SelectNodes("signal");
            //        foreach (XmlNode signal in signals)
            //        {
            //            XmlNode lead = signal["lead"];
            //            string readLead = lead.InnerText;

            //            XmlNode samples = signal["samples"];
            //            string readSamples = samples.InnerText;
            //            Vector<double> readDigits = converter.stringToVector(readSamples);

            //            Tuple<string, Vector<double>> readSignal = Tuple.Create(readLead, readDigits);
            //            Signals.Add(readSignal);
            //        }
            //        basicData.Output = Signals;
            //    }
            //}
            //this.Data = basicData;
        }

        public void Save(ECG_Data data)
        {
            if (data is TestModule3_Data)
            {
                //TestModule3_Data basicData = data as TestModule3_Data;

                //ECG_Worker ew = new ECG_Worker();
                //ew.CreateFile();
                //XmlDocument file = ew.InternalXMLFile;
                //XmlNode root = file.SelectSingleNode("EKG");

                //XmlElement module = file.CreateElement(string.Empty, "module", string.Empty);
                //string moduleName = this.GetType().Name;
                //moduleName = moduleName.Replace("_Data_Worker", "");
                //module.SetAttribute("name", moduleName);
                //root.AppendChild(module);

                //XmlElement frequency = file.CreateElement(string.Empty, "frequency", string.Empty);
                //XmlText frequencyValue = file.CreateTextNode(basicData.Frequency.ToString());
                //frequency.AppendChild(frequencyValue);
                //module.AppendChild(frequency);

                //XmlElement sampleAmount = file.CreateElement(string.Empty, "sampleAmount", string.Empty);
                //XmlText sampleAmountValue = file.CreateTextNode(basicData.SampleAmount.ToString());
                //sampleAmount.AppendChild(sampleAmountValue);
                //module.AppendChild(sampleAmount);

                //List<Tuple<string, Vector<double>>> signals = basicData.Output;
                //foreach (var tuple in signals)
                //{
                //    XmlElement signal = file.CreateElement(string.Empty, "signal", string.Empty);
                //    module.AppendChild(signal);

                //    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                //    XmlText leadValue = file.CreateTextNode(tuple.Item1.ToString());
                //    lead.AppendChild(leadValue);
                //    signal.AppendChild(lead);

                //    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                //    string samplesText = null;
                //    foreach (var value in tuple.Item2)
                //    {
                //        samplesText += value.ToString() + " ";
                //    }

                //    XmlText samplesValue = file.CreateTextNode(samplesText);
                //    samples.AppendChild(samplesValue);
                //    signal.AppendChild(samples);

                //}

                //ew.InternalXMLFile = file;

                //string fileName = analysisName + "_TestModule3.xml";
                ////Console.WriteLine(System.IO.Path.Combine(directory, fileName));
                //file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }
    }
}
