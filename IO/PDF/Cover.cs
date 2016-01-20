using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

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
        public void DefineCover(Document document, System.Collections.Generic.List<string> _moduleList)
        {
            Paragraph paragraph = section.AddParagraph();
            //paragraph.Format.SpaceAfter = "3cm";
            PutLogoInHeader();
            paragraph = section.AddParagraph("Final Report");
            paragraph.Format.Font.Size = 32;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.SpaceAfter = "2cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            //Cover.AddCoverAnalysisList(paragraph, section);
            Cover.InsertCoverContent(paragraph, section, _moduleList);

            //Image image = document.LastSection.AddImage("cover.png");
            //Image image = section.AddImage("cover.png");
            //image.Left = "8.5cm";

            paragraph = section.AddParagraph("Date: ");
            paragraph.AddDateField();
            //paragraph.Format.LeftIndent = "5cm";
            paragraph.Format.Borders.DistanceFromTop = "2cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph = section.AddParagraph("Created from file: filename there");
            paragraph.Format.Alignment = ParagraphAlignment.Center;

        }

        public static void AddCoverAnalysisList(Paragraph paragraph, Cell cell, System.Collections.Generic.List<string> _moduleList)
        {
            cell.AddParagraph("Modules included:\n\n");
            
            //string[] items = " Analysis 1 | Analysis 2 | Analysis 3 | Analysis 4 | Analysis 5 | Analysis ... | Analysis n".Split('|');

            foreach(string element in _moduleList)
            {
                ListInfo listinfo = new ListInfo();
                //listinfo.ContinuePreviousList = idx > 0;
                listinfo.ListType = ListType.NumberList1;
                paragraph = cell.AddParagraph(element);
                System.Console.WriteLine(element);
                //paragraph.Style = "MyBulletList";
                paragraph.Format.ListInfo = listinfo;
            }
        }

        public static void InsertCoverContent(Paragraph paragraph, Section section, System.Collections.Generic.List<string> _moduleList)
        {

            Table table = new Table();
            
            Column column = table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            Cover.AddCoverAnalysisList(paragraph, cell, _moduleList);
            cell.Format.Alignment = ParagraphAlignment.Left;
            cell = row.Cells[1];
            //Image image = cell.AddImage("cover.png");

            //row = table.AddRow();
            //cell = row.Cells[1];
            //cell.AddParagraph("Date: ");
            //cell.AddParagraph(paragraph.AddDateField().ToString());
            
            //paragraph.AddDateField();
            //cell.AddParagraph("Data 2");

            //row = table.AddRow();
            //cell = row.Cells[0];
            //cell.AddParagraph("Param 2");
            //cell = row.Cells[1];
            //cell.AddParagraph("Data 2");

            //table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            section.Add(table);
        }

        private void PutLogoInHeader()
        {
            Image image = this.section.Headers.Primary.AddImage("cover.png");
            image.Height = "4cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;
        }
    }
}
