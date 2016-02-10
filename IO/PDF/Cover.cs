using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EKG_Project.IO
{
    public class Cover
    {
        /// <summary>
        /// Defines the cover page.
        /// </summary>

        private Section section;

        public Cover (Document _doc)
        {
            section = _doc.AddSection();
        }
        public void DefineCover(Document document, PDF.StoreDataPDF _data)
        {
            Paragraph paragraph = section.AddParagraph();
        
            PutLogoInHeader();
            this.section.Footers.Primary.AddParagraph().AddPageField();

            paragraph = section.AddParagraph("Final Report");
            paragraph.Format.Font.Size = 32;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "3cm";
            paragraph.Format.SpaceAfter = "2.5cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            this.InsertCoverContent(paragraph, section, _data);

            Table table = new Table();

            table.Borders.Width = 1;

            Column column = table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            Cell cell1 = row.Cells[0];
            cell1.AddParagraph("DateTime: ");
            cell1.Format.Alignment = ParagraphAlignment.Center;
            cell1.Style = "Heading2";
            cell1 = row.Cells[1];
            cell1.AddParagraph().AddDateField();
            cell1.Format.Alignment = ParagraphAlignment.Center;
            cell1.Style = "Heading2";

            row = table.AddRow();
            Cell cell2 = row.Cells[0];
            cell2.AddParagraph("Created from file: ");
            cell2.Format.Alignment = ParagraphAlignment.Center;
            cell2.Style = "Heading2";
            cell2 = row.Cells[1];
            string filename = Path.GetFileName(_data.Filename);
            cell2.AddParagraph(filename);
            cell2.Format.Alignment = ParagraphAlignment.Center;
            cell2.Style = "Heading2";

            section.Add(table);
        }

        public void AddCoverAnalysisList(Paragraph paragraph, Cell cell, System.Collections.Generic.List<string> _moduleList)
        {
            cell.AddParagraph("Modules included:\n\n");
            cell.Style = "Heading2";
            cell.Format.Font.Color = Colors.Blue;
            paragraph.Style = "Heading1";

            foreach (string element in _moduleList)
            {
                ListInfo listinfo = new ListInfo();
                listinfo.ListType = ListType.NumberList1;
                paragraph = cell.AddParagraph(element);
                paragraph.Format.ListInfo = listinfo;
                paragraph.Format.Font.Color = Colors.Black;
                paragraph.Style = "Heading2";
            }
        }

        public void InsertCoverContent(Paragraph paragraph, Section section, PDF.StoreDataPDF _data)
        {

            Table table = new Table();

            table.Borders.Width = 1;
            table.Shading.Color = Colors.WhiteSmoke;

            Column column = table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(8));

            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            this.AddCoverAnalysisList(paragraph, cell, _data.ModuleList);
            cell.Format.Alignment = ParagraphAlignment.Left;
            cell = row.Cells[1];
            this.AddFileInfo(paragraph, cell, _data.AnalisysName);
            cell.Format.Alignment = ParagraphAlignment.Justify;
            //cell.Format.Shading.Color = Colors.LightBlue;

            section.Add(table);
        }

        private void AddFileInfo(Paragraph paragraph, Cell cell, string _analisysName)
        {
            


            Basic_New_Data_Worker basicDataWorker = new Basic_New_Data_Worker(_analisysName);

            List<string> leads = basicDataWorker.LoadLeads();
            string Allleads = "";

            foreach (var element in leads )
            {
                Allleads += element + " ";
            }


            paragraph = cell.AddParagraph("Analisys name:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph(_analisysName);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph("Frequency:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph(basicDataWorker.LoadAttribute(Basic_Attributes.Frequency).ToString() + " Hz");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph("Sample amount:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph(basicDataWorker.getNumberOfSamples(leads.ElementAt(0)).ToString());
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph("Leads:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph.Style = "Heading2";
            paragraph = cell.AddParagraph(Allleads);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Style = "Heading2";

        }
        private void PutLogoInHeader()
        {
            DebugECGPath path = new DebugECGPath();          

            Image image = this.section.Headers.Primary.AddImage(path.getResourcesPath() + "\\logo.png");

            image.Height = "4cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;
            image.Section.PageSetup.TopMargin = Unit.FromCentimeter(6.0);
        }
    }
}
