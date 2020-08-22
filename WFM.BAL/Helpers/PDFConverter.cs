using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFM.BAL.Helpers
{
    public class PDFConverter : MarshalByRefObject
    {
        object paramMissing = Type.Missing;
        public string errormessage;
        private bool wordavailable = false;
        private bool checkedword = false;

        public bool WordIsAvailable()
        {
            //don't check every time.... first time only
            if (!checkedword)
            {

                try
                {
                    _Application wordApplication = new Application()
                    {
                        Visible = true
                    };
                    wordApplication.Quit(ref paramMissing, ref paramMissing,
                                         ref paramMissing);
                    wordavailable = true;
                }
                catch
                {
                    wordavailable = false;
                }
                checkedword = true;
            }
            return wordavailable;

        }

        public bool convert(string source, string output)
        {
            if (System.IO.File.Exists(source))
            {
                //start conversion
                try
                {
                    _Application wordApplication = new Application();
                    Document wordDocument = null;
                    object paramSourceDocPath = source;

                    string paramExportFilePath = output;

                    //this part is copied from Microsoft MSDN

                    //set exportformat to pdf
                    WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
                    bool paramOpenAfterExport = false;
                    WdExportOptimizeFor paramExportOptimizeFor =
                        WdExportOptimizeFor.wdExportOptimizeForPrint;
                    WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
                    int paramStartPage = 0;
                    int paramEndPage = 0;
                    WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
                    bool paramIncludeDocProps = true;
                    bool paramKeepIRM = true;
                    WdExportCreateBookmarks paramCreateBookmarks =
                        WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    bool paramDocStructureTags = true;
                    bool paramBitmapMissingFonts = true;
                    bool paramUseISO19005_1 = false;

                    try
                    {
                        // Open the source document.
                        wordDocument = wordApplication.Documents.Open(
                            ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                            ref paramMissing, ref paramMissing, ref paramMissing,
                            ref paramMissing, ref paramMissing, ref paramMissing,
                            ref paramMissing, ref paramMissing, ref paramMissing,
                            ref paramMissing, ref paramMissing, ref paramMissing,
                            ref paramMissing);

                        // Export it in the specified format.
                        if (wordDocument != null)
                            wordDocument.ExportAsFixedFormat(paramExportFilePath,
                                                             paramExportFormat, paramOpenAfterExport,
                                                             paramExportOptimizeFor, paramExportRange, paramStartPage,
                                                             paramEndPage, paramExportItem, paramIncludeDocProps,
                                                             paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                                                             paramBitmapMissingFonts, paramUseISO19005_1,
                                                             ref paramMissing);
                    }
                    catch (Exception ex)
                    {
                        // Respond to the error
                        errormessage = ex.Message;
                    }
                    finally
                    {
                        // Close and release the Document object.
                        if (wordDocument != null)
                        {
                            wordDocument.Close(ref paramMissing, ref paramMissing,
                                               ref paramMissing);
                            wordDocument = null;
                        }

                        // Quit Word and release the ApplicationClass object.
                        if (wordApplication != null)
                        {
                            wordApplication.Quit(ref paramMissing, ref paramMissing,
                                                 ref paramMissing);
                            wordApplication = null;
                        }

                        //i don't know why this is here two times. I just copied it from the MSDN howto interop with word 2007
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception err)
                {
                    errormessage = err.Message;
                }
                return true;

            }
            else
            {
                errormessage = "ERROR: Inputfile not found";
            }

            return false;

        }

    }

}
