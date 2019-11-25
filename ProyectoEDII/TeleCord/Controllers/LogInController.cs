using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeleCord.Controllers
{
    public class LogInController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Log_in()
        {
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            //compre with mongo
            //success: load messages site
            //fail: show message and form again
            return View();
        }
    }
}