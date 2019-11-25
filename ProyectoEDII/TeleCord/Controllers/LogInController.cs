using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeleCord.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TeleCord.Controllers
{
    public class LogInController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //Get LogIn users
        public ActionResult Log_in()
        {
            //Post
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            LogInElements elemento = new LogInElements();
            elemento.Password = password;
            elemento.UserName = userName;
            IEnumerable<LogInElements> login = null;
            var id = "5dd9bffe81ab023bcca4d1c6";
            //Put
            using (var client = new HttpClient())
            {
                elemento.Id = id;
                client.BaseAddress = new Uri("http://localhost:52824");
                var putTask = client.PutAsync("api/LogIn/" + id, new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
                putTask.Wait();
            }
            //Delete
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52824");
                var deleteTask = client.DeleteAsync("api/LogIn/" + id);
                deleteTask.Wait();
            }
            //Post
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52824");
                var postjob = client.PostAsync("api/LogIn", new StringContent(new JavaScriptSerializer().Serialize(elemento),Encoding.UTF8,"application/json"));
                postjob.Wait();
            }
            //Get
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52824");
                var responseTask = client.GetAsync("api/LogIn");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    login = JsonConvert.DeserializeObject<IList<LogInElements>>(readTask.Result);
                }
                else
                {
                    login = Enumerable.Empty<LogInElements>();
                }
            }
            //compre with mongo
            //success: load messages site
            //fail: show message and form again
            foreach (LogInElements elements in login)
            {
                if ((elements.Password == password) && (elements.UserName == userName))
                {
                    return View(login);
                }
            }
            return HttpNotFound();
        }
    }
}