using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;

namespace WFM.UI.Controllers
{
    public class QuoteController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly QuoteService quoteService = new QuoteService();
        private readonly ClientService clientService = new ClientService();
        private readonly EmployeeService employeeService = new EmployeeService();
        private readonly CategoryService categoryService = new CategoryService();
        private readonly QuoteTermService quoteTermService = new QuoteTermService();

        public QuoteController()
        {

        }
        public QuoteController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Quote
        public ActionResult Index()
        {
            return View();
        }

        // GET: Quote
        public ActionResult Details(int? id)
        {
            Quote quote = new Quote();

            if (id != null)
            {
                quote = quoteService.GetQuoteById(id);
            }
            else
            {
                var code = CommonService.GenerateCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), false).Replace('-', '/');
                quote.Code = code;
            }

            var quoteList = quoteService.GetQuoteList().Where(q => q.ConvertedToOrder == false).ToList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();
            var quoteTermList = quoteTermService.GetQuoteTermList();

            ViewBag.QuoteTermList = quoteTermList;
            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");
            ViewBag.CategoryList = new SelectList(categoryService.GetCategoryList(), "Id", "Name");

            return View(quote);
        }

        public ActionResult GetList()
        {

            var list = quoteService.GetQuoteFullList();
            List<QuoteView> modelList = new List<QuoteView>();
            foreach (var item in list)
            {
                modelList.Add(new QuoteView()
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    Version = item.Version,
                    Code = item.Code,
                    Value = item.Value,
                    FileAttched = item.FileAttched,
                    CreatedDate = item.CreatedDate,
                    CreatedDateString = item.CreatedDate.Value.ToLongDateString()
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int? id)
        {
            object documentFormat = 8;
            string _FileName = id.ToString() + ".docx";
            string _docFilepath = Path.Combine(Server.MapPath("~/Quotes"), _FileName);

            byte[] fileBytes = System.IO.File.ReadAllBytes(_docFilepath);
            string fileName = _FileName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult Print(int? id)
        {
            object documentFormat = 8;
            string _FileName = id.ToString() + ".docx";
            string _docFilepath = Path.Combine(Server.MapPath("~/Quotes"), _FileName);
            object _pdfFilePath = Server.MapPath("~/Quotes/") + id.ToString() + ".pdf";
            object fileSavePath = Path.Combine(Server.MapPath("~/Quotes"), _FileName);

            _Application applicationclass = new Application();
            applicationclass.Documents.Open(ref fileSavePath);
            applicationclass.Visible = false;
            Document document = applicationclass.ActiveDocument;
            document.SaveAs(ref _pdfFilePath, ref documentFormat);
            document.Close();

            //WordDocument wordDocument = new WordDocument(_docFilepath, FormatType.Docx);
            //DocToPDFConverter converter = new DocToPDFConverter();
            //PdfDocument pdfDocument = converter.ConvertToPDF(wordDocument);
            //pdfDocument.Save(_pdfFilePath);
            //pdfDocument.Close(true);
            //wordDocument.Close();
            //System.Diagnostics.Process.Start("WordtoPDF.pdf");

            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("content-disposition", "attachment; filename=" + id.ToString() + ".pdf");
            response.WriteFile(_pdfFilePath.ToString());
            response.ContentType = "";
            response.End();

            byte[] fileBytes = System.IO.File.ReadAllBytes(_docFilepath);
            string fileName = _FileName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Quote model, FormCollection formCollection)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                var productIdArray = formCollection["productIdArray"].Split(',');
                var qtyArray = formCollection["qtyArray"].Split(',');
                var descriptionArray = formCollection["descriptionArray"].Split(',');
                var costArray = formCollection["costArray"].Split(',');
                var vatArray = formCollection["vatArray"].Split(',');
                var sizeArray = formCollection["sizeArray"].Split(',');

                int id = model.Id;
                Quote quote = null;
                Quote oldQuote = null;
                if (model.Id == 0)
                {
                    
                    quote = new Quote
                    {
                        ClientId = model.ClientId,
                        Code = model.Code,
                        CodeNumber = int.Parse(model.Code.Split('-')[3]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        Version = 1,
                        ChanneledBy = model.ChanneledBy,
                        Value = model.Value,
                        Comments = model.Comments,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId(),
                        Header = model.Header,
                        IsActive = true,
                        FileAttched = false,
                        ConvertedToOrder = false,
                        IsVAT = model.IsVAT
                    };

                    oldQuote = new Quote();
                    oldData = new JavaScriptSerializer().Serialize(oldQuote);
                    newData = new JavaScriptSerializer().Serialize(quote);
                }
                else
                {
                    quote = quoteService.GetQuoteById(model.Id);
                    oldQuote = quoteService.GetQuoteById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Quote()
                    {
                        Id = oldQuote.Id
                    });

                    quote.ClientId = model.ClientId;
                    quote.ChanneledBy = model.ChanneledBy;
                    quote.Value = model.Value;
                    //quote.Version = quote.Version + 1;
                    quote.Comments = model.Comments;
                    quote.UpdatedBy = User.Identity.GetUserId();
                    quote.UpdatedDate = DateTime.Now;

                    newData = new JavaScriptSerializer().Serialize(new Quote()
                    {
                        Id = quote.Id
                    });
                }


                int i = 0;

                foreach(var item in productIdArray)
                {
                    QuoteItem quoteItem = new QuoteItem
                    {
                        CatgoryId = int.Parse(item),
                        Qty = (qtyArray[i] == "") ? 0 : double.Parse(qtyArray[i]),
                        Value = (costArray[i] == "") ? 0 : decimal.Parse(costArray[i]),
                        VAT = (vatArray[i] == "") ? 0 : decimal.Parse(vatArray[i]),
                        Size = (sizeArray[i] == "") ? "" : sizeArray[i]
                    };
                    i++;
                    quote.QuoteItems.Add(quoteItem);
                }

                quoteService.SaveOrUpdate(quote);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Quote",
                    NewData = newData,
                    OldData = oldData,
                    UpdatedOn = DateTime.Now,
                    UserId = User.Identity.GetUserId()
                });

                TempData["Message"] = ResourceData.SaveSuccessMessage;
            }
            catch (Exception ex)
            {
                TempData["Message"] = string.Format(ResourceData.SaveErrorMessage, ex.InnerException);
            }

            return RedirectToAction("Index", "Quote");
        }

        public PartialViewResult _NewClientPartial()
        {
            return PartialView();
        }
    }
}