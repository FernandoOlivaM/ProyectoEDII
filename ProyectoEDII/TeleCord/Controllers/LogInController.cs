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
using DLLS;
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
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            var levels = Convert.ToInt32(Request.Form["levels"].ToString());
            LogInElements element = new LogInElements();
            element.Password = password;
            element.UserName = userName;
            IEnumerable<LogInElements> login = null;
            ////Put
            //using (var client = new HttpClient())
            //{
            //    elemento.Id = id;
            //    client.BaseAddress = new Uri("http://localhost:52824");
            //    var putTask = client.PutAsync("api/LogIn/" + id, new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
            //    putTask.Wait();
            //}
            ////Delete
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:52824");
            //    var deleteTask = client.DeleteAsync("api/LogIn/" + id);
            //    deleteTask.Wait();
            //}
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
                if ((elements.UserName == userName))
                {
                    ZigZag cifradoZigZag = new ZigZag();
                    var CaracterExtra = new byte();
                    var BytesList = cifradoZigZag.LecturaDescifrado(elements.Password, ref CaracterExtra);
                    var CantidadCaracterExtra = 0;
                    var Matrix = cifradoZigZag.MatrixCreationDecryption(BytesList.Count(), levels, ref CantidadCaracterExtra);
                    var CaracterExtra2 = new byte();
                    if (CantidadCaracterExtra > BytesList.Count())
                    {
                        BytesList = cifradoZigZag.AgregarCaracterExtra(BytesList, CantidadCaracterExtra, ref CaracterExtra2);
                    }
                    //hace falta enviar CaracterExtra2
                    var Decipherpassword = cifradoZigZag.DecifrarMensaje(levels, BytesList, Matrix, CaracterExtra);
                    if (Decipherpassword == password)
                    {
                        return RedirectToAction("Index","Messages", new { userName});
                    }
                }
            }
            return HttpNotFound();
        }
        public ActionResult SignUp()
        {
            //Post
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            var levels = Convert.ToInt32(Request.Form["levels"].ToString());
            //método para cifrar la clave ZigZag
            ZigZag cifradoZigZag = new ZigZag();
            var BytesList = cifradoZigZag.LecturaCifrado(password);
            var CantidadCaracterExtra = 0;
            var Matrix = cifradoZigZag.MatrixCreation(BytesList.Count(), levels, ref CantidadCaracterExtra);
            var CaracterExtra = new byte();
            if (CantidadCaracterExtra > BytesList.Count())
            {
                BytesList = cifradoZigZag.AgregarCaracterExtra(BytesList, CantidadCaracterExtra, ref CaracterExtra);
            }
            BytesList = cifradoZigZag.CifrarMensaje(Matrix, levels, BytesList, CaracterExtra);
            var CipherPassword = string.Empty;
            foreach(byte bit in BytesList)
            {
                CipherPassword += (char)bit;
            }
            //elemento a cifrar
            LogInElements elemento = new LogInElements();
            elemento.Password = CipherPassword;
            elemento.UserName = userName;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52824");
                var postjob = client.PostAsync("api/LogIn", new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
                postjob.Wait();
            }
            return RedirectToAction("Index");
        }
    }
}