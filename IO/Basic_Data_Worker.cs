using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class Basic_Data_Worker : IECG_Worker
    {
        public Basic_Data_Worker() { }

        public void Save(ECG_Data data)
        {
            if (data is Basic_Data)
            {
                Basic_Data basicData = data as Basic_Data;

                ECG_Worker ew = new ECG_Worker();
                ew.CreateFile();
                XmlDocument file = ew.InternalXMLFile;
                XmlNode root = file.SelectSingleNode("EKG");

                XmlElement module = file.CreateElement(string.Empty, "module", string.Empty);
                string moduleName = this.GetType().Name;
                moduleName = moduleName.Replace("_Data_Worker", "");
                module.SetAttribute("name", moduleName);
                root.AppendChild(module);

                XmlElement frequency = file.CreateElement(string.Empty, "frequency", string.Empty);
                XmlText frequencyValue = file.CreateTextNode(basicData.Frequency.ToString());
                frequency.AppendChild(frequencyValue);
                module.AppendChild(frequency);

                XmlElement sampleAmount = file.CreateElement(string.Empty, "sampleAmount", string.Empty);
                XmlText sampleAmountValue = file.CreateTextNode(basicData.SampleAmount.ToString());
                sampleAmount.AppendChild(sampleAmountValue);
                module.AppendChild(sampleAmount);

                List<Tuple<string, Vector<double>>> signals = basicData.Signals;
                foreach (var tuple in signals)
                {
                    XmlElement signal = file.CreateElement(string.Empty, "signal", string.Empty);
                    module.AppendChild(signal);

                    XmlElement lead = file.CreateElement(string.Empty, "lead", string.Empty);
                    XmlText leadValue = file.CreateTextNode(tuple.Item1.ToString());
                    lead.AppendChild(leadValue);
                    signal.AppendChild(lead);

                    XmlElement samples = file.CreateElement(string.Empty, "samples", string.Empty);
                    string samplesText = null;
                    foreach (var value in tuple.Item2)
                    {
                        samplesText += value.ToString() + " ";
                    }

                    XmlText samplesValue = file.CreateTextNode(samplesText);
                    samples.AppendChild(samplesValue);
                    signal.AppendChild(samples);

                }

                ew.InternalXMLFile = file;

                string directory = @"C:\temp\";
                string analysisName = "Analysis5";
                string fileName = analysisName + "_Data.xml";
                file.Save(directory + fileName);

            }

        }
        public void Load()
        {

        }

        static void Main()
        {
            
            
        }
    }
}
