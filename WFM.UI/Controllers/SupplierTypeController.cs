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
    public class SupplierTypeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly SupplierTypeService supplierTypeService = new SupplierTypeService();

        public SupplierTypeController()
        {
        }

        public SupplierTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: SupplierType
        public ActionResult Index(int? id)
        {
            SupplierType supplierType = new SupplierType();
            if (id != null)
            {
                supplierType = supplierTypeService.GetSupplierTypeById(id);
            }
            return View(supplierType);
        }
        [Authorize(Roles = "Administrator,Management,Sales,Design,Factory")]
        public ActionResult GetList()
        {
            List<SupplierType> list = supplierTypeService.GetSupplierTypeList();

            List<BaseViewModel> modelList = new List<BaseViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new SupplierTypeViewModel() { Id = item.Id, Type=item.Type });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales")]
        public ActionResult SaveOrUpdate(SupplierType model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                SupplierType supplierType = null;
                SupplierType oldsupplierType = null;
                if (model.Id == 0)
                {
                    supplierType = new SupplierType
                    {
                        Type = model.Type,
                        
                    };

                    oldsupplierType = new SupplierType();
                    oldData = new JavaScriptSerializer().Serialize(oldsupplierType);
                    newData = new JavaScriptSerializer().Serialize(supplierType);
                }
                else
                {
                    supplierType = supplierTypeService.GetSupplierTypeById(model.Id);
                    oldsupplierType = supplierTypeService.GetSupplierTypeById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new SupplierType()
                    {
                        Id = oldsupplierType.Id,
                        Type = oldsupplierType.Type,
                       
                    });

                    supplierType.Type = model.Type;
                   
                    

                    newData = new JavaScriptSerializer().Serialize(new SupplierType()
                    {
                        Id = supplierType.Id,
                        Type = supplierType.Type,
                        
                    });
                }

                supplierTypeService.SaveOrUpdate(supplierType);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "SupplierType",
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


            return RedirectToAction("Index", "SupplierType");
        }
    }
}