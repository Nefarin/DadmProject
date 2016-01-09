using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.ECG_Baseline;
using EKG_Project.Modules;
using System.Diagnostics;

namespace EKG_Project.IO
{
    public class ECG_Baseline_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private ECG_Baseline_Data _data;

        public ECG_Baseline_Data Data
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

        public ECG_Baseline_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public ECG_Baseline_Data_Worker(String analysisName)
            : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is ECG_Baseline_Data)
            {
                ECG_Baseline_Data basicData = data as ECG_Baseline_Data;

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

                List<Tuple<string, Vector<double>>> signalsFiltered = basicData.SignalsFiltered;
                foreach (var tuple in signalsFiltered)
                {
                    XmlElement signalFiltered = file.CreateElement(string.Empty, "signalFiltered", string.Empty);
                    module.AppendChild(signalFiltered);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1);
                    lead.AppendChild(leadValue);
                    signalFiltered.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    signalFiltered.AppendChild(samples);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            ECG_Baseline_Data basicData = new ECG_Baseline_Data();
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
                    List<Tuple<string, Vector<double>>> SignalsFiltered = new List<Tuple<string, Vector<double>>>();
                    XmlNodeList signals = module.SelectNodes("signalFiltered");
                    foreach (XmlNode signal in signals)
                    {
                        XmlNode lead = signal["lead"];
                        string readLead = lead.InnerText;

                        XmlNode samples = signal["samples"];
                        string readSamples = samples.InnerText;
                        Vector<double> readDigits = converter.stringToVector(readSamples);

                        Tuple<string, Vector<double>> readSignal = Tuple.Create(readLead, readDigits);
                        SignalsFiltered.Add(readSignal);
                    }
                    basicData.SignalsFiltered = SignalsFiltered;
                }
            }
            this.Data = basicData;
        }


        //public static void Main()
        //{
        //    ECG_Baseline_Data_Worker worker = new ECG_Baseline_Data_Worker();
        //    worker.Load();

        //    foreach (var signal in worker.Data.SignalsFiltered)
        //    {
        //        //Console.WriteLine(String.Format("Signal {0}: {1} \n", signal.Item1, signal.Item2.ToString()));
        //        Debug.WriteLine(String.Format("Signal {0}: {1} \n", signal.Item1, signal.Item2.ToString()));
        //        //Debug.
        //    }

            
        //    //Console.Read();

        //}
    }
}
