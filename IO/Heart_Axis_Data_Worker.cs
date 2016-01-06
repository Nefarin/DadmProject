using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Heart_Axis;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    class Heart_Axis_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Heart_Axis_Data _data;

        public Heart_Axis_Data Data
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

        public Heart_Axis_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Heart_Axis_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Heart_Axis_Data)
            {
                Heart_Axis_Data basicData = data as Heart_Axis_Data;

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

                double HeartAxis = basicData.HeartAxis;
                XmlElement heartAxis = file.CreateElement(string.Empty, "HeartAxis", string.Empty);
                XmlText heartAxisValue = file.CreateTextNode(HeartAxis.ToString());
                heartAxis.AppendChild(heartAxisValue);
                module.AppendChild(heartAxis);

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            Heart_Axis_Data basicData = new Heart_Axis_Data();

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
                    XmlNode heartAxis = module["HeartAxis"];
                    basicData.HeartAxis = Convert.ToDouble(heartAxis.InnerText, new System.Globalization.NumberFormatInfo());
                }
            }
            this.Data = basicData;
        }
    }
}
