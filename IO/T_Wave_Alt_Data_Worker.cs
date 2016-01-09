using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.T_Wave_Alt;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class T_Wave_Alt_Data_Worker
    {
        string directory;
        private T_Wave_Alt_Data _data;

        public T_Wave_Alt_Data Data
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

        public T_Wave_Alt_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public T_Wave_Alt_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is T_Wave_Alt_Data)
            {
                T_Wave_Alt_Data basicData = data as T_Wave_Alt_Data;

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

                int[] alternansIndexArray = basicData.AlternansIndexArray;

                XmlElement AlternansIndexArrayNode = file.CreateElement(string.Empty, "AlternansIndexArray", string.Empty);
                string samplesText = null;
                foreach (var value in alternansIndexArray)
                {
                    samplesText += value.ToString() + " ";
                }

                XmlText samplesValue = file.CreateTextNode(samplesText);
                AlternansIndexArrayNode.AppendChild(samplesValue);
                module.AppendChild(AlternansIndexArrayNode);

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        public void Load()
        {
            T_Wave_Alt_Data basicData = new T_Wave_Alt_Data();

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
                    XmlNode node = module["AlternansIndexArray"];
                    string readNode = node.InnerText;
                    int[] convertedNode = readNode
                                               .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(point => int.Parse(point))
                                               .ToArray();
                    basicData.AlternansIndexArray = convertedNode;
                }
            }
            this.Data = basicData;
        }
    }
}
