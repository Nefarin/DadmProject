using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MigraDoc.DocumentObjectModel;
//using MigraDoc.DocumentObjectModel.Shapes;
//using MigraDoc.DocumentObjectModel.Tables;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;



namespace EKG_Project.IO.PDF
{
    class AddSvgAsPage
    {
        public AddSvgAsPage(string _filenameSource, string _filenameTarget)
        {
            // Get some file names
            string[] files = { _filenameSource, _filenameTarget };

            // Open the output document
            PdfDocument outputDocument = new PdfDocument();

            // Iterate files
            foreach (string file in files)
            {
                // Open the document to import pages from it.
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            string filename = _filenameSource;
            outputDocument.Save(filename);
        }
    }


}
