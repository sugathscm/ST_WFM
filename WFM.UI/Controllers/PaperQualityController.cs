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
    //[Authorize]
    public class PaperQualityController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly PaperQualityService paperQualityService = new PaperQualityService();

        public PaperQualityController()
        {
        }

        public PaperQualityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: PaperQuality
        public ActionResult Index(int? id)
        {
            PaperQuality paperQuality = new PaperQuality();
            if (id != null)
            {
                paperQuality = paperQualityService.GetPaperQualityById(id);
            }
            return View(paperQuality);
        }

        public ActionResult GetList()
        {
            List<PaperQuality> list = paperQualityService.GetPaperQualityList();

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
        public ActionResult SaveOrUpdate(PaperQuality model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                PaperQuality paperQuality = null;
                PaperQuality oldPaperQuality = null;
                if (model.Id == 0)
                {
                    paperQuality = new PaperQuality
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldPaperQuality = new PaperQuality();
                    oldData = new JavaScriptSerializer().Serialize(oldPaperQuality);
                    newData = new JavaScriptSerializer().Serialize(paperQuality);
                }
                else
                {
                    paperQuality = paperQualityService.GetPaperQualityById(model.Id);
                    oldPaperQuality = paperQualityService.GetPaperQualityById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new PaperQuality()
                    {
                        Id = oldPaperQuality.Id,
                        Name = oldPaperQuality.Name,
                        IsActive = oldPaperQuality.IsActive
                    });

                    paperQuality.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    paperQuality.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new PaperQuality()
                    {
                        Id = paperQuality.Id,
                        Name = paperQuality.Name,
                        IsActive = paperQuality.IsActive
                    });
                }

                paperQualityService.SaveOrUpdate(paperQuality);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "PaperQuality",
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


            return RedirectToAction("Index", "PaperQuality");
        }
    }
}