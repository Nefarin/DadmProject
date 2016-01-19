 //*using System;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;


namespace EKG_Project.IO
{
  class Documents
  {
    public static Document CreateDocument()
    {
      // Create a new MigraDoc document
      Document document = new Document();
      document.Info.Title = "Analisys Report";
      document.Info.Subject = "";
      document.Info.Author = "Krzysztof Kaganiec";

      Styles.DefineStyles(document);

      Cover cover = new Cover(document);
      cover.DefineCover(document);
      TableOfContents.DefineTableOfContents(document);

      DefineContentSection(document);

      //Paragraphs.DefineParagraphs(document);
      Tables.DefineTables(document);
      //Charts.DefineCharts(document);

      return document;
    }

    /// <summary>
    /// Defines page setup, headers, and footers.
    /// </summary>
    static void DefineContentSection(Document document)
    {
      Section section = document.AddSection();
      section.PageSetup.OddAndEvenPagesHeaderFooter = true;
      section.PageSetup.StartingNumber = 1;

      HeaderFooter header = section.Headers.Primary;
      header.AddParagraph("\tOdd Page Header");
      
      header = section.Headers.EvenPage;
      header.AddParagraph("Even Page Header");

      // Create a paragraph with centered page number. See definition of style "Footer".
      Paragraph paragraph = new Paragraph();
      paragraph.AddTab();
      paragraph.AddPageField();

      // Add paragraph to footer for odd pages.
      section.Footers.Primary.Add(paragraph);
      // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
      // not belong to more than one other object. If you forget cloning an exception is thrown.
      section.Footers.EvenPage.Add(paragraph.Clone());
    }
  }
}
