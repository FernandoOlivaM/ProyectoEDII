using DLLS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TeleCord.Models;

namespace TeleCord.Controllers
{
    public class RecieveController : Controller
    {
        // GET: Recieve
        public ActionResult Index()
        {
            var active = false;
            var result = Request.Cookies["User"]["token"];
            var tokenValidation = TokenManager.ValidateToken(result);
            if (tokenValidation.ValidTo < DateTime.UtcNow)
            {
                active = true;
            }
            if (!active)
            {
                var UsersList = new List<Users>();
                var User = new Users();
                var login = User.GetLogIn();
                foreach (LogInElements loggers in login)
                {
                    var Users = new Users();
                    Users.UserName = loggers.UserName;
                    if (Users.UserName != Request.Cookies["User"]["username"])
                    {
                        UsersList.Add(Users);
                    }
                }
                return View(UsersList);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        public ActionResult RecieveMessage()
        {
            var ToUser = Request.Form["UserList"].ToString();
            var Users = new Users();
            var diffieHellman = new DiffieHellman();
            var PrivateKey = Convert.ToInt32(Request.Cookies["User"]["privatekey"]);
            var PublicKey = Users.ObtenerB(ToUser);
            var K = diffieHellman.GenerarK(PublicKey, PrivateKey);
            var Key = Convert.ToString(K, 2);
            Key = Key.PadLeft(10, '0');
            return RedirectToAction("Decifrar", new { Key, ToUser });
        }
        public ActionResult Decifrar(string Key, string ToUser)
        {
            var User = new Users();
            var messages = User.GetMessages();
            var StringList = new List<string>();
            var ToRList = new List<string>();
            foreach (MessagesElements elements in messages)
            {
                if ((elements.Transmitter == Request.Cookies["User"]["username"]) && (elements.Reciever == ToUser))
                {
                    StringList.Add(elements.text);
                    ToRList.Add("1");
                }
                else
                {
                    if ((elements.Transmitter == ToUser) && (elements.Reciever == Request.Cookies["User"]["username"]))
                    {
                        StringList.Add(elements.text);
                        ToRList.Add("0");
                    }
                }
            }
            //Decifrar cada mensaje con la llave que se recibió
            SDES DecifradoSDES = new SDES();
            var P10 = "8537926014";
            var P8 = "79358216";
            var P4 = "0321";
            var EP = "01323210";
            var IP = "63572014";
            var ReverseIP = DecifradoSDES.GenerarIPInverso(IP);
            //generar claves
            var resultanteLS1 = DecifradoSDES.GenerarLS1(Key, P10);
            var K1 = DecifradoSDES.GenerarK1(resultanteLS1, P8);
            var K2 = DecifradoSDES.GenerarK2(resultanteLS1, P8);
            var BinaryList = DecifradoSDES.LecutraArchivoDecifrar(StringList);
            var MessagesList = new List<MessagesElements>();
            var cifrar = false;
            var counter = 0;
            foreach (List<string> list in BinaryList)
            {
                var Message = new MessagesElements();
                var response = string.Empty;
                foreach (string binary in list)
                {
                    byte bytefinal = DecifradoSDES.CifrarODecifrar(binary, IP, EP, K1, P4, K2, ReverseIP, cifrar);
                    response += (char)bytefinal;
                }
                Message.Transmitter = ToRList[counter] == "1" ? Request.Cookies["User"]["username"] : ToUser;
                Message.Reciever = ToRList[counter] == "1" ? ToUser : Request.Cookies["User"]["username"];
                Message.text = response;
                MessagesList.Add(Message);
                counter++;
            }
            return View(MessagesList);
        }
    }
}