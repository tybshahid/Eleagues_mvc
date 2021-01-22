using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cricket.Controllers
{
    public class StarsController : Controller
    {
        // GET: Stars
        public ActionResult Index()
        {
            return View();
        }
    }
}