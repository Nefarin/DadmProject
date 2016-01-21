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
    /// <summary>
    /// Class that creates internal XML file
    /// </summary>
    public class ECG_Worker 
    {
        //FIELDS
        /// <summary>
        /// Stores worker
        /// </summary>
        IECG_Worker worker;

        private XmlDocument _internalXMLFile;

        public ECG_Worker() { }

        /// <summary>
        /// Gets or sets internalXMLFile
        /// </summary>
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

        //METHODS
        /// <summary>
        /// Creates internal XML file
        /// </summary>
        public void CreateFile()
        {
            InternalXMLFile = new XmlDocument();

            XmlDeclaration XMLDeclaration = InternalXMLFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement header = InternalXMLFile.DocumentElement;
            InternalXMLFile.InsertBefore(XMLDeclaration, header);

            XmlElement root = InternalXMLFile.CreateElement(string.Empty, "EKG", string.Empty);
            InternalXMLFile.AppendChild(root);

        }
    }
}
