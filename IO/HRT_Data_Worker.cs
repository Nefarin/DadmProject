using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.HRT;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that saves and loads HRT_Data from internal XML file
    /// </summary>
    public class HRT_Data_Worker
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
        private HRT_Data _data;

        /// <summary>
        /// Gets or sets HRT_Data
        /// </summary>
        public HRT_Data Data
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

        public HRT_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public HRT_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        //METHODS
        /// <summary>
        /// Saves HRT_Data
        /// </summary>
        /// <param name="data">HRT_Data</param>
        public void Save(ECG_Data data)
        {
            if (data is HRT_Data)
            {
                HRT_Data basicData = data as HRT_Data;

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

                object[] Properties = { basicData._TurbulenceOnset, basicData._TurbulenceSlope, basicData._VPCtachogram};
                string[] Names = { "TurbulenceOnset", "TurbulenceSlope", "VPCtachogram" };
                int licznik = 0;

                foreach (var property in Properties)
                {
                    Vector<double> vector = (Vector<double>) property;
                    XmlElement moduleNode = file.CreateElement(string.Empty, Names[licznik], string.Empty);
                    
                    StringBuilder builder = new StringBuilder();
                    foreach (var value in vector)
                    {
                        builder.Append(value.ToString());
                        builder.Append(" ");
                    }
                    string samplesText = builder.ToString();

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    moduleNode.AppendChild(samplesValue);
                    module.AppendChild(moduleNode);

                    licznik++;
                }

                object[] aProperties = { basicData._TurbulenceOnsetMean, basicData._TurbulenceSlopeMean};
                string[] aNames = { "TurbulenceOnsetMean", "TurbulenceSlopeMean" };
                int alicznik = 0;

                foreach (var property in aProperties)
                {
                    double[] array = (double[])property;
                    XmlElement moduleNode = file.CreateElement(string.Empty, aNames[alicznik], string.Empty);

                    StringBuilder builder = new StringBuilder();
                    foreach (var value in array)
                    {
                        builder.Append(value.ToString());
                        builder.Append(" ");
                    }
                    string samplesText = builder.ToString();

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    moduleNode.AppendChild(samplesValue);
                    module.AppendChild(moduleNode);

                    alicznik++;
                }

                XmlElement VPCcount = file.CreateElement(string.Empty, "VPCcount", string.Empty);
                XmlText VPCcountValue = file.CreateTextNode(basicData._VPCcount.ToString());
                VPCcount.AppendChild(VPCcountValue);
                module.AppendChild(VPCcount);

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }

        /// <summary>
        /// Loads and sets HRT_Data
        /// </summary>
        public void Load()
        {
            HRT_Data basicData = new HRT_Data();
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
                    XmlNode node = module["TurbulenceOnset"];
                    string readNode = node.InnerText;
                    Vector<double> readDigits = converter.stringToVector(readNode);
                    basicData._TurbulenceOnset = readDigits;

                    XmlNode node1 = module["TurbulenceSlope"];
                    string readNode1 = node1.InnerText;
                    Vector<double> readDigits1 = converter.stringToVector(readNode1);
                    basicData._TurbulenceSlope = readDigits1;

                    XmlNode node2 = module["VPCtachogram"];
                    string readNode2 = node2.InnerText;
                    Vector<double> readDigits2 = converter.stringToVector(readNode2);
                    basicData._VPCtachogram = readDigits2;

                    XmlNode node3 = module["TurbulenceOnsetMean"];
                    string readNode3 = node3.InnerText;
                    double[] convertedNode3 = readNode3
                                               .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(point => double.Parse(point))
                                               .ToArray();
                    basicData._TurbulenceOnsetMean = convertedNode3;

                    XmlNode node4 = module["TurbulenceSlopeMean"];
                    string readNode4 = node4.InnerText;
                    double[] convertedNode4 = readNode3
                                               .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(point => double.Parse(point))
                                               .ToArray();
                    basicData._TurbulenceSlopeMean = convertedNode4;

                    XmlNode node5 = module["VPCcount"];
                    basicData._VPCcount = Convert.ToInt32(node5.InnerText, new System.Globalization.NumberFormatInfo());
                
                }
            }
            this.Data = basicData;
        }
    }
}
