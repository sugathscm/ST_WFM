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
    public class SupplierController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly SupplierService supplierService = new SupplierService();
        private readonly SupplierTypeService supplierTypeService = new SupplierTypeService();

        public SupplierController()
        {
        }

        public SupplierController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Supplier Supplier = new Supplier();
            if (id != null)
            {
                Supplier = supplierService.GetSupplierById(id);
            }
            ViewBag.SupplierTypeList = new SelectList(supplierTypeService.GetSupplierTypeList(), "Id", "Type");
            return View(Supplier);
        }

        public ActionResult GetList()
        {
            List<Supplier> list = supplierService.GetSupplierList();

            List<SupplierViewModel> modelList = new List<SupplierViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new SupplierViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Address = item.Address,
                    ContactNo = item.ContactNo,
                    ContactPerson = item.ContactPerson,
                    SupplierType = (item.SupplierTypeId == 0 || item.SupplierTypeId == null) ? "" : item.SupplierType.Type,
               

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Design,Finance")]
        public ActionResult SaveOrUpdate(Supplier model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Supplier supplier = null;
                Supplier oldsupplier = null;
                if (model.Id == 0)
                {
                    supplier = new Supplier
                    {
                        Name = model.Name,
                        Address = model.Address,
                        ContactNo = model.ContactNo,
                        ContactPerson=model.ContactPerson,
                        SupplierTypeId =model.SupplierTypeId ,

                    };

                    oldsupplier = new Supplier();
                    oldData = new JavaScriptSerializer().Serialize(oldsupplier);
                    newData = new JavaScriptSerializer().Serialize(supplier);
                }
                else
                {
                    supplier = supplierService.GetSupplierById(model.Id);
                    oldsupplier = supplierService.GetSupplierById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Supplier()
                    {
                        Id = oldsupplier.Id,
                        Name = oldsupplier.Name,
                        Address = oldsupplier.Address,
                        ContactNo = oldsupplier.ContactNo,
                        ContactPerson = oldsupplier.ContactPerson,
                        SupplierTypeId = model.SupplierTypeId,

                    });

                    supplier.Name = model.Name;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new Supplier()
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        Address = supplier.Address,
                        ContactNo = supplier.ContactNo,
                        ContactPerson= supplier.ContactPerson,
                        SupplierTypeId = model.SupplierTypeId,  

                    });
                }

                supplierService.SaveOrUpdate(supplier);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Supplier",
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


            return RedirectToAction("Index", "Supplier");
        }
        [HttpPost]
        public JsonResult GetSupplierById(int? id)
        {
            var supplier = supplierService.GetSupplierById(id);

            return Json(new Supplier { Id = supplier.Id, Name = supplier.Name,Address=supplier.Address,ContactNo=supplier.ContactNo,ContactPerson=supplier.ContactPerson }, JsonRequestBehavior.AllowGet);
        }
    }
}