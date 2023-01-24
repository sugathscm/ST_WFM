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
    public class IlluminationController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly IlluminationService illuminationService = new IlluminationService();

        public IlluminationController()
        {
        }

        public IlluminationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Illumination illumination = new Illumination();
            if (id != null)
            {
                illumination = illuminationService.GetIlluminationId(id);
            }
            return View(illumination);
        }

        public ActionResult GetList()
        {
            List<Illumination> list = illuminationService.GetIlluminationList();

            List<IlluminationViewModel> modelList = new List<IlluminationViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new IlluminationViewModel()
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
        public ActionResult SaveOrUpdate(Illumination model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Illumination illumination = null;
                Illumination oldillumination = null;
                if (model.Id == 0)
                {
                    illumination = new Illumination
                    {
                        Name = model.Name

                    };

                    oldillumination = new Illumination();
                    oldData = new JavaScriptSerializer().Serialize(oldillumination);
                    newData = new JavaScriptSerializer().Serialize(illumination);
                }
                else
                {
                    illumination = illuminationService.GetIlluminationId(model.Id);
                    oldillumination = illuminationService.GetIlluminationId(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Illumination()
                    {
                        Id = oldillumination.Id,
                        Name = oldillumination.Name

                    });

                    illumination.Name = model.Name;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new Illumination()
                    {
                        Id = illumination.Id,
                        Name = illumination.Name

                    });
                }

                illuminationService.SaveOrUpdate(illumination);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Illumination",
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


            return RedirectToAction("Index", "Illumination");
        }

    }
}