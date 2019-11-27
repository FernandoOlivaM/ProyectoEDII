using DLLS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TeleCord.datos;
using TeleCord.Models;

namespace TeleCord.Controllers
{
    public class MessagesController : Controller
    {
        public ActionResult Index()
        {
            var UsersList = new List<Users>();
            var User = new Users();
            var login = User.GetLogIn();
            foreach(LogInElements loggers in login)
            {
                var Users = new Users();
                Users.UserName = loggers.UserName;
                UsersList.Add(Users);
            }
            return View(UsersList);
        }
        [HttpPost]
        //falta revisar la vista de este y mejorarla, es algo extra
        public ActionResult SendMessage(HttpPostedFileBase postedFile)
        {
            var ToUser = Request.Form["UserList"].ToString();
            var message = Request.Form["message"].ToString();
            if (postedFile != null)
            {
                return View();
            }
            else
            {
                var Users = new Users();
                var diffieHellman = new DiffieHellman();
                var Privatekey = datosSingelton.Datos.PrivateKey;
                var PublicKey = Users.ObtenerB(ToUser);
                var K = diffieHellman.GenerarK(PublicKey, Privatekey);
                var Key = Convert.ToString(K, 2);
                Key = Key.PadLeft(10, '0');
                return RedirectToAction("Cifrar", new { Key, message,ToUser });
            }
        }
        public ActionResult Cifrar(string Key,string message, string ToUser)
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
            elemento.Transmitter = datosSingelton.Datos.Nombre;
            elemento.Reciever = ToUser;
            elemento.text = ciphertext;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51508");
                var postjob = client.PostAsync("api/Messages", new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
                postjob.Wait();
            }
            return View();
        }
    }
}