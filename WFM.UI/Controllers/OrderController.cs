using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using System.Web.Configuration;

namespace WFM.UI.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly OrderService orderService = new OrderService();
        private readonly QuoteService quoteService = new QuoteService();
        private readonly ClientService clientService = new ClientService();
        private readonly EmployeeService employeeService = new EmployeeService();
        private readonly CategoryService categoryService = new CategoryService();
        private readonly QuoteTermService quoteTermService = new QuoteTermService();
        private readonly WarrantyPeriodService warrantyPeriodService = new WarrantyPeriodService();

        public OrderController()
        {

        }
        public OrderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }

        // GET: Order
        public ActionResult Details(int? id)
        {
            Order order = new Order();

            if (id != null)
            {
                order = orderService.GetOrderById(id);
            }
            else
            {
                //var code = CommonService.GenerateOrderCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), false).Replace('-', '/');
                //order.Code = code;
            }

            var quoteList = quoteService.GetQuoteList().Where(q => q.IsConverted == false).ToList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();
            var orderTermList = quoteTermService.GetQuoteTermList();

            ViewBag.OrderTermList = orderTermList;
            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");
            ViewBag.WarrantyPeriodList = new SelectList(warrantyPeriodService.GetWarrantyPeriodList(), "Id", "Name");
            ViewBag.CategoryList = categoryService.GetCategoryList();
            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];
            return View(order);
        }

        public ActionResult GetHistoryList()
        {
            var list = orderService.GetOrderHistoryList();

            List<OrderView> modelList = new List<OrderView>();

            foreach (var item in list)
            {
                modelList.Add(new OrderView()
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    Code = item.Code,
                    Value = item.Value,
                    CreatedDate = item.CreatedDate,
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    Header = item.Header
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            var list = orderService.GetOrderActiveList();

            List<OrderView> modelList = new List<OrderView>();

            foreach (var item in list)
            {
                modelList.Add(new OrderView()
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    Code = item.Code,
                    Value = item.Value,
                    CreatedDate = item.CreatedDate,
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    Header = item.Header
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Order model, FormCollection formCollection)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                var productIdArray = formCollection["productIdArray"].Split(',');
                var qtyArray = formCollection["qtyArray"].Split(',');
                var descriptionArray = formCollection["descriptionArray"].Split(',');
                var costArray = formCollection["costArray"].Split(',');
                var vatArray = formCollection["vatArray"].Split(',');
                var sizeArray = formCollection["sizeArray"].Split(',');

                int id = model.Id;
                Order order = null;
                Order oldOrder = null;
                if (model.Id == 0)
                {
                    order = new Order
                    {
                        ClientId = model.ClientId,
                        Code = model.Code,
                        CodeNumber = int.Parse(model.Code.Split('/')[3]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        ChanneledBy = model.ChanneledBy,
                        Value = model.Value,
                        Comments = model.Comments,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId(),
                        Header = model.Header,
                        IsVAT = model.IsVAT,
                        WarrantyPeriodId = model.WarrantyPeriodId,
                        ContactPerson = model.ContactPerson,
                        ContactMobile = model.ContactMobile,
                        FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod,
                        IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod,
                        LetteringWarrantyPeriod = model.LetteringWarrantyPeriod
                    };

                    oldOrder = new Order();
                    oldData = new JavaScriptSerializer().Serialize(oldOrder);
                    newData = new JavaScriptSerializer().Serialize(order);
                }
                else
                {
                    order = orderService.GetOrderById(model.Id);
                    oldOrder = orderService.GetOrderById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Order()
                    {
                        Id = oldOrder.Id
                    });

                    order.ClientId = model.ClientId;
                    order.ChanneledBy = model.ChanneledBy;
                    order.Value = model.Value;
                    order.Header = model.Header;
                    order.Comments = model.Comments;
                    order.UpdatedBy = User.Identity.GetUserId();
                    order.UpdatedDate = DateTime.Now;
                    order.ContactPerson = model.ContactPerson;
                    order.ContactMobile = model.ContactMobile;
                    order.FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod;
                    order.IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod;
                    order.LetteringWarrantyPeriod = model.LetteringWarrantyPeriod;

                    newData = new JavaScriptSerializer().Serialize(new Order()
                    {
                        Id = order.Id
                    });

                    orderService.RemoveItems(order);
                }

                int i = 0;
                order.OrderItems.Clear();
                OrderItem orderItem = null;

                foreach (var item in productIdArray)
                {
                    if (model.Id == 0)
                    {
                        orderItem = new OrderItem
                        {
                            CategoryId = int.Parse(item),
                            Qty = (qtyArray[i] == "") ? 0 : double.Parse(qtyArray[i]),
                            UnitCost = (costArray[i] == "") ? 0 : double.Parse(costArray[i]),
                            VAT = (vatArray[i] == "") ? 0 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i]
                        };
                        order.OrderItems.Add(orderItem);
                    }
                    else
                    {
                        orderItem = new OrderItem
                        {
                            CategoryId = int.Parse(item),
                            Qty = (qtyArray[i] == "") ? 0 : double.Parse(qtyArray[i]),
                            UnitCost = (costArray[i] == "") ? 0 : double.Parse(costArray[i]),
                            VAT = (vatArray[i] == "") ? 0 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i],
                            OrderId = model.Id
                        };

                        orderService.SaveOrUpdate(orderItem);
                    }

                    i++;
                }

                orderService.SaveOrUpdate(order);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Order",
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

            return RedirectToAction("Index", "Order");
        }

        public PartialViewResult _NewClientPartial()
        {
            return PartialView();
        }

        public ActionResult PrintOrder(int id)
        {
            Order Order = orderService.GetOrderById(id);

            return PartialView("_PrintOrder", Order);
        }
    }
}