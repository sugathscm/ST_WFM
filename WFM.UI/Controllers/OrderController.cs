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
using WFM.UI.Models;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using System.Web.Configuration;
using Aspose.Words;
using System.Data.Entity;
using WFM.DAL;
using WFM.BAL.Helpers;
using System.ComponentModel.Design;
using Microsoft.Ajax.Utilities;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Web.Security;
using System.Data;

namespace WFM.UI.Controllers
{
    public class test
    {
        public string name { get; set;}
        public string name1 { get; set; }
        public string name2 { get; set; }

    }

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
        private readonly OrderTypeService orderTypeService = new OrderTypeService();
        private readonly ErrorLogService errlog = new ErrorLogService();
        

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


        public JsonResult Convert(int qouteid)
        {
            /// Quote qoute = new Quote();
            var qoute = GetQuoteById(qouteid);

            OrderView order = new OrderView(orderService);
            order.Month = qoute.Month;
            order.Year = qoute.Year;

            order.CreatedDate = qoute.CreatedDate;
            
            order.QuoteId = qoute.Id;
            order.ClientId = (int)qoute.ClientId;

            order.Code = qoute.Code;
            order.CodeNumber = qoute.CodeNumber;
            order.Comments = qoute.Comments;
            order.ContactPerson = qoute.ContactPerson;
            order.ContactMobile = qoute.ContactMobile;
            order.Header = qoute.Header;
            //order.FrameworkWarrantyPeriod = qoute.FrameworkWarrantyPeriod;
            //order.IlluminationWarrantyPeriod = qoute.IlluminationWarrantyPeriod;
            //order.LetteringWarrantyPeriod = qoute.LetteringWarrantyPeriod;
            
            List<OrderItem> orItem = new List<OrderItem>();
            foreach (QuoteItem qitm in qoute.QuoteItems)
            {
                OrderItem otm = new OrderItem();
                otm.UnitCost = (double)qitm.UnitCost;
                otm.Qty = (int)qitm.Qty;
                otm.Size=qitm.Size;
                otm.VAT = (double)qitm.VAT;
                otm.TotalCost= (double)qitm.TotalCost;
                otm.Installation = qitm.Installation;
                otm.Description=qitm.Description;
                otm.VisibilityId= qitm.VisibilityId;
                otm.FrameworkWarrantyPeriod = (int)qitm.FrameworkWarrantyPeriod;
                otm.LetteringWarrantyPeriod= (int)qitm.LetteringWarrantyPeriod;
                otm.IlluminationWarrantyPeriod=(int)qitm.IlluminationWarrantyPeriod;
                


                otm.CategoryId = qitm.CategoryId;                
                orItem.Add(otm);

            }
            order.OrderItems = orItem;

            //OrderView t = new OrderView();
            //t.ClientName = "saad";
            //t.ClientId = 12;
            //t.Code= "dkskd";

            // return Json(order, JsonRequestBehavior.AllowGet);
            // return Json(order, JsonRequestBehavior.AllowGet);
            // return Json(new { data = order }, JsonRequestBehavior.AllowGet);
            //return order;

            return new JsonResult
            {
                Data = order,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        //[Authorize]
        //  [Authorize(Roles = "Administrator")]
        //  [Authorize(Roles = "Sales")]
        //  [Authorize(Roles = "Design")]
        [Authorize(Roles = "Administrator,Management,Sales,Finance,Factory")]
        public ActionResult QouteDetails(int? QuoteId, string FromQuate)
        {
            Order order = new Order();
            Quote qoute = new Quote();
            qoute = GetQuoteById(QuoteId);

            if (qoute != null)
            {
               // order = ConvertQuotetoOrder(qoute);
                
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
            var orderTypeList = orderTypeService.GetOrderTypeList();
            var warrantyPeriodList = warrantyPeriodService.GetWarrantyPeriodList();

            ViewBag.OrderTermList = orderTermList;
            ViewBag.OrderTypeList = new SelectList(orderTypeList, "Id", "Name");
            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");
            ViewBag.WarrantyPeriodList = new SelectList(warrantyPeriodList, "Id", "Duration");
            ViewBag.CategoryList = categoryService.GetCategoryList();
            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];
            return View(order);
        }
        public ActionResult History()
        {
            return View();
        }
      // [Authorize]
       [Authorize(Roles = "Administrator,Management,Sales,Finace,Factory")]
        // GET: Order
        public ActionResult Details(int? id, int? qouteid)
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //var roles = userManager.GetRoles(User.Identity.GetUserId());

            //var role = User.IsInRole("Administrator");
            Order order = new Order();
             
            if (id != null)
            {
                order = orderService.GetOrderById(id);
            }
            else
            {
                var code = CommonService.GenerateQuoteCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), false);//.Replace('-', '/');
                order.Code = code;
            }
            
