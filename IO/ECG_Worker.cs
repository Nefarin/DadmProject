using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using EKG_Project.Modules;

namespace EKG_Project.IO
{
    public class ECG_Worker 
    {
        IECG_Worker worker;
        private XmlDocument _internalXMLFile;

        public ECG_Worker() { }

        public XmlDocument InternalXMLFile
        {
            get
            {
                return _internalXMLFile;
            }
            set
            {
                _internalXMLFile = value;
            }
        }

        public void CreateFile()
        {
            InternalXMLFile = new XmlDocument();

            XmlDeclaration XMLDeclaration = InternalXMLFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement header = InternalXMLFile.DocumentElement;
            InternalXMLFile.InsertBefore(XMLDeclaration, header);

            XmlElement root = InternalXMLFile.CreateElement(string.Empty, "EKG", string.Empty);
            InternalXMLFile.AppendChild(root);

        }


        static void Main()
        {
            Console.Read();
        }
    }
}
