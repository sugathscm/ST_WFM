using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
using WFM.BAL.Services;

namespace WFM.UI.Controllers
{
    public class ReportController : Controller
    {

        private ApplicationUserManager _userManager;
        private readonly OrderService orderService = new OrderService();



        public ReportController()
        {

        }
        public ReportController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: 
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Order()
        {
            return View();
        }
        public ActionResult Invoice()
        {
            return View();
        }


    }
}