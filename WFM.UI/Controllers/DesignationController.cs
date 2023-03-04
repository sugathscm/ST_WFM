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
    public class DesignationController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly DesignationService designationService = new DesignationService();

        public DesignationController()
        {
        }

        public DesignationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Designation
        public ActionResult Index(int? id)
        {
            Designation designation = new Designation();
            if (id != null)
            {
                designation = designationService.GetDesignationById(id);
            }
            return View(designation);
        }
        [Authorize(Roles = "Administrator,Management,Sales,Design,Factory")]
        public ActionResult GetList()
        {
            List<Designation> list = designationService.GetDesignationList();

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
        [Authorize(Roles = "Administrator,Management,Sales")]
        public ActionResult SaveOrUpdate(Designation model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Designation designation = null;
                Designation oldDesignation = null;
                if (model.Id == 0)
                {
                    designation = new Designation
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldDesignation = new Designation();
                    oldData = new JavaScriptSerializer().Serialize(oldDesignation);
                    newData = new JavaScriptSerializer().Serialize(designation);
                }
                else
                {
                    designation = designationService.GetDesignationById(model.Id);
                    oldDesignation = designationService.GetDesignationById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Designation()
                    {
                        Id = oldDesignation.Id,
                        Name = oldDesignation.Name,
                        IsActive = oldDesignation.IsActive
                    });

                    designation.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    designation.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Designation()
                    {
                        Id = designation.Id,
                        Name = designation.Name,
                        IsActive = designation.IsActive
                    });
                }

                designationService.SaveOrUpdate(designation);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Designation",
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


            return RedirectToAction("Index", "Designation");
        }
    }
}