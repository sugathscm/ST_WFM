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
    public class VisibilityController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly VisibilityService visibilityservice = new VisibilityService();

        public VisibilityController()
        {
        }

        public VisibilityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Visibility visibility = new Visibility();
            if (id != null)
            {
                visibility = visibilityservice.GetVisibilityId(id);
            }
            return View(visibility);
        }

        public ActionResult GetList()
        {
            List<Visibility> list = visibilityservice.GetVisibilityList();

            List<VisibilityViewModel> modelList = new List<VisibilityViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new VisibilityViewModel()
                {
                    Id = item.Id,                  
                    Name = item.Name
                  
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales")]
        public ActionResult SaveOrUpdate(Visibility model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Visibility visibility = null;
                Visibility oldVisibility = null;
                if (model.Id == 0)
                {
                    visibility = new Visibility
                    {
                        Name = model.Name
                       
                    };

                    oldVisibility = new Visibility();
                    oldData = new JavaScriptSerializer().Serialize(oldVisibility);
                    newData = new JavaScriptSerializer().Serialize(visibility);
                }
                else
                {
                    visibility = visibilityservice.GetVisibilityId(model.Id);
                    oldVisibility = visibilityservice.GetVisibilityId(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Visibility()
                    {
                        Id = oldVisibility.Id,
                        Name = oldVisibility.Name
                       
                    });

                    visibility.Name = model.Name;
                  
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                   

                    newData = new JavaScriptSerializer().Serialize(new Visibility()
                    {
                        Id = visibility.Id,
                        Name = visibility.Name
                       
                    });
                }

                visibilityservice.SaveOrUpdate(visibility);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Visibility",
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


            return RedirectToAction("Index", "Visibility");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Management,Sales,Design")]
        public JsonResult GetDescriptionById(int? id)
        {
            var visibility = visibilityservice.GetVisibilityId(id);

            return Json(new Category { Id = visibility.Id, Name = visibility.Name }, JsonRequestBehavior.AllowGet);
        }

    }
}