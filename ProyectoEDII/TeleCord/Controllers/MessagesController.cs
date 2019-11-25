using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeleCord.Controllers
{
    public class MessagesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendMessage(HttpPostedFileBase postedFile)
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            var message = Request.Form["message"].ToString();

            if (postedFile!=null)
            {
                //comprimir el archivo
            }
            else
            {
                //se trabaja solo con el mensaje
            }
            return View();
        }
        
    }
}