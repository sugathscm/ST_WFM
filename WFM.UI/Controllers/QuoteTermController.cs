using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;

namespace WFM.UI.Controllers
{
     public class QuoteTermController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly QuoteTermService quoteTermService = new QuoteTermService();

        public QuoteTermController()
        {
        }

        public QuoteTermController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: QuoteTerm
        public ActionResult Index(int? id)
        {
            QuoteTerm quoteTerm = new QuoteTerm();
            if (id != null)
            {
                quoteTerm = quoteTermService.GetQuoteTermById(id);
            }
            return View(quoteTerm);
        }

        public ActionResult GetList()
        {
            List<QuoteTerm> list = quoteTermService.GetQuoteTermList();

            List<BaseViewModel> modelList = new List<BaseViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new BaseViewModel() { Id = item.Id, IsActive = item.IsActive, Name = item.Name });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(QuoteTerm model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                QuoteTerm quoteTerm = null;
                QuoteTerm oldQuoteTerm = null;
                if (model.Id == 0)
                {
                    quoteTerm = new QuoteTerm
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldQuoteTerm = new QuoteTerm();
                    oldData = new JavaScriptSerializer().Serialize(oldQuoteTerm);
                    newData = new JavaScriptSerializer().Serialize(quoteTerm);
                }
                else
                {
                    quoteTerm = quoteTermService.GetQuoteTermById(model.Id);
                    oldQuoteTerm = quoteTermService.GetQuoteTermById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new QuoteTerm()
                    {
                        Id = oldQuoteTerm.Id,
                        Name = oldQuoteTerm.Name,
                        IsActive = oldQuoteTerm.IsActive
                    });

                    quoteTerm.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    quoteTerm.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new QuoteTerm()
                    {
                        Id = quoteTerm.Id,
                        Name = quoteTerm.Name,
                        IsActive = quoteTerm.IsActive
                    });
                }

                quoteTermService.SaveOrUpdate(quoteTerm);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "QuoteTerm",
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


            return RedirectToAction("Index", "QuoteTerm");
        }
    }

}