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
            Table table = new Table(); Console.WriteLine("1");
            Console.WriteLine("rows" + _rows);
            _rows = _strToStr.Count;

            table.Borders.Width = 2;

            Column column;
            Row row;
            //Cell cell; 
            int colWidth = 15 / _cols; Console.WriteLine("2");

            for (int count = 0; count < _cols; count++)
            {
                Console.WriteLine("cols" + _cols);
                column = table.AddColumn(Unit.FromCentimeter(colWidth));
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            Console.WriteLine("rows" + _rows);
            for (int count = 0; count < _rows; count++)
            {
                Console.WriteLine("rows" + _rows);
                row = table.AddRow();
                var item = _strToStr.ElementAt(count);
                Console.WriteLine("cols" + _cols);
                if (_cols == 3)
                {
                    var key = item.Key;
                    int leadIndex = key.IndexOf(" ");
                    
                    string lead = key.Substring(0, leadIndex);
                    Console.WriteLine(lead);
                    string param = key.Substring(leadIndex);
                    Console.WriteLine(param);
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
