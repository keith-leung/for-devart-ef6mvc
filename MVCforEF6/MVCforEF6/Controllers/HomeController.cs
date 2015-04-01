using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCforEF6.Controllers
{
    public class HomeController : Controller
    {
        private AtlanticDXContext m_datacontext = new AtlanticDXContext();

        public ActionResult Index()
        {
            int count = m_datacontext.SysMenus.Count();
            System.Diagnostics.Debug.WriteLine("Test data: " + count.ToString());

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