            //ViewBag.UploadFile = null;

            // if (qouteid != null)
            //{
            //    Quote qoute = new Quote();
            //    qoute = GetQuoteById(qouteid);
            //    order = ConvertQuotetoOrder(qoute);

            //  //  return Json(new { order });
            //    return Json(order, JsonRequestBehavior.AllowGet);
            //    //var code = CommonService.GenerateOrderCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), false).Replace('-', '/');
            //    //order.Code = code;
            //}

            var quoteList = quoteService.GetQuoteList().Where(q => q.IsConverted == false).ToList();
            var clientList = clientService.GetClientList();
            var employeeList = employeeService.GetEmployeeList();
            var orderTermList = quoteTermService.GetQuoteTermList();
            var orderTypeList = orderTypeService.GetOrderTypeList();
            var IlluminationList = orderService.GetIlluminationList();
            var DeliveryTypeList=orderService.GetDeliveryTypeList();
            var warrantyPeriodList = warrantyPeriodService.GetWarrantyPeriodList();
            var VisibilityList = orderService.GetVisibilityList();
            //List<BaseViewModel> VisibilityList = new List<BaseViewModel>();

            //VisibilityList.Add(new BaseViewModel() { Id = 1, Name = "Single Sided" });
            //VisibilityList.Add(new BaseViewModel() { Id = 2, Name = "Double Sided" });


            //List<BaseViewModel> WarrantyPeriodList = new List<BaseViewModel>();
            //for (int i = 1; i < 10; i++)
            //{
            //    WarrantyPeriodList.Add(new BaseViewModel() { Id = i, Name = i.ToString() });
            //}


            //List<BaseViewModel> IlluminationList = new List<BaseViewModel>();
            //for (int i = 1; i < 10; i++)
            //{
            //    IlluminationList.Add(new BaseViewModel() { Id = i, Name = i.ToString() });
            //}



