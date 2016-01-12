﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules.ST_Segment;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class ST_Segment_Data_Worker
    {
        string directory;
        string analysisName;
        private ST_Segment_Data _data;

        public ST_Segment_Data Data
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

        public ST_Segment_Data_Worker()
        {
            IECGPath pathBuilder = new DebugECGPath();
            directory = pathBuilder.getTempPath();
            Data = null;
        }

        public ST_Segment_Data_Worker(String analysisName) : this()
        {
            this.analysisName = analysisName;
        }

        public void Save(ECG_Data data)
        {
            if (data is ST_Segment_Data)
            {
                ST_Segment_Data basicData = data as ST_Segment_Data;

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

                object[] Properties = { basicData.tJs, basicData.tSTs };
                string[] Names = { "tJs", "tSTs" };
                int licznik = 0;

                foreach (var property in Properties)
                {
                    List<long> list = (List<long>)property;

                    XmlElement moduleNode = file.CreateElement(string.Empty, Names[licznik], string.Empty);
                    
                    string samplesText = null;
                    foreach (var value in list)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    moduleNode.AppendChild(samplesValue);

                    module.AppendChild(moduleNode);
                    licznik++;
                }

                object[] sProperties = { basicData.ConcaveCurves, basicData.ConvexCurves, basicData.DecreasingLines, basicData.HorizontalLines, basicData.IncreasingLines };
                string[] sNames = { "ConcaveCurves", "ConvexCurves", "DecreasingLines", "HorizontalLines", "IncreasingLines" };
                int slicznik = 0;

                foreach (var property in sProperties)
                {
                    int value = (int)property;
                    XmlElement moduleNode = file.CreateElement(string.Empty, sNames[slicznik], string.Empty);
                    XmlText textValue = file.CreateTextNode(value.ToString());

                    moduleNode.AppendChild(textValue);
                    module.AppendChild(moduleNode);

                    slicznik++;

                }

                ew.InternalXMLFile = file;

                file.Save(System.IO.Path.Combine(directory, fileName));

            }

        }

        public void Load()
        {
            ST_Segment_Data basicData = new ST_Segment_Data();

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
                    XmlNode node = module["tJs"];
                    string readNode = node.InnerText;
                    List<long> readList = stringToList(readNode);
                    basicData.tJs = readList;

                    XmlNode node1 = module["tSTs"];
                    string readNode1 = node1.InnerText;
                    List<long> readList1 = stringToList(readNode1);
                    basicData.tSTs = readList1;

                    XmlNode sNode = module["ConcaveCurves"];
                    basicData.ConcaveCurves = Convert.ToInt32(sNode.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode1 = module["ConvexCurves"];
                    basicData.ConvexCurves = Convert.ToInt32(sNode1.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode2 = module["DecreasingLines"];
                    basicData.DecreasingLines = Convert.ToInt32(sNode2.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode3 = module["HorizontalLines"];
                    basicData.HorizontalLines = Convert.ToInt32(sNode3.InnerText, new System.Globalization.NumberFormatInfo());

                    XmlNode sNode4 = module["IncreasingLines"];
                    basicData.IncreasingLines = Convert.ToInt32(sNode4.InnerText, new System.Globalization.NumberFormatInfo());

                }
            }
            this.Data = basicData;
        }

        public static List<long> stringToList(string input)
        {
            long[] digits = input
                              .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(digit => Convert.ToInt64(digit, new System.Globalization.NumberFormatInfo()))
                              .ToArray();
            List<long> list = new List<long>();
            for (int i = 0; i < digits.Length; i++)
                list.Add(digits[i]);
            return list;
        }
    }
}
