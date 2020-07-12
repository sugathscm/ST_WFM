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
    public class ClientController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly ClientService clientService = new ClientService();
        private readonly DesignationService designationService = new DesignationService();

        public ClientController()
        {

        }
        public ClientController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Client
        public ActionResult Index(int? id)
        {
            Client client = new Client();


            if (id != null)
            {
                client = clientService.GetClientById(id);
            }

            var listData = designationService.GetDesignationList();

            ViewBag.ListObject = new SelectList(listData, "Id", "Name");

            return View(client);
        }

        public ActionResult GetList()
        {

            var list = clientService.GetClientFullList();
            List<ClientView> modelList = new List<ClientView>();
            foreach (var item in list)
            {
                modelList.Add(new ClientView()
                {
                    Id = item.Id,
                    Name = item.Name,                    
                    IsActive = item.IsActive,
                    CPTitle = item.CPTitle,
                    CPName = item.CPName,
                    CPMobile = item.CPMobile,
                    Email = item.Email,
                    LandLine = item.LandLine,
                    FixedLine = item.FixedLine,
                    DesignationName = (item.CPDesignationId == 0 || item.CPDesignationId == null) ? "" : item.Designation.Name,
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Client model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Client client = null;
                Client oldClient = null;
                if (model.Id == 0)
                {
                    client = new Client
                    {
                        Name = model.Name,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        PostCode = model.PostCode,
                        Email = model.Email,
                        FixedLine = model.FixedLine,
                        LandLine = model.LandLine,
                        IsActive = true,
                        CPTitle = model.CPTitle,
                        CPName = model.CPName,
                        CPMobile = model.CPMobile,
                        CPDesignationId = model.CPDesignationId
                    };

                    oldClient = new Client();
                    oldData = new JavaScriptSerializer().Serialize(oldClient);
                    newData = new JavaScriptSerializer().Serialize(client);
                }
                else
                {
                    client = clientService.GetClientById(model.Id);
                    oldClient = clientService.GetClientById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Client()
                    {
                        Id = oldClient.Id,
                        Name = oldClient.Name,
                        AddressLine1 = oldClient.AddressLine1,
                        AddressLine2 = oldClient.AddressLine2,
                        City = oldClient.City,
                        PostCode = oldClient.PostCode,
                        Email = oldClient.Email,
                        FixedLine = oldClient.FixedLine,
                        LandLine = oldClient.LandLine,
                        IsActive = oldClient.IsActive,
                        CPTitle = oldClient.CPTitle,
                        CPName = oldClient.CPName,
                        CPMobile = oldClient.CPMobile,
                        CPDesignationId = oldClient.CPDesignationId
                    });

                    client.Name = model.Name;
                    client.AddressLine1 = model.AddressLine1;
                    client.AddressLine2 = model.AddressLine2;
                    client.City = model.City;
                    client.PostCode = model.PostCode;
                    client.Email = model.Email;
                    client.FixedLine = model.FixedLine;
                    client.LandLine = model.LandLine;
                    client.CPTitle = model.CPTitle;
                    client.CPName = model.CPName;
                    client.CPMobile = model.CPMobile;
                    client.CPDesignationId = model.CPDesignationId;
                    client.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Client()
                    {
                        Id = client.Id,
                        Name = oldClient.Name,
                        AddressLine1 = oldClient.AddressLine1,
                        AddressLine2 = oldClient.AddressLine2,
                        City = oldClient.City,
                        PostCode = oldClient.PostCode,
                        Email = oldClient.Email,
                        FixedLine = oldClient.FixedLine,
                        LandLine = oldClient.LandLine,
                        IsActive = oldClient.IsActive,
                        CPTitle = oldClient.CPTitle,
                        CPName = oldClient.CPName,
                        CPMobile = oldClient.CPMobile,
                        CPDesignationId = oldClient.CPDesignationId
                    });
                }

                clientService.SaveOrUpdate(client);

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Client",
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

            return RedirectToAction("Index", "Client");
        }
    }
}