using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WFM.BAL;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;

namespace WFM.UI.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly OrderService orderService = new OrderService();
        private readonly QuoteService quoteService = new QuoteService();
        private readonly ClientService clientService = new ClientService();
        private readonly EmployeeService employeeService = new EmployeeService();

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

        // GET: Order
        public ActionResult Details(int? id)
        {
            Order order = new Order();

            if (id != null)
            {
                order = orderService.GetOrderById(id);
            }

            var quoteList = quoteService.GetQuoteList().Where(q => q.ApprovedBy != null).ToList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();

            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");

            return View(order);
        }

        public ActionResult GetList()
        {

            var list = orderService.GetOrderFullList();
            List<OrderView> modelList = new List<OrderView>();
            foreach (var item in list)
            {
                modelList.Add(new OrderView()
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    StatusName = item.Status.Name,
                    Code = item.Code
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Order model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Order order = null;
                Order oldOrder = null;
                if (model.Id == 0)
                {
                    order = new Order
                    {
                        ClientId = model.ClientId
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

                    newData = new JavaScriptSerializer().Serialize(new Order()
                    {
                        Id = order.Id
                    });
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
    }
}