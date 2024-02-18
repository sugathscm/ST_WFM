using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;
using System.Web.Configuration;
using WFM.BAL.Helpers;

namespace WFM.UI.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly OrderService orderService = new OrderService();
        private readonly InvoiceService invoiceService = new InvoiceService();
        private ApplicationUserManager _userManager;
        public InvoiceController()
        {

        }
        public InvoiceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Management,Sales,Factory,Design,Department")]
        public ActionResult GetList()
        {
            var list = invoiceService.GetInvoiceList();

            List<InvoiceView> modelList = new List<InvoiceView>();

            foreach (var item in list)
            {
                try { 
                modelList.Add(new InvoiceView(invoiceService)
                {
                    Id= item.Id,
                    Code = item.Code,
                    OrderCode = item.Order.Code,
                    ClientName=item.Order.Client.Name,
                    CreatedDateString=item.CreatedDate.ToString(),
                    Channelledby=item.Order.Employee.Name,
                 

                });
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateInvoice(int id)
        {
            Order order = orderService.GetOrderById(id);
            if (invoiceService.InvoiceExists(order.Id))
            {
                return RedirectToAction("Index", "Invoice");
            }

            String invoiceCode = CommonService.GenerateInvoiceCodeSave(order);
            var utcTime = DateTime.Now.ToUniversalTime();
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

            Invoice invoice = new Invoice
            {
                Code = invoiceCode,
                CodeNumber= int.Parse(invoiceCode.Split('-')[2]),
                OrderId = order.Id,
                CreatedDate= today,

            };
            
            
            invoiceService.SaveOrUpdate(invoice);
            return RedirectToAction("Index", "Invoice");
        }

        public ActionResult PrintInvoice(int id)
        {
            Invoice invoice = invoiceService.GetInvoiceById(id);
            OrderService orderService = new OrderService();
            InvoiceView invoiceView = new InvoiceView(invoiceService);
            Order order = orderService.GetOrderById(id);


            PropertyCopier<Invoice, InvoiceView>.Copy(invoice, invoiceView);
            ViewBag.VATNo = ResourceData.VATNo;
            ViewBag.Name = ResourceData.Name;
            ViewBag.AuthorizedPersonName = ResourceData.AuthorizedPersonName;
            ViewBag.AuthorizedPersonDesignation = ResourceData.AuthorizedPersonDesignation;
            invoiceView.OrderType = invoice.Order.OrderType.Name.ToString();   
            invoiceView.ClientName = invoice.Order.Client.Name.ToString();
            invoiceView.CreatedDateString = invoice.CreatedDate.Value.ToString("dd/MM/yyyy");
            invoiceView.ClientAddress = invoice.Order.Client.AddressLine1.ToString();
            invoiceView.VatNo=invoice.Order.VATNo.ToString();
            invoiceView.ClientVatNo=invoice.Order.Client.VATNumber ==null ?"": invoice.Order.Client.VATNumber.ToString();
            invoiceView.Items=invoice.Order.OrderItems.ToList();
            invoiceView.AdvancePayment = invoice.Order.AdvancePayment;
            //invoiceView.isVat = invoice.Order.OrderType.Name =="" ? true : false;
            //orderView.CreatedDateString = orderView.CreatedDate.Value.ToString("dd/MM/yyyy");
            invoiceView.orderTotal = CalculateOrdItmTotal(order) + CalculateAdnChargeTotal(order);
            invoiceView.vatAmount = CalculateOrdItmVatTotal(order);         
            // orderView.OrderTermDetails = orderService.get.GetQuoteTermsByQuoteId(quote.Id);
            invoiceView.BalancePayment = (double)((invoiceView.orderTotal + invoiceView.vatAmount) - (invoiceView.AdvancePayment != null ? invoiceView.AdvancePayment : 0));
            invoiceView.TotalWithVat = (double)(invoiceView.orderTotal +    invoiceView.vatAmount);

            return PartialView("_PrintInvoice", invoiceView);
        }
        private double CalculateOrdItmTotal(Order order)
        {
            double? TotalCost = order.OrderItems.Sum(x => x.Qty * x.UnitCost);
            return TotalCost.HasValue ? TotalCost.Value : 0;
        }

        private double CalculateAdnChargeTotal(Order order)
        {
            double? TotalCost = order.AdditionalCharges.Sum(x => x.Qty * x.Cost);
            return TotalCost.HasValue ? TotalCost.Value : 0;
        }

        private double CalculateOrdItmVatTotal(Order order)
        {
            double? TotalVat = 0;
            //double? Cost = order.OrderItems.Sum(x => x.Qty * x.UnitCost);
            if (order.OrderTypeId == 1)
                TotalVat = order.OrderItems.Sum(x => ((x.Qty * x.UnitCost) / 100) * order.VatPercentage);
            return TotalVat.HasValue ? TotalVat.Value : 0;
        }


    }
}