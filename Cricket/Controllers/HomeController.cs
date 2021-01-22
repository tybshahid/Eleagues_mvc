using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cricket.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ViewResult Error()
        {
            return View("Error");
        }
        [ValidateAntiForgeryToken]
        public ActionResult About()
        {
            return View("About");
        }
    }
}