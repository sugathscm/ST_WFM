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
using Aspose.Words;
using System.Globalization;

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

        [Authorize(Roles = "Administrator,Management,Sales,Factory,Design,Department,Finance")]
        public ActionResult GetList()
        {
            var list = invoiceService.GetInvoiceList();

            List<InvoiceView> modelList = new List<InvoiceView>();

            foreach (var item in list)
            {
                try
                {
                    modelList.Add(new InvoiceView(invoiceService)
                    {
                        Id = item.Id,
                        Code = item.Code,
                        OrderCode = item.Order.Code,
                        ClientName = item.Order.Client.Name,
                        CreatedDateString = item.CreatedDate.Value.ToString("dd/MM/yyyy"),
                        Channelledby = item.Order.Employee.Name,


                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Management,Sales,Factory,Design,Department,Finance")]
        public ActionResult GetFilteredList(string fromDate, string toDate, string client, string salesPerson)
        {
            var list = invoiceService.GetInvoiceList();

            try
            {
                // Apply filters
                if (!string.IsNullOrEmpty(fromDate))
                {
                    // Convert fromDate string to DateTime
                    var fromDateValue = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    list = list.Where(o => o.CreatedDate >= fromDateValue).ToList();
                }

                if (!string.IsNullOrEmpty(toDate))
                {
                    // Convert toDate string to DateTime
                    var toDateValue = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    list = list.Where(o => o.CreatedDate <= toDateValue).ToList();
                }

                if (!string.IsNullOrEmpty(client))
                {
                    list = list.Where(o => o.Order.Client.Name.ToLower().Contains(client.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(salesPerson))
                {
                    list = list.Where(o => o.Order.Employee.Name.ToLower().Contains(salesPerson.ToLower())).ToList();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            List<InvoiceView> modelList = new List<InvoiceView>();

            foreach (var item in list)
            {
                try
                {
                    var addcharge = CalculateAdnChargeTotal(item.Order);
                    var vatTot = CalculateOrdItmVatTotal(item.Order.OrderItems.ToList(), (double)item.Order.VatPercentage, item.Order.OrderType.Name);
                    var Total = CalculateOrdItmTotal(item.Order.OrderItems.ToList()) +addcharge+ vatTot;

                    modelList.Add(new InvoiceView(invoiceService)
                    {
                        Id = item.Id,
                        Code = item.Code,
                        OrderCode = item.Order.Code,
                        ClientName = item.Order.Client.Name,
                        CreatedDateString = item.CreatedDate.Value.ToString("dd/MM/yyyy"),
                        Channelledby = item.Order.Employee.Name,
                        Location=item.Order.Location,
                        TotalWithVat = Total

                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateInvoice(int id)
        {
            Order order = orderService.GetOrderById(id);
            // var exist = invoiceService.InvoiceExists(order.Id);
            if (invoiceService.InvoiceExists(order.Id))
            {
                return Json(new { redirectUrl = Url.Action("Index", "Invoice") }, JsonRequestBehavior.AllowGet);
            }
            if (order.OrderType.Name == "STC" || order.OrderType.Name == "STR")
            {
                return Json(new { error = "Order type cannot be invoiced." }, JsonRequestBehavior.AllowGet);
            }
            String invoiceCode = CommonService.GenerateInvoiceCodeSave(order);
            var utcTime = DateTime.Now.ToUniversalTime();
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

            Invoice invoice = new Invoice
            {
                Code = invoiceCode,
                CodeNumber = int.Parse(invoiceCode.Split('-')[2]),
                OrderId = order.Id,
                CreatedDate = today,

            };


            invoiceService.SaveOrUpdate(invoice);
            return Json(new { redirectUrl = Url.Action("Index", "Invoice") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintInvoice(int id)
        {
            Invoice invoice = invoiceService.GetInvoiceById(id);
            //OrderService orderService = new OrderService();
            InvoiceView invoiceView = new InvoiceView(invoiceService);
            // Order order = orderService.GetOrderById(id);


            PropertyCopier<Invoice, InvoiceView>.Copy(invoice, invoiceView);

            ViewBag.Name = ResourceData.Name;
            ViewBag.AuthorizedPersonName = ResourceData.AuthorizedPersonName;
            ViewBag.AuthorizedPersonDesignation = ResourceData.AuthorizedPersonDesignation;
            invoiceView.OrderType = invoice.Order.OrderType.Name.ToString();
            invoiceView.OrderCode = invoice.Order.Code.ToString();
            invoiceView.ClientSVatNo = invoice.Order.Client.SVATNumber?.ToString() ?? "";
            invoiceView.ClientPoNo = invoice.Order.PONumber?.ToString() ?? "";
            invoiceView.ClientName = invoice.Order.Client.Name?.ToString() ?? "";
            invoiceView.VatNo = invoice.Order.VATNo.ToString();
            invoiceView.CreatedDateString = invoice.CreatedDate.Value.ToString("dd/MM/yyyy");
            invoiceView.ClientAddress = invoice.Order.Client.AddressLine1?.ToString() ?? "";
            invoiceView.ClientAddress2 = invoice.Order.Client.AddressLine2?.ToString() ?? "";
            invoiceView.VatNo = invoice.Order.VATNo.ToString();
            invoiceView.ClientVatNo = invoice.Order.Client.VATNumber == null ? "" : invoice.Order.Client.VATNumber.ToString();
            invoiceView.Items = invoice.Order.OrderItems.ToList();
            invoiceView.additionalCharges = invoice.Order.AdditionalCharges.ToList();
            invoiceView.AdvancePayment = invoice.Order.AdvancePayment;
            //invoiceView.isVat = invoice.Order.OrderType.Name =="" ? true : false;
            //orderView.CreatedDateString = orderView.CreatedDate.Value.ToString("dd/MM/yyyy");
            invoiceView.orderTotal = CalculateOrdItmTotal(invoiceView.Items) + CalculateAdnChargeTotal(invoice.Order);
            invoiceView.vatAmount = CalculateOrdItmVatTotal(invoiceView.Items, (double)invoiceView.Order.VatPercentage, invoiceView.OrderType);
            // orderView.OrderTermDetails = orderService.get.GetQuoteTermsByQuoteId(quote.Id);
            invoiceView.BalancePayment = (double)((invoiceView.orderTotal + invoiceView.vatAmount) - (invoiceView.AdvancePayment != null ? invoiceView.AdvancePayment : 0));
            invoiceView.TotalWithVat = (double)(invoiceView.orderTotal + invoiceView.vatAmount);

            return PartialView("_PrintInvoice", invoiceView);
        }
        private double CalculateOrdItmTotal(List<OrderItem> Items)
        {
            double TotalCost = 0;
            foreach (OrderItem item in Items)
            {
                TotalCost += item.Qty * item.UnitCost;
            }
            //double? TotalCost = order.OrderItems.Sum(x => x.Qty * x.UnitCost);
            return TotalCost;
        }

        private double CalculateAdnChargeTotal(Order order)
        {
            double TotalCost = 0;
            foreach (AdditionalCharge charge in order.AdditionalCharges)
            {
                TotalCost += charge.Qty * charge.Cost;
            }

            return TotalCost;
        }

        public double CalculateOrdItmVatTotal(List<OrderItem> Items, double VatPercentage, string type)
        {
            double TotalVat = 0;

            //double? Cost = order.OrderItems.Sum(x => x.Qty * x.UnitCost);
            if (String.Equals(type, "STA"))
            {
                foreach (OrderItem item in Items)
                {
                    TotalVat += (item.Qty * item.UnitCost) * (VatPercentage / 100);
                }
            }
            //TotalVat = order.OrderItems.Sum(x => ((x.Qty * x.UnitCost) / 100) * order.VatPercentage);
            return TotalVat;
        }

    }
    
}