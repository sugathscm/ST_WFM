using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;
namespace WFM.UI.Controllers
{
    public class FasiaController : Controller
    {

        private ApplicationUserManager _userManager;
        private readonly FasiaService fasiaService = new FasiaService();

        public FasiaController()
        {
        }

        public FasiaController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Fasia Fasia = new Fasia();
            if (id != null)
            {
                Fasia = fasiaService.GetFasiaById(id);
            }
            return View(Fasia);
        }
        public ActionResult GetList()
        {
            List<Fasia> list = fasiaService.GetFasiaList();

            List<FasiaViewModel> modelList = new List<FasiaViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new FasiaViewModel()
                {
                    Id = item.Id,
                    Fasia =item.Facia

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Fasia model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Fasia fasia = null;
                Fasia oldfasia = null;
                if (model.Id == 0)
                {
                    fasia = new Fasia
                    {
                        Facia = model.Facia,

                    };

                    oldfasia = new Fasia();
                    oldData = new JavaScriptSerializer().Serialize(oldfasia);
                    newData = new JavaScriptSerializer().Serialize(fasia);
                }
                else
                {
                    fasia = fasiaService.GetFasiaById(model.Id);
                    oldfasia = fasiaService.GetFasiaById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Fasia()
                    {
                        Id = oldfasia.Id,
                       Facia = oldfasia.Facia,

                    });

                    fasia.Facia = model.Facia;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new Fasia()
                    {
                        Id = fasia.Id,
                        Facia = fasia.Facia,

                    });
                }

                fasiaService.SaveOrUpdate(fasia);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Size",
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


            return RedirectToAction("Index", "Fasia");
        }

         [HttpPost]
        public JsonResult GetfasiaById(int? id)
        {
            var fasia = fasiaService.GetFasiaById(id);

            return Json(new Fasia { Id = fasia.Id,Facia=fasia.Facia }, JsonRequestBehavior.AllowGet);
        }

    }
}