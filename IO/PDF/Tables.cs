using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Collections.Generic;
using System.Linq;

namespace EKG_Project.IO
{
  public static class Tables
  {
    public static void DefineTable(Document document, int _rows, int _cols, Dictionary<string, string> _strToStr)
    {
            Table table = new Table();
            table.Borders.Width = 1;

            Column column;
            Row row;
            //Cell cell;

            for (int count = 0; count < _cols; count++)
            {
                column = table.AddColumn(Unit.FromCentimeter(5));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            for (int count = 0; count < _rows; count++)
            {
                row = table.AddRow();
                var item = _strToStr.ElementAt(count);
                row.Cells[0].AddParagraph(item.Key);
                row.Cells[1].AddParagraph(item.Value);
            }

            table.SetEdge(0, 0, _cols, _rows, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }
  }
}
