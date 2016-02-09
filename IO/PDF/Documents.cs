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

    public Document Document { get; set; }

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
                int element = (int)_data.ModuleOption;
                System.Console.WriteLine(element);

                PDFModuleClasses.IPDFModuleClass PDFModule;
                switch (element)
                {
                    /*case 0: //ECG_BASELINE

                        PDFModule = new PDFModuleClasses.ECG_Baseline_PDF(Document);
                        PDFModule.FillReportForModule("ECG_BASELINE", _data.statsDictionary);
                        break;

                    */
                    case 1: //R_PEAKS

                        PDFModule = new PDFModuleClasses.R_PEAKS_PDF(Document);
                        PDFModule.FillReportForModule("R_PEAKS", _data.statsDictionary);
                        break;

                    case 3: //HRV1:

                        PDFModule = new PDFModuleClasses.HRV1_PDF(Document);
                        PDFModule.FillReportForModule("HRV1", _data.statsDictionary);
                        break;

                    /*case 4: //HRV2

                        PDFModule = new PDFModuleClasses.HRV2_PDF(Document);
                        PDFModule.FillReportForModule("HRV2", _data.statsDictionary);
                        break;
                        */
                    case 5: //WAVES

                        PDFModule = new PDFModuleClasses.WAVES_PDF(Document);
                        PDFModule.FillReportForModule("WAVES", _data.statsDictionary);
                        break;

                    case 6: //HRV_DFA

                        PDFModule = new PDFModuleClasses.HRV_DFA_PDF(Document);
                        PDFModule.FillReportForModule("HRV_DFA", _data.statsDictionary);
                        break;

                    /*case 8: //ST_SEGMENT:
                        PDFModule = new PDFModuleClasses.ST_SEGMENT_PDF(Document);
                        PDFModule.FillReportForModule("ST_SEGMENT", _data.statsDictionary);
                        break;
                        */
                    case 9:  //T_WAVE_ALT:
                        PDFModule = new PDFModuleClasses.T_WAVE_ALT_PDF(Document);
                        PDFModule.FillReportForModule("T_WAVE_ALT", _data.statsDictionary);
                        break;

                    /*case 10: //SLEEP_APNEA

                        PDFModule = new PDFModuleClasses.SLEEP_APNEA_PDF(Document);
                        PDFModule.FillReportForModule("SLEEP_APNEA", _data.statsDictionary);
                        break;
                        */
                    case 11: //HEART_CLASS

                        PDFModule = new PDFModuleClasses.HEART_CLASS_PDF(Document);
                        PDFModule.FillReportForModule("HEART_CLASS", _data.statsDictionary);
                        break;

                    case 12: //ATRIAL_FIBER

                        PDFModule = new PDFModuleClasses.ATRIAL_FIBER_PDF(Document);
                        PDFModule.FillReportForModule("ATRIAL_FIBER", _data.statsDictionary);
                        break;

                    case 13: //QT_DISP

                        PDFModule = new PDFModuleClasses.QT_DISP_PDF(Document, _data.AnalisysName);
                        PDFModule.FillReportForModule("QT_DISP", _data.statsDictionary);
                        break;

                    /*case 14: //FLUTTER

                        PDFModule = new PDFModuleClasses.FLUTTER_PDF(Document);
                        PDFModule.FillReportForModule("FLUTTER", _data.statsDictionary);
                        break;
                        */
                    case 15: //HRT:

                        PDFModule = new PDFModuleClasses.HRT_PDF(Document);
                        PDFModule.FillReportForModule("HRT", _data.statsDictionary);
                        break;

                    /*case 17: //HEART_AXIS
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
        public enum AvailableOptions
        {
            ECG_BASELINE,
            R_PEAKS,
            VCG_T_LOOP,
            HRV1,
            HRV2,
            WAVES,
            HRV_DFA,
            SIG_EDR,
            ST_SEGMENT,
            T_WAVE_ALT,
            SLEEP_APNEA,
            HEART_CLASS,
            ATRIAL_FIBER,
            QT_DISP,
            FLUTTER,
            HRT,
            ECTOPIC_BEAT,
            HEART_AXIS,
            TEST_MODULE
        }

    }
}
