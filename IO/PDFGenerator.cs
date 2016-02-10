using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.RtfRendering;
using MigraDoc.DocumentObjectModel.Shapes;

namespace EKG_Project.IO
{
    /// <summary>
    /// Class that triggers PDF report
    /// </summary> 
    class PDFGenerator
    {
        //FIELDS
        /// <summary>
        /// Stores Pdf documents
        /// </summary>
        public Document document;

        //FIELDS
        /// <summary>
        /// Stores Pdf filename
        /// </summary>
        public string filename;

        //FIELDS
        /// <summary>
        /// Stores Analysis name
        /// </summary>
        public string analysisName;

        //FIELDS
        /// <summary>
        /// Stores documents Creator, which define its content
        /// </summary>
        Documents documentCreator;

        //public string Filename { get; set; }

        public PDFGenerator()
        {
            Documents documentCreator = new Documents();
            document = documentCreator.Document;
            filename = "PDF.pdf";
            //this.GeneratePDF();
        }
        public PDFGenerator(string _filename)
        {
            documentCreator = new Documents();
            document = documentCreator.Document;
            filename = _filename;
            //this.GeneratePDF();
        }



        //METHODS
        /// <summary>
        /// Trigger pdf generation
        /// </summary>
        /// <param name="_data">PDFStore data</param>
        /// <param name="init">true if first called</param>
        public void GeneratePDF(PDF.StoreDataPDF _data, bool init)
        {
            analysisName = _data.AnalisysName;
            System.Console.WriteLine("GEN" + analysisName);
            documentCreator.CreateDocument(_data, init);
        }

        //METHODS
        /// <summary>
        /// Save pdf
        /// </summary>
        public void SaveDocument()
        {
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "Analysis Report");
            //filename = "PDFexample.pdf";
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filename);
            mergePDFFiles();
            //Process.Start(filename);
        }

        //METHODS
        /// <summary>
        /// Add plot screenshots at the end of document
        /// </summary>
        public void mergePDFFiles()
        {
            string automaticDirPath = System.IO.Directory.GetCurrentDirectory();
            automaticDirPath = automaticDirPath.Remove(automaticDirPath.IndexOf("bin") + 4) + @analysisName;

            if (System.IO.Directory.Exists(automaticDirPath))
            {
                string[] filePaths = Directory.GetFiles(automaticDirPath);

                foreach (string element in filePaths)
                {
                    PDF.AddSvgAsPage addPage = new PDF.AddSvgAsPage(filename, element);
                }
            }
        }

        //METHODS
        /// <summary>
        /// Starts default pdf reader
        /// </summary>
        public void ProcessStart()
        {
            Process.Start(filename);
        }

        /*static void Main(string[] args)
        {

            PDFGenerator pdf = new PDFGenerator();
            //pdf.Filename = "PDFexample.pdf";
            Process.Start(pdf.filename);
        }*/
    }
}
