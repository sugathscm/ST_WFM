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
    public class FrameworkController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly FrameworkService frameworkService = new FrameworkService();

        public FrameworkController()
        {
        }

        public FrameworkController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Framework Framework = new Framework();
            if (id != null)
            {
                Framework = frameworkService.GetFrameworkById(id);
            }
            return View(Framework);
        }
        public ActionResult GetList()
        {
            List<Framework> list = frameworkService.GetFrameworkList();

            List<FrameworkViewModel> modelList = new List<FrameworkViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new FrameworkViewModel()
                {
                    Id = item.Id,
                    Framework = item.Framework1

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Framework model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Framework framework = null;
                Framework oldframework = null;
                if (model.Id == 0)
                {
                    framework = new Framework
                    {
                        Framework1 = model.Framework1,

                    };

                    oldframework = new Framework();
                    oldData = new JavaScriptSerializer().Serialize(oldframework);
                    newData = new JavaScriptSerializer().Serialize(framework);
                }
                else
                {
                    framework = frameworkService.GetFrameworkById(model.Id);
                    oldframework = frameworkService.GetFrameworkById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Framework()
                    {
                        Id = oldframework.Id,
                         Framework1= oldframework.Framework1,

                    });

                    framework.Framework1 = model.Framework1;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new Framework()
                    {
                        Id = framework.Id,
                        Framework1 = framework.Framework1,

                    });
                }

                frameworkService.SaveOrUpdate(framework);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Framework",
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


            return RedirectToAction("Index", "Framework");
        }
        [HttpPost]
        public JsonResult GetframeworkById(int? id)
        {
            var framework = frameworkService.GetFrameworkById(id);

            return Json(new Framework { Id = framework.Id, Framework1 = framework.Framework1 }, JsonRequestBehavior.AllowGet);
        }
    }
}