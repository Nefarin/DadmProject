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
        private string filename;

        //public string Filename { get; set; }

        public PDFGenerator()
        {
            document = Documents.CreateDocument();
            filename = "PDFexample.pdf";
            this.GeneratePDF();
        }
        public PDFGenerator(string _filename)
        {
            document = Documents.CreateDocument();
            filename = _filename;
            this.GeneratePDF();
        }




        public void GeneratePDF()
        {
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "Analysis Report");
            filename = "PDFexample.pdf";
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filename);
        }
           
        static void Main(string[] args)
        {

            PDFGenerator pdf = new PDFGenerator();
            //pdf.Filename = "PDFexample.pdf";
            Process.Start(pdf.filename);
        }
    }
}
