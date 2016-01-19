using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Fields;

namespace EKG_Project.IO
{ 
  public class TableOfContents
  {
    /// <summary>
    /// Defines the cover page.
    /// </summary>
    public static void DefineTableOfContents(Document document)
    {
      Section section = document.LastSection;

      section.AddPageBreak();
      Paragraph paragraph = section.AddParagraph("Table of Contents");
      paragraph.Format.SpaceBefore = "3cm";
      paragraph.Format.Font.Size = 18;
      paragraph.Format.Font.Bold = true;
      paragraph.Format.SpaceAfter = 24;
      paragraph.Format.OutlineLevel = OutlineLevel.Level1;

      paragraph = section.AddParagraph();
      paragraph.Style = "TOC";
      Hyperlink hyperlink = paragraph.AddHyperlink("Module 1");
      hyperlink.AddText("Module 1\t");
      //hyperlink.AddPageRefField("Paragraphs");

      paragraph = section.AddParagraph();
      paragraph.Style = "TOC";
      hyperlink = paragraph.AddHyperlink("Module 2");
      hyperlink.AddText("Module 2\t");
      //hyperlink.AddPageRefField("Tables");

      paragraph = section.AddParagraph();
      paragraph.Style = "TOC";
      hyperlink = paragraph.AddHyperlink("Module n");
      hyperlink.AddText("Module 3\t");
            //hyperlink.AddPageRefField("Charts");

        paragraph = section.AddParagraph();
        paragraph.Style = "TOC";
        hyperlink = paragraph.AddHyperlink("Module n");
        hyperlink.AddText("Module 4\t");
        //hyperlink.AddPageRefField("Charts");

        paragraph = section.AddParagraph();
        paragraph.Style = "TOC";
        hyperlink = paragraph.AddHyperlink("Module n");
        hyperlink.AddText("Module n\t");
        //hyperlink.AddPageRefField("Charts");
        }
    }
}
