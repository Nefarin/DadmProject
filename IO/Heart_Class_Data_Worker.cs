using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.Heart_Class;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class Heart_Class_Data_Worker
    {
        string directory;
        string analysisName = "Analysis6";
        private Heart_Class_Data _data;

        public Heart_Class_Data Data
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

        public Heart_Class_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public Heart_Class_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is Heart_Class_Data)
            {
                Heart_Class_Data basicData = data as Heart_Class_Data;

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

                List<Tuple<int, int>> list = basicData.QrsComplexLabel;
                foreach (var tuple in list)
                {
                    XmlElement moduleNode = file.CreateElement(string.Empty, "QrsComplexLabel", string.Empty);
                    module.AppendChild(moduleNode);

                    XmlElement item1 = file.CreateElement(string.Empty, "item1", string.Empty);
                    XmlText item1Value = file.CreateTextNode(tuple.Item1.ToString());
                    item1.AppendChild(item1Value);
                    moduleNode.AppendChild(item1);

                    XmlElement item2 = file.CreateElement(string.Empty, "item2", string.Empty);
                    XmlText item2Value = file.CreateTextNode(tuple.Item2.ToString());
                    item2.AppendChild(item2Value);
                    moduleNode.AppendChild(item2);
                }

                uint TotalNumberOfQrsComplex = basicData.TotalNumberOfQrsComplex;
                XmlElement totalNumberOfQrsComplex = file.CreateElement(string.Empty, "TotalNumberOfQrsComplex", string.Empty);
                XmlText value = file.CreateTextNode(TotalNumberOfQrsComplex.ToString());
                totalNumberOfQrsComplex.AppendChild(value);
                module.AppendChild(totalNumberOfQrsComplex);


                uint NumberOfClass = basicData.NumberOfClass;
                XmlElement numberOfClass = file.CreateElement(string.Empty, "NumberOfClass", string.Empty);
                XmlText numberOfClassValue = file.CreateTextNode(NumberOfClass.ToString());
                numberOfClass.AppendChild(numberOfClassValue);
                module.AppendChild(numberOfClass);

                double PercentOfNormalComplex = basicData.PercentOfNormalComplex;
                XmlElement percentOfNormalComplex = file.CreateElement(string.Empty, "PercentOfNormalComplex", string.Empty);
                XmlText percentOfNormalComplexValue = file.CreateTextNode(PercentOfNormalComplex.ToString());
                percentOfNormalComplex.AppendChild(percentOfNormalComplexValue);
                module.AppendChild(percentOfNormalComplex);

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }
        }
        public void Load()
        {
            Heart_Class_Data basicData = new Heart_Class_Data();

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
                    List<Tuple<int, int>> list = new List<Tuple<int, int>>();
                    XmlNodeList nodes = module.SelectNodes("QrsComplexLabel");
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode item1 = node["item1"];
                        string readItem1 = item1.InnerText;
                        int convertedItem1 = Convert.ToInt32(readItem1, new System.Globalization.NumberFormatInfo());

                        XmlNode item2 = node["item2"];
                        string readItem2 = item1.InnerText;
                        int convertedItem2 = Convert.ToInt32(readItem2, new System.Globalization.NumberFormatInfo());

                        Tuple<int, int> read = Tuple.Create(convertedItem1, convertedItem2);
                        list.Add(read);
                    }
                    basicData.QrsComplexLabel = list;

                    XmlNode totalNumberOfQrsComplex = module["TotalNumberOfQrsComplex"];
                    basicData.TotalNumberOfQrsComplex = Convert.ToUInt32(totalNumberOfQrsComplex.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode numberOfClass = module["NumberOfClass"];
                    basicData.NumberOfClass = Convert.ToUInt32(numberOfClass.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode percentOfNormalComplex = module["PercentOfNormalComplex"];
                    basicData.PercentOfNormalComplex = Convert.ToUInt32(percentOfNormalComplex.InnerText, new System.Globalization.NumberFormatInfo());
                    
                }
            }
            this.Data = basicData;
        }

    }
}
