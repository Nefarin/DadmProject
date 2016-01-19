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

      //DefineContentSection(Document);

      //Paragraphs.DefineParagraphs(document);
      foreach(string element in _moduleList)
            {
                PDFModuleClasses.IPDFModuleClass PDFModule;
                switch (element)
                {
                    case "ECG_BASELINE":

                        PDFModule = new PDFModuleClasses.ECG_Baseline_PDF(Document);
                        PDFModule.FillReportForModule(element);
                        break;
                    
                        
                    case "R_PEAKS":

                        PDFModule = new PDFModuleClasses.ECG_Baseline_PDF(Document);
                        PDFModule.FillReportForModule(element);
                        break;

                    /*
                case AvailableOptions.WAVES:
                    if (this.ModuleParam == null)
                    {
                        this.ModuleParam = new Waves_Params(this.AnalysisName);
                        this.ModuleParam.GUIParametersAvailable = true;
                        FillDictionaries();
                    }
                    break;
                case AvailableOptions.ATRIAL_FIBER:
                    if (this.ModuleParam == null)
                    {
                        this.ModuleParam = new Atrial_Fibr_Params(this.AnalysisName);
                        this.ModuleParam.GUIParametersAvailable = true;
                        FillDictionaries();
                    }
                    break;
                case AvailableOptions.HEART_CLASS:
                    this.ModuleParam = new Heart_Class_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.HEART_AXIS:
                    this.ModuleParam = new Heart_Axis_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.SLEEP_APNEA:
                    this.ModuleParam = new Sleep_Apnea_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.HRV2:
                    this.ModuleParam = new HRV2_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.QT_DISP:
                    if (this.ModuleParam == null)
                    {
                        this.ModuleParam = new QT_Disp_Params(this.AnalysisName);
                        this.ModuleParam.GUIParametersAvailable = true;
                        FillDictionaries();
                    }
                    break;
                case AvailableOptions.FLUTTER:
                    this.ModuleParam = new Flutter_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.HRV_DFA:
                    this.ModuleParam = new HRV_DFA_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;

                case AvailableOptions.TEST_MODULE:
                    this.ModuleParam = new TestModule_Params(500);
                    this.ModuleParam.GUIParametersAvailable = true;
                    FillDictionaries();
                    break;s
                case AvailableOptions.HRV1:
                    this.ModuleParam = new HRV1_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.ST_SEGMENT:
                    this.ModuleParam = new ST_Segment_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.T_WAVE_ALT:
                    this.ModuleParam = new T_Wave_Alt_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.SIG_EDR:
                    this.ModuleParam = new SIG_EDR_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;
                case AvailableOptions.HRT:
                    this.ModuleParam = new HRT_Params(this.AnalysisName);
                    this.ModuleParam.GUIParametersAvailable = false;
                    FillDictionaries();
                    break;*/
                    default:

                        break;
                }
            }

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
