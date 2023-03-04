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
    public class WarrantyPeriodController :Controller
    {
        private ApplicationUserManager _userManager;
        private readonly WarrantyPeriodService warrantyPeriodService = new WarrantyPeriodService();

        public WarrantyPeriodController()
        {
        }

        public WarrantyPeriodController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public ActionResult Index(int? id)
        {
            WarrantyPeriod warrantyPeriod = new WarrantyPeriod();
            if (id != null)
            {
                warrantyPeriod = warrantyPeriodService.GetWarrantyPeriodById(id);
            }
            return View(warrantyPeriod);
        }

        public ActionResult GetList()
        {
            List<WarrantyPeriod> list = warrantyPeriodService.GetWarrantyPeriodList();

            List<WarrantyPeriodViewModel> modelList = new List<WarrantyPeriodViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new WarrantyPeriodViewModel()
                {
                    Id = item.Id,
                    Duration = item.Duration

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Design,Factory")]
        public ActionResult SaveOrUpdate(WarrantyPeriod model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                WarrantyPeriod warrantyPeriod = null;
                WarrantyPeriod oldwarrantyPeriod = null;
                if (model.Id == 0)
                {
                    warrantyPeriod = new WarrantyPeriod()
                    {
                        Duration = model.Duration

                    };

                    oldwarrantyPeriod = new WarrantyPeriod();
                    oldData = new JavaScriptSerializer().Serialize(oldwarrantyPeriod);
                    newData = new JavaScriptSerializer().Serialize(warrantyPeriod);
                }
                else
                {
                    warrantyPeriod = warrantyPeriodService.GetWarrantyPeriodById(model.Id);
                    oldwarrantyPeriod = warrantyPeriodService.GetWarrantyPeriodById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new WarrantyPeriod()
                    {
                        Id = oldwarrantyPeriod.Id,
                        Duration = oldwarrantyPeriod.Duration

                    });

                    warrantyPeriod.Duration = model.Duration;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new WarrantyPeriod()
                    {
                        Id = warrantyPeriod.Id,
                        Duration = warrantyPeriod.Duration

                    });
                }

                warrantyPeriodService.SaveOrUpdate(warrantyPeriod);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "WarrantyPeriod",
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


            return RedirectToAction("Index", "WarrantyPeriod");
        }
    }
}