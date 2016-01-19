﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace EKG_Project.IO.PDFModuleClasses
{
    class HRV_DFA_PDF : IPDFModuleClass
    {
        public Document Document { get; set; }
        public Section Section { get; set; }

        public HRV_DFA_PDF(Document document)
        {
            Document = document;
            Section = Document.AddSection();
            Section.AddPageBreak();
        }

        public void FillReportForModule(string _header)
        {
            Paragraph paragraph = Section.AddParagraph(_header);
            paragraph.AddBookmark(_header);
            paragraph.Format.SpaceBefore = "3cm";
            paragraph.Format.Font.Size = 18;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 24;
            paragraph.Format.OutlineLevel = OutlineLevel.Level1;
        }

    }
}
