using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace EKG_Project.IO
{
  public class Tables
  {
    public static void DefineTables(Document document)
    {
        Paragraph paragraph = document.LastSection.AddParagraph("Table Overview", "Heading2");
        paragraph.AddBookmark("Tables");
        paragraph.Format.SpaceAfter = "2cm";
        DemonstrateSimpleTable(document);
        

        Paragraph paragraph2 = document.LastSection.AddParagraph("Chart Overview", "Heading2");
        paragraph2.Format.SpaceBefore = "3cm";
        Image image = document.LastSection.AddImage("wykres.png");
        image.Width = "15cm";

  }

    public static void DemonstrateSimpleTable(Document document)
    {
      document.LastSection.AddParagraph("Simple Table", "Heading2");

      Table table = new Table();
      table.Borders.Width = 0.75;

      Column column = table.AddColumn(Unit.FromCentimeter(2));
      column.Format.Alignment = ParagraphAlignment.Center;

      table.AddColumn(Unit.FromCentimeter(5));

      Row row = table.AddRow();
      row.Shading.Color = Colors.PaleGoldenrod;
      Cell cell = row.Cells[0];
      cell.AddParagraph("Items");
      cell = row.Cells[1];
      cell.AddParagraph("Description");

      row = table.AddRow();
      cell = row.Cells[0];
      cell.AddParagraph("Param 1");
      cell = row.Cells[1];
      cell.AddParagraph("Data 2");

      row = table.AddRow();
      cell = row.Cells[0];
      cell.AddParagraph("Param 2");
      cell = row.Cells[1];
      cell.AddParagraph("Data 2");

      table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

      document.LastSection.Add(table);
    }

  }
}
