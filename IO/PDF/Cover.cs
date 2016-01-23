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
        public void DefineCover(Document document, PDF.StoreDataPDF _data)
        {
            Paragraph paragraph = section.AddParagraph();
        
            PutLogoInHeader();

            paragraph = section.AddParagraph("Final Report");
            paragraph.Format.Font.Size = 32;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "3cm";
            paragraph.Format.SpaceAfter = "2.5cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            this.InsertCoverContent(paragraph, section, _data);

            paragraph = section.AddParagraph("Date: ");
            paragraph.AddDateField();

            paragraph.Format.Borders.DistanceFromTop = "2cm";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = section.AddParagraph("Created from file: " + _data.Filename);
            paragraph.Format.Alignment = ParagraphAlignment.Center;

        }

        public void AddCoverAnalysisList(Paragraph paragraph, Cell cell, System.Collections.Generic.List<string> _moduleList)
        {
            cell.AddParagraph("Modules included:\n\n");
            cell.Format.Font.Color = Colors.Blue;

            foreach (string element in _moduleList)
            {
                ListInfo listinfo = new ListInfo();
                listinfo.ListType = ListType.NumberList1;
                paragraph = cell.AddParagraph(element);
                paragraph.Format.ListInfo = listinfo;
                paragraph.Format.Font.Color = Colors.Black;
            }
        }

        public void InsertCoverContent(Paragraph paragraph, Section section, PDF.StoreDataPDF _data)
        {

            Table table = new Table();
            
            Column column = table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            this.AddCoverAnalysisList(paragraph, cell, _data.ModuleList);
            cell.Format.Alignment = ParagraphAlignment.Left;
            cell = row.Cells[1];
            this.AddFileInfo(paragraph, cell, _data.AnalisysName);
            cell.Format.Alignment = ParagraphAlignment.Justify;

            section.Add(table);
        }

        private void AddFileInfo(Paragraph paragraph, Cell cell, string _analisysName)
        {
            Basic_Data_Worker basicDataWorker = new Basic_Data_Worker(_analisysName);
            basicDataWorker.Load();

            
            paragraph = cell.AddParagraph("Analisys name:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph = cell.AddParagraph(_analisysName);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph = cell.AddParagraph("Frequency:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph = cell.AddParagraph(basicDataWorker.BasicData.Frequency.ToString() + " Hz");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceAfter = "0.5cm";
            paragraph = cell.AddParagraph("Sample amount:");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Blue;
            paragraph = cell.AddParagraph(basicDataWorker.BasicData.SampleAmount.ToString());
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Color = Colors.Black;

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
