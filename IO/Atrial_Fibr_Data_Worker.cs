using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Atrial_Fibr;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class Atrial_Fibr_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Atrial_Fibr_Data _data;

        public Atrial_Fibr_Data Data
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

        public Atrial_Fibr_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Atrial_Fibr_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Atrial_Fibr_Data)
            {
                Atrial_Fibr_Data basicData = data as Atrial_Fibr_Data;

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

                List<Tuple<bool, int[], string, string>> afDetection = basicData.AfDetection;
                foreach (var tuple in afDetection)
                {
                    XmlElement detectedAF = file.CreateElement(string.Empty, "detectedAF", string.Empty);
                    module.AppendChild(detectedAF);

                    XmlElement detected = file.CreateElement(string.Empty, "detected", string.Empty);
                    XmlText detectedValue = file.CreateTextNode(tuple.Item1.ToString());
                    detected.AppendChild(detectedValue);
                    detectedAF.AppendChild(detected);

                    XmlElement detectedPoints = file.CreateElement(string.Empty, "detectedPoints", string.Empty);
                    string detectedPointsText = null;
                    foreach (var value in tuple.Item2)
                    {
                        detectedPointsText += value.ToString() + " ";
                    }

                    XmlText detectedPointsValue = file.CreateTextNode(detectedPointsText);
                    detectedPoints.AppendChild(detectedPointsValue);
                    detectedAF.AppendChild(detectedPoints);

                    XmlElement detectedS = file.CreateElement(string.Empty, "detectedS", string.Empty);
                    XmlText detectedSValue = file.CreateTextNode(tuple.Item3);
                    detectedS.AppendChild(detectedSValue);
                    detectedAF.AppendChild(detectedS);

                    XmlElement timeofAF = file.CreateElement(string.Empty, "timeofAF", string.Empty);
                    XmlText timeofAFValue = file.CreateTextNode(tuple.Item4);
                    timeofAF.AppendChild(timeofAFValue);
                    detectedAF.AppendChild(timeofAF);

                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }

        }

        public void Load()
        {
            Atrial_Fibr_Data basicData = new Atrial_Fibr_Data();

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
                    List<Tuple<bool, int[], string, string>> AfDetections = new List<Tuple<bool, int[], string, string>>();
                    XmlNodeList detectedAFs = module.SelectNodes("detectedAF");
                    foreach (XmlNode detectedAF in detectedAFs)
                    {
                        XmlNode detected = detectedAF["detected"];
                        string readDetected = detected.InnerText;
                        bool convertedDetected = Convert.ToBoolean(readDetected);

                        XmlNode detectedPoints = detectedAF["detectedPoints"];
                        string readDetectedPoints = detectedPoints.InnerText;
                        int[] convertedDetectedPoints = readDetectedPoints
                                                   .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(point => int.Parse(point))
                                                   .ToArray();

                        XmlNode detectedS = detectedAF["detectedS"];
                        string readDetectedS = detectedS.InnerText;

                        XmlNode timeofAF = detectedAF["timeofAF"];
                        string readTimeofAF = timeofAF.InnerText;

                        Tuple<bool, int[], string, string> readDetectedAF = Tuple.Create(convertedDetected, convertedDetectedPoints, readDetectedS, readTimeofAF);
                        AfDetections.Add(readDetectedAF);
                    }
                    basicData.AfDetection = AfDetections;
                }
            }
            this.Data = basicData;
        }
    }
}
