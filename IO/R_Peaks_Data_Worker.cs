using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.R_Peaks;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class R_Peaks_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private R_Peaks_Data _data;

        public R_Peaks_Data Data
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

        public R_Peaks_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public R_Peaks_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is R_Peaks_Data)
            {
                R_Peaks_Data basicData = data as R_Peaks_Data;

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

                List<Tuple<string, Vector<double>>> rPeaks = basicData.RPeaks;
                foreach (var tuple in rPeaks)
                {
                    XmlElement RPeaks = file.CreateElement(string.Empty, "RPeaks", string.Empty);
                    module.AppendChild(RPeaks);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    RPeaks.AppendChild(lead);

                    XmlElement indices = file.CreateElement(string.Empty, "indices", string.Empty);
                    string indicesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        indicesText += value.ToString() + " ";
                    }

                    XmlText indicesValue = file.CreateTextNode(indicesText);
                    indices.AppendChild(indicesValue);
                    RPeaks.AppendChild(indices);
                }

                List<Tuple<string, Vector<double>>> rrIntervals = basicData.RRInterval;
                foreach (var tuple in rrIntervals)
                {
                    XmlElement RRInterval = file.CreateElement(string.Empty, "RRInterval", string.Empty);
                    module.AppendChild(RRInterval);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    RRInterval.AppendChild(lead);

                    XmlElement intervals = file.CreateElement(string.Empty, "intervals", string.Empty);
                    string intervalsText = null;
                    foreach (var value in tuple.Item2)
                    {
                        intervalsText += value.ToString() + " ";
                    }

                    XmlText intervalsValue = file.CreateTextNode(intervalsText);
                    intervals.AppendChild(intervalsValue);
                    RRInterval.AppendChild(intervals);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            R_Peaks_Data basicData = new R_Peaks_Data();
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
                    List<Tuple<string, Vector<double>>> RPeaks = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList rPeaks = module.SelectNodes("RPeaks");
                    foreach (XmlNode rPeak in rPeaks)
                    {
                        XmlNode lead = rPeak["lead"];
                        string readLead = lead.InnerText;

                        XmlNode indices = rPeak["indices"];
                        string readIndices = indices.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readIndices);

                        Tuple<string, Vector<double>> readRPeak = Tuple.Create(readLead, readDigits);
                        RPeaks.Add(readRPeak);
                    }
                    basicData.RPeaks = RPeaks;

                    List<Tuple<string, Vector<double>>> RRInterval = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList intervals = module.SelectNodes("RRInterval");
                    foreach (XmlNode interval in intervals)
                    {
                        XmlNode lead = interval["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = interval["intervals"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readInterval = Tuple.Create(readLead, readDigits);
                        RRInterval.Add(readInterval);
                    }
                    basicData.RRInterval = RRInterval;
                }
            }
            this.Data = basicData;
        }
    }
}
