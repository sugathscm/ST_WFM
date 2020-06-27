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
    public class PrinterController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly PrinterService printerService = new PrinterService();

        public PrinterController()
        {
        }

        public PrinterController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Printer
        public ActionResult Index(int? id)
        {
            Printer printer = new Printer();
            if (id != null)
            {
                printer = printerService.GetPrinterById(id);
            }
            return View(printer);
        }

        public ActionResult GetList()
        {
            List<Printer> list = printerService.GetPrinterList();

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
        public ActionResult SaveOrUpdate(Printer model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Printer printer = null;
                Printer oldPrinter = null;
                if (model.Id == 0)
                {
                    printer = new Printer
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldPrinter = new Printer();
                    oldData = new JavaScriptSerializer().Serialize(oldPrinter);
                    newData = new JavaScriptSerializer().Serialize(printer);
                }
                else
                {
                    printer = printerService.GetPrinterById(model.Id);
                    oldPrinter = printerService.GetPrinterById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Printer()
                    {
                        Id = oldPrinter.Id,
                        Name = oldPrinter.Name,
                        IsActive = oldPrinter.IsActive
                    });

                    printer.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    printer.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Printer()
                    {
                        Id = printer.Id,
                        Name = printer.Name,
                        IsActive = printer.IsActive
                    });
                }

                printerService.SaveOrUpdate(printer);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Printer",
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


            return RedirectToAction("Index", "Printer");
        }
    }
}