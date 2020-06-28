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
    public class EmployeeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly EmployeeService employeeService = new EmployeeService();
        private readonly DesignationService designationService = new DesignationService();

        public EmployeeController()
        {

        }
        public EmployeeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Employee
        public ActionResult Index(int? id)
        {
            Employee employee = new Employee();


            if (id != null)
            {
                employee = employeeService.GetEmployeeById(id);
            }

            var listData = designationService.GetDesignationList();

            ViewBag.ListObject = new SelectList(listData, "Id", "Name");

            return View(employee);
        }

        public ActionResult GetList()
        {

            var list = employeeService.GetEmployeeList();
            List<EmployeeView> modelList = new List<EmployeeView>();
            foreach (var item in list)
            {
                modelList.Add(new EmployeeView()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Title = item.Title,
                    Name = item.Name,
                    Mobile = item.Mobile,
                    Email = item.Email,
                    FixedLine = item.FixedLine,
                    DesignationName = (item.DesignationId == 0) ? "" : designationService.GetDesignationById(item.DesignationId).Name,
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Employee model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Employee employee = null;
                Employee oldEmployee = null;
                if (model.Id == 0)
                {
                    employee = new Employee
                    {
                        Title = model.Title,
                        Name = model.Name,
                        Mobile = model.Mobile,
                        Email = model.Email,
                        FixedLine = model.FixedLine,
                        IsActive = true,
                        DesignationId = model.DesignationId
                    };

                    oldEmployee = new Employee();
                    oldData = new JavaScriptSerializer().Serialize(oldEmployee);
                    newData = new JavaScriptSerializer().Serialize(employee);
                }
                else
                {
                    employee = employeeService.GetEmployeeById(model.Id);
                    oldEmployee = employeeService.GetEmployeeById(model.Id);

                    oldData = new JavaScriptSerializer().Serialize(new Employee()
                    {
                        Id = oldEmployee.Id,
                        Title = oldEmployee.Title,
                        Name = oldEmployee.Name,
                        Mobile = oldEmployee.Mobile,
                        Email = oldEmployee.Email,
                        FixedLine = oldEmployee.FixedLine,
                        DesignationId = oldEmployee.DesignationId,
                        IsActive = oldEmployee.IsActive
                    });

                    employee.Title = model.Title;
                    employee.Name = model.Name;
                    employee.Mobile = model.Mobile;
                    employee.Email = model.Email;
                    employee.FixedLine = model.FixedLine;
                    employee.DesignationId = model.DesignationId;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    employee.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Employee()
                    {
                        Id = employee.Id,
                        Title = employee.Title,
                        Name = employee.Name,
                        Mobile = employee.Mobile,
                        Email = employee.Email,
                        FixedLine = employee.FixedLine,
                        DesignationId = employee.DesignationId,
                        IsActive = employee.IsActive
                    });
                }

                CommonService.SaveDataAudit(new DataAudit()
                {
                    Entity = "Employee",
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

            return RedirectToAction("Index", "Employee");
        }
    }
}