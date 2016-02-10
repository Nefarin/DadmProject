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
        public Document Document  { get; set; }
        public Section Section { get; set; }

        public TableOfContents(Document document)
        {
            Document = document;
            Section = Document.LastSection;
        }

        public void DefineTableOfContents( System.Collections.Generic.List<string> _moduleList)
        {
            Section.AddPageBreak();
            Paragraph paragraph = Section.AddParagraph("Table of Contents");
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Size = 18;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 24;
            //paragraph.Format.OutlineLevel = OutlineLevel.Level1;

            Hyperlink hyperlink;

            foreach (string element in _moduleList)
            {
                paragraph = Section.AddParagraph();
                paragraph.Style = "TOC";
                hyperlink = paragraph.AddHyperlink(element);
                hyperlink.AddText(element + "\t");
                hyperlink.AddPageRefField(element);
            }

            paragraph = Section.AddParagraph();
            paragraph.Style = "TOC";
            hyperlink = paragraph.AddHyperlink("Plot screenshots");
            hyperlink.AddText("Plot screenshots" + "\t");
            //hyperlink.AddPageRefField("Plot Screens");
        }
    }
}
