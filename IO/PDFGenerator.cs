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
    class PDFGenerator
    {
        public Document document;
        public string filename;
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




        public void GeneratePDF(PDF.StoreDataPDF _data, bool init)
        {
            documentCreator.CreateDocument(_data, init);
        }

        public void SaveDocument()
        {
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "Analysis Report");
            //filename = "PDFexample.pdf";
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filename);
            //Process.Start(filename);
        }

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
