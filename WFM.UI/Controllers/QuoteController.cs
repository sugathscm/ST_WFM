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
using System.Web.Configuration;
using WFM.BAL.Helpers;

namespace WFM.UI.Controllers
{
    public class QuoteController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly QuoteService quoteService = new QuoteService();
        private readonly ClientService clientService = new ClientService();
        private readonly EmployeeService employeeService = new EmployeeService();
        private readonly DivisionService divisionService = new DivisionService();
        private readonly CategoryService categoryService = new CategoryService();
        private readonly QuoteTermService quoteTermService = new QuoteTermService();
        private readonly OrderTypeService orderTypeService = new OrderTypeService();
        private readonly WarrantyPeriodService warrantyPeriodService = new WarrantyPeriodService();

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
            ViewBag.Divisions = divisionService.GetDivisionList();
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        // GET: Quote
        public ActionResult Details(int? id)
        {
            Quote quote = new Quote();

            var quoteList = quoteService.GetQuoteList().Where(q => q.ConvertedToOrder == false).OrderBy(o => o.Id).ToList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();
            var orderTypeList = orderTypeService.GetOrderTypeList();
            var quoteTermList = quoteTermService.GetQuoteTermList();

            ViewBag.QuoteTermList = quoteTermList;
            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.OrderTypeList = new SelectList(orderTypeList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");

            List<BaseViewModel> WarrantyPeriodList = new List<BaseViewModel>();
            for (int i = 1; i < 10; i++)
            {
                WarrantyPeriodList.Add(new BaseViewModel() { Id = i, Name = i.ToString() });
            }

            List<BaseViewModel> VisibilityList = new List<BaseViewModel>();

            VisibilityList.Add(new BaseViewModel() { Id = 1, Name = "Single Sided" });
            VisibilityList.Add(new BaseViewModel() { Id = 2, Name = "Double Sided" });

            ViewBag.WarrantyPeriodList = WarrantyPeriodList;
            ViewBag.VisibilityList = VisibilityList;
            ViewBag.CategoryList = categoryService.GetCategoryList();
            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];

            if (id != null)
            {
                quote = quoteService.GetQuoteById(id);
            }
            else
            {
                var code = CommonService.GenerateQuoteCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), false).Replace('-', '/');
                quote.Code = code;
            }

            return View(quote);
        }

        public ActionResult GetHistoryList()
        {
            var list = quoteService.GetQuoteConvertedList();

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
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    IsApproved = item.IsApproved,
                    Header = item.Header
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            var list = quoteService.GetQuoteActiveList();

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
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    IsApproved = item.IsApproved,
                    Header = item.Header
                   
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
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
                var categoryTypeArray = formCollection["categoryTypeArray"].Split(',');
                var visibilityArray = formCollection["visibilityArray"].Split(',');
                var frameworkWarrantyPeriodArray = formCollection["frameworkWarrantyPeriodArray"].Split(',');
                var letteringWarrantyPeriodArray = formCollection["letteringWarrantyPeriodArray"].Split(',');
                var illuminationWarrantyPeriodArray = formCollection["illuminationWarrantyPeriodArray"].Split(',');
                var installationArray = formCollection["installationArray"].Split(',');
                var termsArray = formCollection["termsArray"].Split(',');

                int id = model.Id;
                Quote quote = null;
                Quote oldQuote = null;
                if (model.Id == 0)
                {

                    quote = new Quote
                    {
                        ClientId = model.ClientId,
                        Code = model.Code,
                        CodeNumber = int.Parse(model.Code.Split('/')[3]),
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
                        IsVAT = model.IsVAT,
                        WarrantyPeriodId = model.WarrantyPeriodId,
                        ContactPerson = model.ContactPerson,
                        ContactMobile = model.ContactMobile,
                        FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod,
                        IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod,
                        LetteringWarrantyPeriod = model.LetteringWarrantyPeriod,
                        PowerSupplyAmp = model.PowerSupplyAmp,
                        AdvancePayment = model.AdvancePayment,
                        ValidDays = model.ValidDays,
                        DeliveryPeriod = model.DeliveryPeriod,
                        IsConverted =false,
                        // OrderTypeId = model.OrderTypeId
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
                    quote.Header = model.Header;
                    quote.Comments = model.Comments;
                    quote.UpdatedBy = User.Identity.GetUserId();
                    quote.UpdatedDate = DateTime.Now;
                    quote.ContactPerson = model.ContactPerson;
                    quote.ContactMobile = model.ContactMobile;
                    quote.AdvancePayment = model.AdvancePayment;
                    quote.PowerSupplyAmp = model.PowerSupplyAmp;
                    quote.ValidDays = model.ValidDays;
                    quote.DeliveryPeriod = model.DeliveryPeriod;
                    //quote.OrderTypeId = model.OrderTypeId;
                    quote.FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod;
                    quote.IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod;
                    quote.LetteringWarrantyPeriod = model.LetteringWarrantyPeriod;

                    newData = new JavaScriptSerializer().Serialize(new Quote()
                    {
                        Id = quote.Id
                    });

                    quoteService.RemoveItems(quote);
                    quoteService.RemoveTerms(quote);
                }

                int i = 0;
                quote.QuoteItems.Clear();
                QuoteItem quoteItem = null;

                foreach (var term in termsArray)
                {
                    if (model.Id == 0)
                    {
                        quote.QuoteTermDetails.Add(new QuoteTermDetail() { QuoteTermId = int.Parse(term) });
                    }
                    else
                    {
                        QuoteTermDetail quoteTermDetail =
                            new QuoteTermDetail()
                            {
                                QuoteTermId = int.Parse(term),
                                QuoteId = model.Id
                            };

                        quoteService.SaveOrUpdate(quoteTermDetail);
                    }
                }

                foreach (var item in productIdArray)
                {
                    if (model.Id == 0)
                    {
                        quoteItem = new QuoteItem
                        {
                            CategoryId = int.Parse(item),
                            CategoryName = categoryService.GetCategoryById(int.Parse(item)).Name,
                            Qty = (qtyArray[i] == "") ? 0 : double.Parse(qtyArray[i]),
                            UnitCost = (costArray[i] == "") ? 0 : double.Parse(costArray[i]),
                            VAT = (vatArray[i] == "") ? 0 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i],
                            Installation = (installationArray[i] == "") ? "" : installationArray[i],
                            CategoryType = (categoryTypeArray[i] == "") ? "" : categoryTypeArray[i],
                            VisibilityId = int.Parse(visibilityArray[i]),
                            FrameworkWarrantyPeriod = int.Parse(frameworkWarrantyPeriodArray[i]),
                            LetteringWarrantyPeriod = int.Parse(letteringWarrantyPeriodArray[i]),
                            IlluminationWarrantyPeriod = int.Parse(illuminationWarrantyPeriodArray[i]),
                        };

                        quote.QuoteItems.Add(quoteItem);
                    }
                    else
                    {
                        quoteItem = new QuoteItem
                        {
                            CategoryId = int.Parse(item),
                            CategoryName = categoryService.GetCategoryById(int.Parse(item)).Name,
                            Qty = (qtyArray[i] == "") ? 0 : double.Parse(qtyArray[i]),
                            UnitCost = (costArray[i] == "") ? 0 : double.Parse(costArray[i]),
                            VAT = (vatArray[i] == "") ? 0 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i],
                            Installation = (installationArray[i] == "") ? "" : installationArray[i],
                            CategoryType = (categoryTypeArray[i] == "") ? "" : categoryTypeArray[i],
                            VisibilityId = int.Parse(visibilityArray[i]),
                            FrameworkWarrantyPeriod = int.Parse(frameworkWarrantyPeriodArray[i]),
                            LetteringWarrantyPeriod = int.Parse(letteringWarrantyPeriodArray[i]),
                            IlluminationWarrantyPeriod = int.Parse(illuminationWarrantyPeriodArray[i]),
                            QuoteId = model.Id
                        };

                        quoteService.SaveOrUpdate(quoteItem);
                    }

                    i++;
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

        public ActionResult PrintQuote(int id)
        {
            Quote quote = quoteService.GetQuoteById(id);
            QuoteView quoteView = new QuoteView();

            PropertyCopier<Quote, QuoteView>.Copy(quote, quoteView);

            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];
            ViewBag.VATNo = ResourceData.VATNo;
            ViewBag.Name = ResourceData.Name;
            ViewBag.AuthorizedPersonName = ResourceData.AuthorizedPersonName;
            ViewBag.AuthorizedPersonDesignation = ResourceData.AuthorizedPersonDesignation;

            quoteView.CreatedDateString = quoteView.CreatedDate.Value.ToString("dd/MM/yyyy");

            quoteView.QuoteTermDetails = quoteTermService.GetQuoteTermsByQuoteId(quote.Id);

            return PartialView("_PrintQuote", quoteView);
        }

        public ActionResult WayForwardQuote(int id)
        {
            Quote quote = quoteService.GetQuoteById(id);
            ViewBag.Divisions = divisionService.GetDivisionList();
            return PartialView("_WayForwardQuote", quote);
        }

        public ActionResult ApproveQuote(int id)
        {
            try
            {
                Quote quote = quoteService.GetQuoteById(id);
                //quote.ApprovedBy = User.Identity.GetUserId();
                quote.ApprovedDate = DateTime.Now;
                quote.IsApproved = true;
                quoteService.SaveOrUpdate(quote);

                return RedirectToAction("Index", "Quote");
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }

        public ActionResult ConvertQuote(FormCollection formCollection)
        {
            try
            {
                int id = int.Parse(formCollection["id"]);
                string wayforward = formCollection["wayforward"];

                List<DocumentViewModel> files = new List<DocumentViewModel>();

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        if (Request.Files[i].ContentLength > 0)
                            files.Add(new DocumentViewModel() { DocumentId = Request.Files.AllKeys[i], PostedFile = Request.Files[i] });
                    }
                }

                foreach (var file in files)
                {
                    string path = Path.Combine(Server.MapPath("~/Docs/" + id + "/" + file.DocumentId));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (file != null)
                    {
                        string fileName = Path.GetFileName(file.PostedFile.FileName);
                        file.PostedFile.SaveAs(path + "/" + fileName);
                    }
                }

                Quote quote = quoteService.GetQuoteById(id);
                quote.UpdatedDate = DateTime.Now;
                quote.IsConverted = true;
                quoteService.SaveOrUpdate(quote);

                OrderService orderService = new OrderService();
                orderService.SaveOrUpdate(quote, User.Identity.GetUserId(), wayforward);

                return RedirectToAction("Index", "Quote");
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }
    }
}