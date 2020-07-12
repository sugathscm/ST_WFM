using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;

namespace WFM.UI.Controllers
{
    public class QuoteController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly QuoteService quoteService = new QuoteService();
        private readonly ClientService clientService = new ClientService();
        private readonly EmployeeService employeeService = new EmployeeService();

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

            var quoteList = quoteService.GetQuoteList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();

            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Name");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");

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
                    Value = item.Value
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Quote model, HttpPostedFileBase file)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Quote quote = null;
                Quote oldQuote = null;
                if (model.Id == 0)
                {
                    var code = CommonService.GenerateCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"));
                    quote = new Quote
                    {
                        ClientId = model.ClientId,
                        Code = code.Replace('-', '/'),
                        CodeNumber = int.Parse(code.Split('-')[2]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        Version = 1,
                        ChanneledBy = model.ChanneledBy,
                        Value = model.Value,
                        Comments = model.Comments,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId()
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
                    quote.Comments = model.Comments;
                    quote.UpdatedBy = User.Identity.GetUserId();
                    quote.UpdatedDate = DateTime.Now;

                    newData = new JavaScriptSerializer().Serialize(new Quote()
                    {
                        Id = quote.Id
                    });
                }

                quoteService.SaveOrUpdate(quote);

                if (file.ContentLength > 0)
                {
                    string _ext = Path.GetExtension(file.FileName);
                    string _FileName = quote.Id.ToString() + _ext;
                    string _path = Path.Combine(Server.MapPath("~/Quotes"), _FileName);
                    file.SaveAs(_path);
                }


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
    }
}