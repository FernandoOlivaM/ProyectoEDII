using DLLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TeleCord.Models;

namespace TeleCord.Controllers
{
    public class MessagesController : Controller
    {
        static string UserName = string.Empty;
        public ActionResult Index(string username)
        {
            UserName = username;
            return View();
        }
        [HttpPost]
        //falta revisar la vista de este y mejorarla, es algo extra
        public ActionResult SendMessage(HttpPostedFileBase postedFile)
        {
            var key = Convert.ToInt32(Request.Form["key"]);
            var message = Request.Form["message"].ToString();

            DateTime now = DateTime.Now;
            long ticks = now.Ticks;
            long aValue = ticks % 23;

            var a = 22;
            var b = 105;
            var g = 15;
            var p = 23;
            DiffieHellman diffiehellman = new DiffieHellman();
            var K = diffiehellman.GenerarClaves(a,b,g,p);
            var Key = Convert.ToString(a, 2);
            Key = Key.PadLeft(10, '0');
            return RedirectToAction("Cifrar", new { Key,message });
        }
        public ActionResult Cifrar(string Key,string message)
        {
            SDES cifradoSDES = new SDES();
            var P10 = "8537926014";
            var P8 = "79358216";
            var P4 = "0321";
            var EP = "01323210";
            var IP = "63572014";
            var ReverseIP = cifradoSDES.GenerarIPInverso(IP);
            //generar claves
            var resultanteLS1= cifradoSDES.GenerarLS1(Key, P10);
            var K1 = cifradoSDES.GenerarK1(resultanteLS1, P8);
            var K2 = cifradoSDES.GenerarK2(resultanteLS1, P8);
            //cifrar
            var BinaryList = cifradoSDES.LecturaArchivo(message);
            var byteList = new List<byte>();
            var cifrar = true;
            foreach (string binary in BinaryList)
            {
                byte bytefinal = cifradoSDES.CifrarODecifrar(binary, IP, EP, K1, P4, K2, ReverseIP, cifrar);
                byteList.Add(bytefinal);
            }
            var ciphertext = string.Empty;
            foreach(byte bit in byteList)
            {
                ciphertext+=(char)bit;
            }
            //enviar el texto
            MessagesElements elemento = new MessagesElements();
            elemento.Transmitter = UserName;
            elemento.Reciever = "Zerafina";
            elemento.text = ciphertext;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52824");
                var postjob = client.PostAsync("api/Messages", new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
                postjob.Wait();
            }
            return View();
        }
    }
}