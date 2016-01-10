using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Flutter;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class Flutter_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Flutter_Data _data;

        public Flutter_Data Data
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

        public Flutter_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Flutter_Data_Worker(String analysisName)  : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Flutter_Data)
            {
                Flutter_Data basicData = data as Flutter_Data;

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

                List<Tuple<int, int>> flutterAnnotations = basicData.FlutterAnnotations;
                foreach (var tuple in flutterAnnotations)
                {
                    XmlElement FlutterAnnotations = file.CreateElement(string.Empty, "FlutterAnnotations", string.Empty);
                    module.AppendChild(FlutterAnnotations);

                    XmlElement onset = file.CreateElement(string.Empty, "onset", string.Empty);
                    XmlText onsetValue = file.CreateTextNode(tuple.Item1.ToString());
                    onset.AppendChild(onsetValue);
                    FlutterAnnotations.AppendChild(onset);

                    XmlElement end = file.CreateElement(string.Empty, "end", string.Empty);
                    XmlText endValue = file.CreateTextNode(tuple.Item2.ToString());
                    end.AppendChild(endValue);
                    FlutterAnnotations.AppendChild(end);
                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            Flutter_Data basicData = new Flutter_Data();

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
                    List<Tuple<int, int>> FlutterAnnotations = new List<Tuple<int, int>>();
                    XmlNodeList flutterAnnotations = module.SelectNodes("FlutterAnnotations");
                    foreach (XmlNode node in flutterAnnotations)
                    {
                        XmlNode onset = node["onset"];
                        string readOnset = onset.InnerText;
                        int convertedOnset = Convert.ToInt32(readOnset, new System.Globalization.NumberFormatInfo());

                        XmlNode end = node["end"];
                        string readEnd = onset.InnerText;
                        int convertedEnd = Convert.ToInt32(readEnd, new System.Globalization.NumberFormatInfo());

                        Tuple<int, int> readFlutterAnnotations = Tuple.Create(convertedOnset, convertedEnd);
                        FlutterAnnotations.Add(readFlutterAnnotations);
                    }
                    basicData.FlutterAnnotations = FlutterAnnotations;

                }
            }
            this.Data = basicData;
        }
    }
}
