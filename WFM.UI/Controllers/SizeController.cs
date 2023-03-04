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
    public class SizeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly SizeService sizeService = new SizeService();

        public SizeController()
        {
        }

        public SizeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            Size Size = new Size();
            if (id != null)
            {
                Size = sizeService.GetSizeById(id);
            }
            return View(Size);
        }

        public ActionResult GetList()
        {
            List<Size> list = sizeService.GetSizeList();

            List<SizeViewModel> modelList = new List<SizeViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new SizeViewModel()
                {
                    Id = item.Id,
                    Size=item.Size1
                    
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Design")]
        public ActionResult SaveOrUpdate(Size model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Size size = null;
                Size oldsize = null;
                if (model.Id == 0)
                {
                    size = new Size
                    {
                        Size1 = model.Size1,
                        
                    };

                    oldsize = new Size();
                    oldData = new JavaScriptSerializer().Serialize(oldsize);
                    newData = new JavaScriptSerializer().Serialize(size);
                }
                else
                {
                    size = sizeService.GetSizeById(model.Id);
                    oldsize = sizeService.GetSizeById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Size()
                    {
                        Id = oldsize.Id,
                        Size1 = oldsize.Size1,
                       
                    });

                    size.Size1 = model.Size1;
                    
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    

                    newData = new JavaScriptSerializer().Serialize(new Size()
                    {
                        Id = size.Id,
                        Size1 = size.Size1,
                      
                    });
                }

                sizeService.SaveOrUpdate(size);

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


            return RedirectToAction("Index", "Size");
        }
        [HttpPost]
        public JsonResult GetsizeById(int? id)
        {
            var size = sizeService.GetSizeById(id);

            return Json(new Size { Id = size.Id,Size1=size.Size1 }, JsonRequestBehavior.AllowGet);
        }
    }
}