            ViewBag.DeliveryTypeList= new SelectList(DeliveryTypeList, "Id", "Type"); 
            ViewBag.IlluminationList = IlluminationList;
            ViewBag.WarrantyPeriodList = warrantyPeriodList;
            ViewBag.VisibilityList = VisibilityList;
            ViewBag.OrderTermList = orderTermList;
            ViewBag.OrderTypeList = new SelectList(orderTypeList, "Id", "Name");
            ViewBag.QuoteList = new SelectList(quoteList, "Id", "Code");
            ViewBag.ClientList = new SelectList(clientList, "Id", "Name");
            ViewBag.ChanneledByList = new SelectList(employeeList, "Id", "Name");
            //ViewBag.WarrantyPeriodList = new SelectList(warrantyPeriodService.GetWarrantyPeriodList(), "Id", "Name");
            ViewBag.CategoryList = categoryService.GetCategoryList();
            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];
            return View(order);
        }

        //[Authorize]
        //[Authorize(Roles = "Administrator")]
        //[Authorize(Roles = "Sales")]
        //[Authorize(Roles = "Design")]
        [Authorize(Roles = "Administrator,Management,Sales,Factory,Design")]
        public ActionResult GetHistoryList()
        {
            var list = orderService.GetOrderHistoryList();

            List<OrderView> modelList = new List<OrderView>();

            foreach (var item in list)
            {
                modelList.Add(new OrderView(orderService)
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    Code = item.Code,
                    Value = item.Value,
                    CreatedDate = item.CreatedDate,
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    //DeliveryDateString = item.DeliveryDate.Value.ToString(),
                    Header = item.Header,
              
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        //[Authorize(Roles = "Administrator")]
        //[Authorize(Roles = "Sales")]
        [Authorize(Roles = "Administrator,Management,Sales,Factory")]
        public ActionResult GetList()
        {   
            var list = orderService.GetOrderActiveList();

            List<OrderView> modelList = new List<OrderView>();

            foreach (var item in list)
            {
                modelList.Add(new OrderView(orderService)
                {
                    Id = item.Id,
                    ClientName = item.Client.Name,
                    Code = item.Code,
                    Month= item.CreatedDate.Value.Month.ToString(),
                   // DeliveryType=item.DeliveryType.Type,
                    CreatedDate = item.CreatedDate,
                    CreatedDateString = item.CreatedDate.Value.ToString(),
                    BaseQoute = item.BaseQuoteId, 
                    ChanneledBy=item.Employee.Name,
                    //DeliveryDate = item.DeliveryDate,
                    //DeliveryDateString = item.DeliveryDate.Value.ToString(),
                    Header = item.Header,
                    OrderTypeId=(int)item.OrderTypeId
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "Administrator")]
        //[Authorize(Roles = "Sales")]
        [Authorize(Roles = "Administrator,Management,Sales,Factory")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Order model, FormCollection formCollection, HttpPostedFileBase[] file)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                var productIdArray = formCollection["productIdArray"].Split(',');
                var itmqtyArray = formCollection["itmqtyArray"].Split(',');
                var descriptionArray = formCollection["descriptionArray"].Split(',');
                var specialInstructionsArray = formCollection["SpecialInstructionsArray"].Split(',');
                var itmcostArray = formCollection["itmcostArray"].Split(',');
                var itmtotcostArray = formCollection["itmtotcostArray"].Split(',');
                var vatArray = formCollection["vatArray"].Split(',');
                var sizeArray = formCollection["sizeArray"].Split(',');
                var chargeItemArray = formCollection["chargeItemArray"].Split(',');
                var AttachmentNameArray = formCollection["AttachmentNameArray"].Split(',');
                var channeledByArray = formCollection["channeledByArray"].Split(',');
                var chargeCostArray = formCollection["chargeCostArray"].Split(',');
                var chargeQtyArray = formCollection["chargeQtyArray"].Split(',');
                var chargeTotalCostArray = formCollection["chargeTotalCostArray"].Split(',');
                
                var installationArray = formCollection["installationArray"].Split(',');
                //var categoryTypeArray = formCollection["categoryTypeArray"].Split(',');
                var visibilityArray = formCollection["visibilityArray"].Split(',');
                var illuminationArray = formCollection["illuminationArray"].Split(',');
                var frameworkWarrantyArray = formCollection["frameworkWarrantyArray"].Split(',');
                var letteringWarrantyArray = formCollection["letteringWarrantyArray"].Split(',');
                var illuminationWarrantyArray = formCollection["illuminationWarrantyArray"].Split(',');
                var artworkFile =formCollection["Artwork"];
                var itemIdArray = formCollection["itemIdArray"].Split(',');
                var addchrgIdArray = formCollection["addchrgIdArray"].Split(',');



                int id = model.Id;

                Order order = null;
                Order oldOrder = null;

                


                if (model.Id == 0)
                {   
                    order = new Order
                    {
                        ClientId = model.ClientId,
                        Code = model.Code,
                        CodeNumber = int.Parse(model.Code.Split('-')[3]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        ChanneledById = model.ChanneledById,
                        Value = model.Value,
                        Comments = model.Comments,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.GetUserId(),
                        Header = model.Header,
                        IsVAT = model.IsVAT,
                        OrderVAT = model.OrderVAT,
                        Location = model.Location,
                        BaseQuoteId = model.BaseQuoteId,
                        AdvancePayment = model.AdvancePayment != null ? model.AdvancePayment :0,
                        //WarrantyPeriodId = model.WarrantyPeriodId,
                        ContactPerson = model.ContactPerson,
                        ContactMobile = model.ContactMobile,
                        DeliveryDate = model.DeliveryDate,
                        DeliveryTypeId = model.DeliveryTypeId,
                        QuoteId = model.QuoteId,
                        Designer=model.Designer,
                        Remarks=model.Remarks,
                        //FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod,
                        //IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod,
                        //LetteringWarrantyPeriod = model.LetteringWarrantyPeriod,
                        OrderItems = model.OrderItems,
                        OrderTypeId = model.OrderTypeId,
                      
                        ArtWork = artworkFile ,
                        //PurchaseOrder=PurchaseOrderFile,
                        IsArtDepartment = model.IsArtDepartment,    
                        IsCladdingsection = model.IsCladdingsection,    
                        IsCuttingDepartment = model.IsCuttingDepartment,
                        IsInstallationTeam = model.IsInstallationTeam,  
                        IsPackingDepartment = model.IsPackingDepartment,    
                        IsPlasticDepartment = model.IsPlasticDepartment,
                        IsPrintingDepartment = model.IsPrintingDepartment,  
                        IsProductionDepartment = model.IsProductionDepartment,
                        IsScreenPrintingDept =model.IsScreenPrintingDept,
                        IsSteelDepartment = model.IsSteelDepartment,
                        
                        
                      
                      
                       
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
                    order.ChanneledById = model.ChanneledById;
                    order.Value = model.Value;
                    order.Header = model.Header;
                    order.Comments = model.Comments;
                    order.UpdatedBy = User.Identity.GetUserId();
                    order.UpdatedDate = DateTime.Now;
                    order.ContactPerson = model.ContactPerson;
                    order.ContactMobile = model.ContactMobile;
                    order.ChanneledById=model.ChanneledById;

                    order.IsVAT = model.IsVAT;
                    order.OrderVAT = model.OrderVAT;
                    order.Location = model.Location;
                    order.BaseQuoteId = model.BaseQuoteId;
                    order.AdvancePayment = model.AdvancePayment;

                    //order.FrameworkWarrantyPeriod = model.FrameworkWarrantyPeriod;
                    //order.IlluminationWarrantyPeriod = model.IlluminationWarrantyPeriod;
                    //order.LetteringWarrantyPeriod = model.LetteringWarrantyPeriod;
                    order.DeliveryDate = model.DeliveryDate;
                  
                    order.QuoteId = model.QuoteId;
                    order.Designer = model.Designer;
                    order.Remarks = model.Remarks;
                    //order.OrderItems = model.OrderItems;
                    order.DeliveryTypeId = model.DeliveryTypeId;
                    order.ArtWork = artworkFile;
                    //order.PurchaseOrder = PurchaseOrderFile;
                    order.OrderTypeId = model.OrderTypeId;


                    order.IsArtDepartment = model.IsArtDepartment;    
                    order.IsCladdingsection = model.IsCladdingsection;    
                    order.IsCuttingDepartment = model.IsCuttingDepartment;
                    order.IsInstallationTeam = model.IsInstallationTeam;
                    order.IsPackingDepartment = model.IsPackingDepartment;    
                    order.IsPlasticDepartment = model.IsPlasticDepartment;
                    order.IsPrintingDepartment = model.IsPrintingDepartment;
                    order.IsProductionDepartment = model.IsProductionDepartment;
                    order.IsScreenPrintingDept = model.IsScreenPrintingDept;
                    order.IsSteelDepartment = model.IsSteelDepartment;

                    newData = new JavaScriptSerializer().Serialize(new Order()
                    {
                        Id = order.Id
                    });

                    //orderService.RemoveItems(order);
                }

                int i = 0;
                //order.OrderItems.Clear();
                OrderItem orderItem = null;

                foreach (var item in productIdArray)
                {
                    if (model.Id == 0)
                    {
                        orderItem = new OrderItem
                        {
                            CategoryId = int.Parse(item),
                            Qty = (itmqtyArray[i] == "") ? 0 : double.Parse(itmqtyArray[i]),
                            UnitCost = (itmcostArray[i] == "") ? 0.00 : double.Parse(itmcostArray[i]),
                            TotalCost = (itmtotcostArray[i] == "") ? 0.00 : double.Parse(itmtotcostArray[i]),
                            VAT = (vatArray[i] == "") ? 0.00 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i],
                            FrameworkWarrantyPeriod = (frameworkWarrantyArray[i] == "") ? 4 : int.Parse(frameworkWarrantyArray[i]),
                            IlluminationWarrantyPeriod = (illuminationWarrantyArray[i] == "") ? 4 : int.Parse(illuminationWarrantyArray[i]),
                            LetteringWarrantyPeriod = (letteringWarrantyArray[i] == "") ? 4 : int.Parse(letteringWarrantyArray[i]),
                            VisibilityId = (visibilityArray[i] == "") ? 3 : int.Parse(visibilityArray[i]),
                            IlluminationId = (illuminationArray[i] == "") ? 7 : int.Parse(illuminationArray[i]),
                            Installation = (installationArray[i] == "") ? "" : installationArray[i],
                            //  CategoryType = (categoryTypeArray[i] == "") ? "" : categoryTypeArray[i],                           
                            SpecialInstruction = (specialInstructionsArray[i] == "") ? "" : specialInstructionsArray[i]



                        };
                        order.OrderItems.Add(orderItem);
                    }
                    else
                    {
                        orderItem = new OrderItem
                        {
                            Id = (itemIdArray[i] == "") ? 0 : int.Parse(itemIdArray[i]),
                            CategoryId = int.Parse(item),
                            Qty = (itmqtyArray[i] == "") ? 0 : double.Parse(itmqtyArray[i]),
                            UnitCost = (itmcostArray[i] == "") ? 0.00 : double.Parse(itmcostArray[i]),
                            TotalCost = (itmtotcostArray[i] == "") ? 0.00 : double.Parse(itmtotcostArray[i]),
                            VAT = (vatArray[i] == "") ? 0.00 : double.Parse(vatArray[i]),
                            Size = (sizeArray[i] == "") ? "" : sizeArray[i],
                            Description = (descriptionArray[i] == "") ? "" : descriptionArray[i],
                            OrderId = model.Id,
                            FrameworkWarrantyPeriod = (frameworkWarrantyArray[i] == "") ? 4 : int.Parse(frameworkWarrantyArray[i]),
                            IlluminationWarrantyPeriod = (illuminationWarrantyArray[i] == "") ? 4 : int.Parse(illuminationWarrantyArray[i]),
                            LetteringWarrantyPeriod = (letteringWarrantyArray[i] == "") ? 4 : int.Parse(letteringWarrantyArray[i]),
                            VisibilityId = (visibilityArray[i] == "") ? 3 : int.Parse(visibilityArray[i]),
                            IlluminationId = (illuminationArray[i] == "") ? 7 : int.Parse(illuminationArray[i]),
                            SpecialInstruction = (specialInstructionsArray[i] == "") ? "" : specialInstructionsArray[i],
                            Installation = (installationArray[i] == "") ? "" : installationArray[i]
                        };

                        orderService.SaveOrUpdate(orderItem);
                    }

                    i++;
                }


                int j = 0;
                //order.AdditionalCharges.Clear();
                AdditionalCharge additionalCharge = null;





                foreach (var item in chargeItemArray)
                {
                    if (model.Id == 0)
                    {
                        additionalCharge = new AdditionalCharge
                        {
                            
                            AddCharge = (chargeItemArray[j] == "") ? "" : chargeItemArray[j],
                            Cost= (chargeCostArray[j] == "") ? 0.00 : double.Parse(chargeCostArray[j]),
                            Qty = (chargeQtyArray[j] == "") ? 0.00 : double.Parse(chargeQtyArray[j]),
                            OrderId = model.Id,
                            TotalCost = (chargeTotalCostArray[j] == "") ? 0.00 : double.Parse(chargeTotalCostArray[j])

                        };
                        order.AdditionalCharges.Add(additionalCharge);
                    }
                    else
                    {
                        additionalCharge = new AdditionalCharge
                        {
                            Id= (addchrgIdArray[j] == "") ? 0 : int.Parse(addchrgIdArray[j]),
                            AddCharge = (chargeItemArray[j] == "") ? "" : chargeItemArray[j],
                            Cost = (chargeCostArray[j] == "") ? 0.00 : double.Parse(chargeCostArray[j]),
                            Qty = (chargeQtyArray[j] == "") ? 0.00 : double.Parse(chargeQtyArray[j]),
                            OrderId = model.Id,
                            TotalCost = (chargeTotalCostArray[j] == "") ? 0.00 : double.Parse(chargeTotalCostArray[j])
                            


                        };
                        orderService.SaveOrUpdate(additionalCharge);
                        //order.AdditionalCharges.Add(additionalCharge);
                       
                    }

                    j++;
                }


                // attachments----------------------------------------------------------------------------------------------------


                j=0;
                OrderAttachment orderAttachment = null;
                List<UploadFile> files = new List<UploadFile>();
                files = (List<UploadFile>)HttpContext.Session["uploadfile"];

                if (files!=null) { 
                    foreach (var item in files)
                    {  if (model.Id == 0)
                        {
                            orderAttachment = new OrderAttachment
                            {
                           
                                ActualName = item.FileName,
                                AttachmentName= item.SaveFile
                           
                            
                            };
                            order.OrderAttachments.Add(orderAttachment);
                        }
                        else
                        {
                            orderAttachment = new OrderAttachment
                            {


                                OrderId = model.Id,
                                ActualName = item.FileName,
                                AttachmentName = item.SaveFile,
                                


                            };
                            orderService.SaveOrUpdate(orderAttachment);
                            //order.OrderAttachments.Add(orderAttachment);
                        }
                        j++;
                    }
                }   
                orderService.SaveOrUpdate(order);

                HttpContext.Session["uploadfile"] = null;

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
                ErrorLog er = new ErrorLog();
                er.Type = "Order";
                er.Error = ex.Message;
                errlog.SaveOrUpdate(er);
                TempData["Message"] = string.Format(ResourceData.SaveErrorMessage, ex.InnerException);
            }
           

            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Management,Sales,Factory")]
        public ActionResult FileUpload()
        {
            
            if (Request.Files.Count > 0)
            {
                var files = Request.Files;
                
                //iterating through multiple file collection   
                foreach (string str in files)
                {
                   HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName.Replace(" ", ""));
                        var saveName = Guid.NewGuid().ToString() + InputFileName;

                        var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + saveName);
                        //Save file to server folder  
                        UploadFile uploadFile = new UploadFile();
                        uploadFile.FileName = InputFileName;
                        uploadFile.SaveFile = saveName;
                        file.SaveAs(ServerSavePath);
                        
                        SaveUpload(uploadFile);
                    }

                }
                return Json("File Uploaded Successfully!");

            }
            else
            {
                return Json("No files to upload");
            }

        }
          private void SaveUpload(UploadFile uploadfile)
        {
            if (  (List<UploadFile>)HttpContext.Session["uploadfile"]  == null)
            {
                List<UploadFile> files = new List<UploadFile>();
                files.Add(uploadfile);
                HttpContext.Session.Add("uploadfile", files);
             }
            else
            {
                List<UploadFile> files = new List<UploadFile>();
                files= (List<UploadFile>)HttpContext.Session["uploadfile"];
                files.Add(uploadfile);
                ViewBag.UploadFile = files;
            
            }
          } 
     
        public Quote GetQuoteById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes
                    .Include("QuoteItems")
                    .Include("Client")
                    .Include("QuoteTermDetails")
                    .Include("QuoteTermDetails.QuoteTerm")

                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }
        public PartialViewResult _NewClientPartial()
        {
            return PartialView();
        }
        [Authorize]
        public ActionResult PrintOrder(int id)
        {
            Order order = orderService.GetOrderById(id);

            OrderView orderView = new OrderView(orderService);
           

            PropertyCopier<Order, OrderView>.Copy(order, orderView);
            ViewBag.VATPercentage = WebConfigurationManager.AppSettings["WBU"];
            ViewBag.VATNo = ResourceData.VATNo;
            ViewBag.Name = ResourceData.Name;
            ViewBag.AuthorizedPersonName = ResourceData.AuthorizedPersonName;
            ViewBag.AuthorizedPersonDesignation = ResourceData.AuthorizedPersonDesignation;
          
     

            orderView.CreatedDateString = orderView.CreatedDate.Value.ToString("dd/MM/yyyy");
            orderView.OrderTotal = CalculateOrdItmTotal(order) + CalculateAdnChargeTotal(order);
            orderView.VAT = CalculateOrdItmVatTotal(order);          //  orderView.OrderTermDetails = orderService.get.GetQuoteTermsByQuoteId(quote.Id);
            orderView.BalancePayment = (double)((orderView.OrderTotal + orderView.VAT) - (orderView.AdvancePayment != null ? orderView.AdvancePayment: 0));
            orderView.TotalWithVat = (double)(orderView.OrderTotal + orderView.VAT );

            return PartialView("_PrintOrder", orderView);
        }

        

        private double CalculateOrdItmTotal(Order order)
        {
            double? TotalCost = order.OrderItems.Sum(x => x.Qty* x.UnitCost);
            return TotalCost.HasValue ? TotalCost.Value : 0;
        }

        private double CalculateAdnChargeTotal(Order order)
        {
            double? TotalCost = order.AdditionalCharges.Sum(x => x.Qty * x.Cost);
            return TotalCost.HasValue ? TotalCost.Value : 0;
        }

        private double CalculateOrdItmVatTotal(Order order)
        { 
            //double? Cost = order.OrderItems.Sum(x => x.Qty * x.UnitCost);
            double? TotalVat = order.OrderItems.Sum(x =>((x.Qty*x.UnitCost)/100)* x.VAT);
            
            return TotalVat.HasValue ? TotalVat.Value : 0;
        }

        

    }
}