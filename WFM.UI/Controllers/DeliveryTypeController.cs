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
    public class DeliveryTypeController :Controller
    {
        private ApplicationUserManager _userManager;
        private readonly DeliveryTypeService deliveryTypeService = new DeliveryTypeService();

        public DeliveryTypeController()
        {
        }

        public DeliveryTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            DeliveryType DeliveryType = new DeliveryType();
            if (id != null)
            {
                DeliveryType = deliveryTypeService.GetDeliveryTypeById(id);
            }
            return View(DeliveryType);
        }
        public ActionResult GetList()
        {
            List<DeliveryType> list = deliveryTypeService.GetDeliveryTypeList();

            List<DeliveryTypeViewModel> modelList = new List<DeliveryTypeViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new DeliveryTypeViewModel()
                {
                    Id = item.Id,
                    Type = item.Type

                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Design")]
        public ActionResult SaveOrUpdate(DeliveryType model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                DeliveryType deliveryType = null;
                DeliveryType olddeliveryType = null;
                if (model.Id == 0)
                {
                    deliveryType = new DeliveryType
                    {
                        Type = model.Type,

                    };

                    olddeliveryType = new DeliveryType();
                    oldData = new JavaScriptSerializer().Serialize(olddeliveryType);
                    newData = new JavaScriptSerializer().Serialize(deliveryType);
                }
                else
                {
                    deliveryType = deliveryTypeService.GetDeliveryTypeById(model.Id);
                    olddeliveryType = deliveryTypeService.GetDeliveryTypeById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new DeliveryType()
                    {
                        Id = olddeliveryType.Id,
                        Type = olddeliveryType.Type,

                    });

                    deliveryType.Type = model.Type;

                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);


                    newData = new JavaScriptSerializer().Serialize(new DeliveryType()
                    {
                        Id = deliveryType.Id,
                        Type = deliveryType.Type,

                    });
                }

                deliveryTypeService.SaveOrUpdate(deliveryType);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "DeliveryType",
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


            return RedirectToAction("Index", "DeliveryType");
        }
        [HttpPost]
        public JsonResult GetDeliveryTypeById(int? id)
        {
            var deliveryType = deliveryTypeService.GetDeliveryTypeById(id);

            return Json(new DeliveryType { Id = deliveryType.Id, Type = deliveryType.Type }, JsonRequestBehavior.AllowGet);
        }

    }
}