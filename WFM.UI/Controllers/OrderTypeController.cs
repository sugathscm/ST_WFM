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
    public class OrderTypeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly OrderTypeService orderTypeService = new OrderTypeService();

        public OrderTypeController()
        {
        }

        public OrderTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: OrderType

        public ActionResult Index(int? id)
        {
            OrderType orderType = new OrderType();
            if (id != null)
            {
                orderType = orderTypeService.GetOrderTypeById(id);
            }
            return View(orderType);
        }

        public ActionResult GetList()
        {
            List<OrderType> list = orderTypeService.GetOrderTypeList();

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
        public ActionResult SaveOrUpdate(OrderType model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                OrderType orderType = null;
                OrderType oldOrderType = null;
                if (model.Id == 0)
                {
                    orderType = new OrderType
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldOrderType = new OrderType();
                    oldData = new JavaScriptSerializer().Serialize(oldOrderType);
                    newData = new JavaScriptSerializer().Serialize(orderType);
                }
                else
                {
                    orderType = orderTypeService.GetOrderTypeById(model.Id);
                    oldOrderType = orderTypeService.GetOrderTypeById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new OrderType()
                    {
                        Id = oldOrderType.Id,
                        Name = oldOrderType.Name,
                        IsActive = oldOrderType.IsActive
                    });

                    orderType.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    orderType.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new OrderType()
                    {
                        Id = orderType.Id,
                        Name = orderType.Name,
                        IsActive = orderType.IsActive
                    });
                }

                orderTypeService.SaveOrUpdate(orderType);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "OrderType",
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

            return RedirectToAction("Index", "OrderType");
        }
    }
}