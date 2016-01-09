using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.QT_Disp;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class QT_Disp_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private QT_Disp_Data _data;

        public QT_Disp_Data Data
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

        public QT_Disp_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public QT_Disp_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is QT_Disp_Data)
            {
                QT_Disp_Data basicData = data as QT_Disp_Data;

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

                object[] Properties = {basicData.QT_disp_local, basicData.QT_mean, basicData.QT_std};
                string[] Names = { "QT_disp_local", "QT_mean", "QT_std" };
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<Tuple<String, double>> list = (List<Tuple<String, double>>)property;
                    foreach (var tuple in list)
                    {
                        XmlElement moduleNode = file.CreateElement(string.Empty, Names[licznik], string.Empty);
                        module.AppendChild(moduleNode);

                        XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                        XmlText leadValue = file.CreateTextNode(tuple.Item1);
                        lead.AppendChild(leadValue);
                        moduleNode.AppendChild(lead);

                        XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                        XmlText samplesValue = file.CreateTextNode(tuple.Item2.ToString());
                        samples.AppendChild(samplesValue);
                        moduleNode.AppendChild(samples);
                    }

                    licznik++;
                }

                XmlElement QT_disp_global = file.CreateElement(string.Empty, "QT_disp_global", string.Empty);
                XmlText value = file.CreateTextNode(basicData.QT_disp_global.ToString());
                QT_disp_global.AppendChild(value);
                module.AppendChild(QT_disp_global);

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            QT_Disp_Data basicData = new QT_Disp_Data();

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
                    List<Tuple<String, double>> list = new List<Tuple<String, double>>();
                    XmlNodeList nodes = module.SelectNodes("QT_disp_local");
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        double convertedSamples = Convert.ToDouble(readSamples, new System.Globalization.NumberFormatInfo());

                        Tuple<String, double> read = Tuple.Create(readLead, convertedSamples);
                        list.Add(read);
                    }
                    basicData.QT_disp_local = list;

                    List<Tuple<String, double>> list1 = new List<Tuple<String, double>>();
                    XmlNodeList nodes1 = module.SelectNodes("QT_mean");
                    foreach (XmlNode node in nodes1)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;


                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        double convertedSamples = Convert.ToDouble(readSamples, new System.Globalization.NumberFormatInfo());

                        Tuple<String, double> read = Tuple.Create(readLead, convertedSamples);
                        list1.Add(read);
                    }
                    basicData.QT_mean = list1;

                    List<Tuple<String, double>> list2 = new List<Tuple<String, double>>();
                    XmlNodeList nodes2 = module.SelectNodes("QT_std");
                    foreach (XmlNode node in nodes2)
                    {
                        XmlNode lead = node["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = node["samples"];
                        string readSamples = samples.InnerText;
                        double convertedSamples = Convert.ToDouble(readSamples, new System.Globalization.NumberFormatInfo());

                        Tuple<String, double> read = Tuple.Create(readLead, convertedSamples);
                        list2.Add(read);
                    }
                    basicData.QT_std = list2;

                    XmlNode qt_disp_global = module["QT_disp_global"];
                    basicData.QT_disp_global = Convert.ToDouble(qt_disp_global.InnerText, new System.Globalization.NumberFormatInfo());
                }
            }
            this.Data = basicData;
        }
    }
}
