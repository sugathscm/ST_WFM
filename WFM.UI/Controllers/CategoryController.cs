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
    public class CategoryController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly CategoryService categoryService = new CategoryService();

        public CategoryController()
        {
        }

        public CategoryController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Category
        public ActionResult Index(int? id)
        {
            Category Category = new Category();
            if (id != null)
            {
                Category = categoryService.GetCategoryById(id);
            }
            return View(Category);
        }

        public ActionResult GetList()
        {
            List<Category> list = categoryService.GetCategoryList();

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
        public ActionResult SaveOrUpdate(Category model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Category Category = null;
                Category oldCategory = null;
                if (model.Id == 0)
                {
                    Category = new Category
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldCategory = new Category();
                    oldData = new JavaScriptSerializer().Serialize(oldCategory);
                    newData = new JavaScriptSerializer().Serialize(Category);
                }
                else
                {
                    Category = categoryService.GetCategoryById(model.Id);
                    oldCategory = categoryService.GetCategoryById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Category()
                    {
                        Id = oldCategory.Id,
                        Name = oldCategory.Name,
                        IsActive = oldCategory.IsActive
                    });

                    Category.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    Category.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Category()
                    {
                        Id = Category.Id,
                        Name = Category.Name,
                        IsActive = Category.IsActive
                    });
                }

                categoryService.SaveOrUpdate(Category);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Category",
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


            return RedirectToAction("Index", "Category");
        }
    }
}