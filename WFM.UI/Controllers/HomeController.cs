using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WFM.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Months = "Jan-2020, Feb-2020, Mar-2020, Apr-2020, May-2020, Jun-2020";
            ViewBag.TotalQuotes = "Total Quotes, 80, 10, 50, 60, 40, 20";
            ViewBag.TotalConvertedJobs = "Total Converted Jobs, 30, 60, 50, 120, 20, 40";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}