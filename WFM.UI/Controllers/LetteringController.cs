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
    public class LetteringController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly LetteringService letteringService = new LetteringService();

        public LetteringController()
        {
        }

        public LetteringController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Lettering Lettering = new Lettering();
            if (id != null)
            {
                Lettering = letteringService.GetLetteringById(id);
            }
            return View(Lettering);
        }

        public ActionResult GetList()
        {
            List<Lettering> list = letteringService.GetLetteringList();

            List<LetteringViewModel> modelList = new List<LetteringViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new LetteringViewModel()
                {
                    Id = item.Id,
                    Name = item.Lettering1

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Lettering model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Lettering lettering = null;
                Lettering oldlettering = null;
                if (model.Id == 0)
                {
                    lettering = new Lettering
                    {
                        Lettering1 = model.Lettering1,

                    };

                    oldlettering = new Lettering();
                    oldData = new JavaScriptSerializer().Serialize(oldlettering);
                    newData = new JavaScriptSerializer().Serialize(lettering);
                }
                else
                {
                    lettering = letteringService.GetLetteringById(model.Id);
                    oldlettering = letteringService.GetLetteringById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Lettering()
                    {
                        Id = oldlettering.Id,
                        Lettering1 = oldlettering.Lettering1,

                    });

                    lettering.Lettering1 = model.Lettering1;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new Lettering()
                    {
                        Id = lettering.Id,
                        Lettering1 = lettering.Lettering1,

                    });
                }

                letteringService.SaveOrUpdate(lettering);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "lettering",
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


            return RedirectToAction("Index", "Lettering");
        }
        [HttpPost]
        public JsonResult GetletteringById(int? id)
        {
            var lettering = letteringService.GetLetteringById(id);

            return Json(new Lettering { Id = lettering.Id, Lettering1 = lettering.Lettering1 }, JsonRequestBehavior.AllowGet);
        }
    }
}