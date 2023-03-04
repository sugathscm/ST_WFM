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
    public class DepartmentController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly DivisionService divisionService = new DivisionService();

        public DepartmentController()
        {
        }

        public DepartmentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Division
        public ActionResult Index(int? id)
        {
            Division Division = new Division();
            if (id != null)
            {
                Division = divisionService.GetDivisionById(id);
            }
            return View(Division);
        }

        public ActionResult GetList()
        {
            List<Division> list = divisionService.GetDivisionList();

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
        [Authorize(Roles = "Administrator,Management,Sales,Design")]
        public ActionResult SaveOrUpdate(Division model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Division Division = null;
                Division oldDivision = null;
                if (model.Id == 0)
                {
                    Division = new Division
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldDivision = new Division();
                    oldData = new JavaScriptSerializer().Serialize(oldDivision);
                    newData = new JavaScriptSerializer().Serialize(Division);
                }
                else
                {
                    Division = divisionService.GetDivisionById(model.Id);
                    oldDivision = divisionService.GetDivisionById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Division()
                    {
                        Id = oldDivision.Id,
                        Name = oldDivision.Name,
                        IsActive = oldDivision.IsActive
                    });

                    Division.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    Division.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Division()
                    {
                        Id = Division.Id,
                        Name = Division.Name,
                        IsActive = Division.IsActive
                    });
                }

                divisionService.SaveOrUpdate(Division);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Division",
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


            return RedirectToAction("Index", "Division");
        }
    }
}