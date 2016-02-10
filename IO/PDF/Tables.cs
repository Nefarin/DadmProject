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

            table.Borders.Width = 2;

            Column column;
            Row row;
            //Cell cell; 
            int colWidth = 15 / _cols; 

            for (int count = 0; count < _cols; count++)
            {
                column = table.AddColumn(Unit.FromCentimeter(colWidth));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            row = table.AddRow();
            row.Cells[0].AddParagraph("Lead Name");
            row.Cells[0].Shading.Color = Colors.LightBlue;
            row.Cells[1].AddParagraph("Parameters");
            row.Cells[1].Shading.Color = Colors.LightBlue;
            row.Cells[2].AddParagraph("Values");
            row.Cells[2].Shading.Color = Colors.LightBlue;

            for (int count = 0; count < _rows; count++)
            {
                row = table.AddRow();
                var item = _strToStr.ElementAt(count);

                if (_cols == 3)
                {
                    var key = item.Key;
                    int leadIndex = key.IndexOf(" ");
                    
                    string lead = key.Substring(0, leadIndex);
                    string param = key.Substring(leadIndex);
                    row.Cells[0].AddParagraph(lead);
                    row.Cells[1].AddParagraph(param);
                    row.Cells[2].AddParagraph(item.Value);
                }
                else
                {
                    row.Cells[0].AddParagraph(item.Key);
                    row.Cells[1].AddParagraph(item.Value);
                }
            }

            table.SetEdge(0, 0, _cols, _rows, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }
  }
}
