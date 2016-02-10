using System;
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
      
        //Document.DefaultPageSetup.TopMargin = 3.5;
    }

        //PROPERTIES
        /// <summary>
        /// Stores Pdf filename
        /// </summary>
        public Document Document { get; set; }

        //METHODS
        /// <summary>
        /// Create pdf document
        /// </summary>
        /// <param name="_data">PDFStore data</param>
        /// <param name="init">true if first called</param>
        public Document CreateDocument(PDF.StoreDataPDF _data, bool init)
        {
            if (init)
            {
                Styles.DefineStyles(Document);

                Cover cover = new Cover(Document);
                cover.DefineCover(Document, _data);
                TableOfContents tableContent = new TableOfContents(Document);
                tableContent.DefineTableOfContents(_data.ModuleList);
            }
            else
            {
                GUI.AvailableOptions element = _data.ModuleOption;
                System.Console.WriteLine(element);

                PDFModuleClasses.IPDFModuleClass PDFModule;
                switch (element)
                {
                    /*case 0: //ECG_BASELINE

                        PDFModule = new PDFModuleClasses.ECG_Baseline_PDF(Document);
                        PDFModule.FillReportForModule("ECG_BASELINE", _data.statsDictionary);
                        break;

                    */
                    case GUI.AvailableOptions.R_PEAKS:

                        PDFModule = new PDFModuleClasses.R_PEAKS_PDF(Document);
                        PDFModule.FillReportForModule("R_PEAKS", _data.statsDictionary);
                        break;

                    case GUI.AvailableOptions.HRV1:

                        PDFModule = new PDFModuleClasses.HRV1_PDF(Document);
                        PDFModule.FillReportForModule("HRV1", _data.statsDictionary);
                        break;

                    /*case GUI.AvailableOptions.HRV2:

                        PDFModule = new PDFModuleClasses.HRV2_PDF(Document);
                        PDFModule.FillReportForModule("HRV2", _data.statsDictionary);
                        break;
                        */
                    case GUI.AvailableOptions.WAVES:

                        PDFModule = new PDFModuleClasses.WAVES_PDF(Document);
                        PDFModule.FillReportForModule("WAVES", _data.statsDictionary);
                        break;

                    case GUI.AvailableOptions.HRV_DFA:

                        PDFModule = new PDFModuleClasses.HRV_DFA_PDF(Document);
                        PDFModule.FillReportForModule("HRV_DFA", _data.statsDictionary);
                        break;

                    /*case GUI.AvailableOptions.ST_SEGMENT:
                        PDFModule = new PDFModuleClasses.ST_SEGMENT_PDF(Document);
                        PDFModule.FillReportForModule("ST_SEGMENT", _data.statsDictionary);
                        break;
                        */
                    case GUI.AvailableOptions.T_WAVE_ALT:
                        PDFModule = new PDFModuleClasses.T_WAVE_ALT_PDF(Document);
                        PDFModule.FillReportForModule("T_WAVE_ALT", _data.statsDictionary);
                        break;

                    /*case GUI.AvailableOptions.SLEEP_APNEA:

                        PDFModule = new PDFModuleClasses.SLEEP_APNEA_PDF(Document);
                        PDFModule.FillReportForModule("SLEEP_APNEA", _data.statsDictionary);
                        break;
                        */
                    case GUI.AvailableOptions.HEART_CLASS:

                        PDFModule = new PDFModuleClasses.HEART_CLASS_PDF(Document);
                        PDFModule.FillReportForModule("HEART_CLASS", _data.statsDictionary);
                        break;

                    case GUI.AvailableOptions.ATRIAL_FIBER:

                        PDFModule = new PDFModuleClasses.ATRIAL_FIBER_PDF(Document);
                        PDFModule.FillReportForModule("ATRIAL_FIBER", _data.statsDictionary);
                        break;

                    case GUI.AvailableOptions.QT_DISP:

                        PDFModule = new PDFModuleClasses.QT_DISP_PDF(Document, _data.AnalisysName);
                        PDFModule.FillReportForModule("QT_DISP", _data.statsDictionary);
                        break;

                    /*caseGUI.AvailableOptions.FLUTTER:

                        PDFModule = new PDFModuleClasses.FLUTTER_PDF(Document);
                        PDFModule.FillReportForModule("FLUTTER", _data.statsDictionary);
                        break;
                        */
                    case GUI.AvailableOptions.HRT:

                        PDFModule = new PDFModuleClasses.HRT_PDF(Document);
                        PDFModule.FillReportForModule("HRT", _data.statsDictionary);
                        break;

                    /*case GUI.AvailableOptions.HEART_AXIS:
                        PDFModule = new PDFModuleClasses.HEART_AXIS_PDF(Document);
                        PDFModule.FillReportForModule("HEART_AXIS", _data.statsDictionary);
                        break;
                        */
                    /*
                    case AvailableOptions.SIG_EDR:
                        this.ModuleParam = new SIG_EDR_Params(this.AnalysisName);
                        this.ModuleParam.GUIParametersAvailable = false;
                        FillDictionaries();
                        break;
                    */
                    default:

                        break;
                }
                
            }

      return Document;
    }

    }
}
