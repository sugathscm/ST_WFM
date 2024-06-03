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
            IlluminationService illuminationService = new IlluminationService();
            SizeService sizeService = new SizeService();
            FrameworkService frameworkService = new FrameworkService();
            LetteringService letteringService = new LetteringService();
            VisibilityService visibilityService = new VisibilityService();
            FasiaService fasiaService = new FasiaService(); 

            var IlluminationList = illuminationService.GetIlluminationList();
            var SizeList=sizeService.GetSizeList();
            var FrameworkList=frameworkService.GetFrameworkList();
            var LetteringList=letteringService.GetLetteringList();
            var VisibilityList=visibilityService.GetVisibilityList();   
            var FasiaList = fasiaService.GetFasiaList();
          
            ViewBag.IlluminationList = IlluminationList;
            ViewBag.SizeList = SizeList;
            ViewBag.FrameworkList = FrameworkList;
            ViewBag.LetteringList = LetteringList;  
            ViewBag.VisibilityList = VisibilityList;    
            ViewBag.FasiaList = FasiaList;



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

            List<CategoryViewModel> modelList = new List<CategoryViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new CategoryViewModel() {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    Description = item.Description,
                    CategoryType = item.CategoryType,
                    
                    
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales, Design,Finance")]
        public ActionResult SaveOrUpdate(Category model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Category category = null;
                Category oldCategory = null;
                if (model.Id == 0)
                {
                    category = new Category
                    {
                        Name = model.Name,
                        Description = model.Description,
                        IsActive = true,
                        CategoryType = model.CategoryType,
                       SizeId = model.SizeId,
                       FrameworkId = model.FrameworkId,
                       IlluminationId = model.IlluminationId,
                       LetteringId = model.LetteringId,
                       VisibilityId = model.VisibilityId,
                       FasiaId = model.FasiaId,

                    };

                    oldCategory = new Category();
                    oldData = new JavaScriptSerializer().Serialize(oldCategory);
                    newData = new JavaScriptSerializer().Serialize(category);
                }
                else
                {
                    category = categoryService.GetCategoryById(model.Id);
                    oldCategory = categoryService.GetCategoryById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Category()
                    {
                        Id = oldCategory.Id,
                        Name = oldCategory.Name,
                        Description = oldCategory.Description,
                        IsActive = oldCategory.IsActive,
                        CategoryType = oldCategory.CategoryType,
                        SizeId = model.SizeId,
                        FrameworkId = model.FrameworkId,
                        IlluminationId = model.IlluminationId,
                        LetteringId = model.LetteringId,
                        VisibilityId = model.VisibilityId,
                        FasiaId = model.FasiaId,
                    });

                    category.Name = model.Name;
                    category.Description = model.Description;
                    category.CategoryType = model.CategoryType;
                    category.SizeId = model.SizeId;
                    category.FrameworkId = model.FrameworkId;
                    category.IlluminationId = model.IlluminationId;
                    category.LetteringId = model.LetteringId;
                    category.VisibilityId = model.VisibilityId;
                    category.FasiaId = model.FasiaId;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    category.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Category()
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = model.Description,
                        IsActive = category.IsActive,
                        SizeId = model.SizeId,
                        FrameworkId = model.FrameworkId,
                        IlluminationId = model.IlluminationId,
                        LetteringId = model.LetteringId,
                        VisibilityId = model.VisibilityId,
                        FasiaId = model.FasiaId,
                    });
                }

                categoryService.SaveOrUpdate(category);

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

        [HttpPost]
        public JsonResult GetDescriptionById(int? id)
        {
            var category = categoryService.GetCategoryById(id);

            return Json(new Category { Id = category.Id, CategoryType = category.CategoryType, Description = category.Description }, JsonRequestBehavior.AllowGet);
        }

    }
}