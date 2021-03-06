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
    class QT_DISP_PDF : IPDFModuleClass
    {
        public Document Document { get; set; }
        public Section Section { get; set; }
        public string AnalisysName { get; set; }
        QT_Disp_Data_Worker _qt_Disp_Data_Worker;

        public QT_DISP_PDF(Document document, string _analisysName)
        {
            Document = document;
            Section = Document.AddSection();
            Section.AddPageBreak();
            AnalisysName = _analisysName;
            _qt_Disp_Data_Worker = new QT_Disp_Data_Worker(AnalisysName);

        }

        public void FillReportForModule(string _header, Dictionary<String, String> _statsDictionary)
        {
            Paragraph paragraph = Section.AddParagraph(_header);
            paragraph.AddBookmark(_header);
            paragraph.Format.Font.Size = 18;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 24;
            paragraph.Format.OutlineLevel = OutlineLevel.Level1;

            InsertStatisticsTable(_statsDictionary);
        }

        public void InsertStatisticsTable(Dictionary<string, string> _strToStr)
        {
            int cols = 3;
            int rows = _strToStr.Count;
            Tables.DefineTable(Document, rows, cols, _strToStr);
        }
    }
    
}
