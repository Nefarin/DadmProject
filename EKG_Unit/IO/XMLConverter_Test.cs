using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EKG_Project.IO;

namespace EKG_Unit.IO
{
    [TestClass]
    public class XMLConverter_Test
    {
        public void createMockXML()
        {
            XmlDocument XMLFile = new XmlDocument();

            XmlDeclaration XMLDeclaration = XMLFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement header = XMLFile.DocumentElement;
            XMLFile.InsertBefore(XMLDeclaration, header);

            XmlElement root = XMLFile.CreateElement(string.Empty, "AnnotatedECG", string.Empty);
            root.SetAttribute("xmlns", "urn:hl7-org:v3");
            XMLFile.AppendChild(root);

            XmlElement series = XMLFile.CreateElement(string.Empty, "series", string.Empty);
            root.AppendChild(series);

            XmlElement component = XMLFile.CreateElement(string.Empty, "component", string.Empty);
            series.AppendChild(component);

            XmlElement sequenceSet = XMLFile.CreateElement(string.Empty, "sequenceSet", string.Empty);
            component.AppendChild(sequenceSet);

            XmlElement component2 = XMLFile.CreateElement(string.Empty, "component", string.Empty);
            sequenceSet.AppendChild(component2);

            XmlElement sequence = XMLFile.CreateElement(string.Empty, "sequence", string.Empty);
            component2.AppendChild(sequence);

            XmlElement code = XMLFile.CreateElement(string.Empty, "code", string.Empty);
            code.SetAttribute("code", "MDC_ECG_LEAD_I");
            sequence.AppendChild(code);

            XmlElement value = XMLFile.CreateElement(string.Empty, "value", string.Empty);
            value.SetAttribute("xsi:type", "SLIST_PQ");
            sequence.AppendChild(value);

            XmlElement origin = XMLFile.CreateElement(string.Empty, "origin", string.Empty);
            origin.SetAttribute("value", "0");
            origin.SetAttribute("unit", "uV");
            sequence.AppendChild(origin);

            XmlElement scale = XMLFile.CreateElement(string.Empty, "scale", string.Empty);
            scale.SetAttribute("value", "5");
            scale.SetAttribute("unit", "uV");
            sequence.AppendChild(scale);

            XmlElement digits = XMLFile.CreateElement(string.Empty, "digits", string.Empty);
            XmlText digitsValue = XMLFile.CreateTextNode("-6 -6 -7 -7 -8 -8 -8 -8 -8 -8");
            digits.AppendChild(digitsValue);
            sequence.AppendChild(digits);

            XMLFile.Save(@"C:\temp\mock.xml");
        }

        [TestMethod]
        public void loadXMLFileTest()
        {
            XMLConverter xml = new XMLConverter();
            XMLConverter_Test test = new XMLConverter_Test();
            test.createMockXML();

            XmlDocument file = new XmlDocument();
            file.Load(@"C:\temp\mock.xml");

            XmlNamespaceManager manager = new XmlNamespaceManager(file.NameTable);
            manager.AddNamespace("hl7", "urn:hl7-org:v3");

            XmlNodeList expected = file.SelectNodes("//hl7:series/hl7:component/hl7:sequenceSet/hl7:component/hl7:sequence", manager);

            xml.loadXMLFile(@"C:\temp\2.xml");

            PrivateObject obj = new PrivateObject(xml);

            XmlNodeList actual = (XmlNodeList) obj.GetField("sequences");

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void getOriginTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");
            
            double expected = 0.0;

            double actual = xml.getOrigin();

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void getScaleTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");

            double expected = 5.0;

            double actual = xml.getScale();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void getFrequencyTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");

            uint expected = (uint) 1000;

            uint actual = xml.getFrequency();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void getPhysSignalTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");
            xml.getOrigin();
            xml.getScale();

            double[] expected = new double[] { 5, 5, 5, 5, 5 };
            double[] actual = xml.getPhysSignal(new double[] { 1000, 1000, 1000, 1000, 1000 });

            CollectionAssert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void getNumberOfSamplesTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");

            uint actual = xml.getNumberOfSamples("I");
            uint expected = 13696;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void getLeadsTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");

            List<string> expected = new List<string>() { "I", "II", "III", "aVR", "aVL", "aVF", "V1", "V2", "V3", "V4", "V5", "V6" };
            List<string> actual = xml.getLeads();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void getSignalTest()
        {
            XMLConverter xml = new XMLConverter();
            xml.loadXMLFile(@"C:\temp\2.xml");
            PrivateObject obj = new PrivateObject(xml);
            obj.SetField("leads", new List<string>() { "I", "II", "III", "aVR", "aVL", "aVF", "V1", "V2", "V3", "V4", "V5", "V6" });

            double[] expectedArray = { -0.03, -0.03, -0.035, -0.035, -0.04, -0.04, -0.04, -0.04, -0.04, -0.04 };
            Vector<double> expectedVector = Vector<double>.Build.Dense(expectedArray.Length);
            expectedVector.SetValues(expectedArray);

            Vector<double> actualVector = xml.getSignal("I", 0, 10);

            Assert.AreEqual(expectedVector, actualVector);
        }

        [TestMethod]
        public void stringToArrayTest()
        {
            XMLConverter xml = new XMLConverter();
            double[] expected = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            string text = "1 2 3 4 5 6 7 8 9 10";
            double[] actual = xml.stringToArray(text);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
