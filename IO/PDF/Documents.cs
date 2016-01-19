 //*using System;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;


namespace EKG_Project.IO
{
  class Documents
  {
    public Documents ()
    {
        // Create a new MigraDoc document
        Document = new Document();
        Document.Info.Title = "Analisys Report";
        Document.Info.Subject = "";
        Document.Info.Author = "Krzysztof Kaganiec";
    }

    public Document Document { get; set; }

    public Document CreateDocument(System.Collections.Generic.List<string> _moduleList)
    {
      Styles.DefineStyles(Document);

      Cover cover = new Cover(Document);
      cover.DefineCover(Document, _moduleList);
      TableOfContents tableContent = new TableOfContents(Document);
      tableContent.DefineTableOfContents(_moduleList);

      DefineContentSection(Document);

      //Paragraphs.DefineParagraphs(document);
      Tables.DefineTables(Document);
      //Charts.DefineCharts(document);

      return Document;
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
