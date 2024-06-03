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
    public class MaterialController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly MaterialService materialService = new MaterialService();

        public MaterialController()
        {
        }

        public MaterialController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Material Material = new Material();
            if (id != null)
            {
                Material = materialService.GetMaterialById(id);
            }
            return View(Material);
        }

        public ActionResult GetList()
        {
            List<Material> list = materialService.GetMaterialList();

            List<MaterialViewModel> modelList = new List<MaterialViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new MaterialViewModel()
                {
                    Id = item.Id,
                    Material=item.Name
                    
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Design,Finance")]
        public ActionResult SaveOrUpdate(Material model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Material material = null;
                Material oldmaterial = null;
                if (model.Id == 0)
                {
                    material = new Material
                    {
                        Name = model.Name,
                        
                    };

                    oldmaterial = new Material();
                    oldData = new JavaScriptSerializer().Serialize(oldmaterial);
                    newData = new JavaScriptSerializer().Serialize(material);
                }
                else
                {
                    material = materialService.GetMaterialById(model.Id);
                    oldmaterial = materialService.GetMaterialById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Material()
                    {
                        Id = oldmaterial.Id,
                        Name = oldmaterial.Name,
                       
                    });

                    material.Name = model.Name;
                    
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    

                    newData = new JavaScriptSerializer().Serialize(new Material()
                    {
                        Id = material.Id,
                        Name = material.Name,
                      
                    });
                }

                materialService.SaveOrUpdate(material);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Material",
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


            return RedirectToAction("Index", "Material");
        }
        [HttpPost]
        public JsonResult GetMaterialById(int? id)
        {
            var material = materialService.GetMaterialById(id);

            return Json(new Material { Id = material.Id,Name=material.Name }, JsonRequestBehavior.AllowGet);
        }
    }
}