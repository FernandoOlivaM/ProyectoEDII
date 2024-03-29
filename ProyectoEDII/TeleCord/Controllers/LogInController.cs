﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using TeleCord.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DLLS;
using System.Net;
using System.Security.Cryptography;
namespace TeleCord.Controllers
{
    public class LogInController : Controller
    {
        static int registroValido = 0;
        public ActionResult Index()
        {
            ViewBag.status = registroValido;
            registroValido = 0;
            return View();
        }
        //Get LogIn users
        

        public ActionResult Log_in()
        {
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            var levels = Convert.ToInt32(Request.Form["levels"].ToString());
            //Get
            var User = new Users();
            var login = User.GetLogIn();
            //compre with mongo
            //success: load messages site
            //fail: show message and form again
            foreach (LogInElements elements in login)
            {
                if ((elements.UserName == userName))
                {
                    var Decipherpassword = User.ZigZagEncryptionDechipher(elements.Password, levels);
                    if (Decipherpassword == password)
                    {
                        //datosSingelton.Datos.Nombre = userName;
                        Decipherpassword = User.ZigZagEncryptionDechipher(elements.PrivateKey, levels);
                        var PrivateKey = Convert.ToInt32(Decipherpassword, 2);
                        //datosSingelton.Datos.PrivateKey = PrivateKey;
                        //toke
                        HMACSHA256 hmac = new HMACSHA256();
                        string keys = Convert.ToBase64String(hmac.Key);
                        var result =TokenManager.GenerateToken(userName,keys);
                        
                        //datosSingelton.Datos.token = tokenValidation;
                        //
                        Response.Cookies["User"]["username"] = userName;
                        Response.Cookies["User"]["privatekey"] = Convert.ToString(PrivateKey);
                        Response.Cookies["User"]["token"] = result;
                        return View();
                    }
                }
            }
            registroValido = 3;
            return RedirectToAction("Index");
        }
        public ActionResult SignUp()
        {
            //Post
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            var levels = Convert.ToInt32(Request.Form["levels"].ToString());
            //Get para verificar que no existe un Usuario con otro Nombre
            var found = false;
            var User = new Users();
            var login = User.GetLogIn();
            foreach (LogInElements elements in login)
            {
                if ((elements.UserName == userName))
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                registroValido = 2;
                return RedirectToAction("Index");
            }
            else
            {
                //método para cifrar la clave ZigZag
                LogInElements elemento = new LogInElements();
                DiffieHellman diffieHellman = new DiffieHellman();
                DateTime now = DateTime.Now;
                var ticks = now.Ticks;
                var ab = (int)(ticks % 17);
                var p = 1021;
                var g = 11;
                var a = Convert.ToString(ab, 2);
                a = a.PadLeft(8, '0');
                //cifrar a en binario
                a = User.ZigZagEncryptionCipher(a, levels);
                //cifrar la contraseña del ususario
                var CipherPassword = User.ZigZagEncryptionCipher(password, levels);
                //generar A con diffie Hellman
                var A = diffieHellman.GenerarClaves(ab, p, g);
                //elemento a cifrar
                elemento.Password = CipherPassword;
                elemento.UserName = userName;
                elemento.A = Convert.ToString(A);
                elemento.PrivateKey = a;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58992/");
                    var postjob = client.PostAsync("api/LogIn", new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
                    postjob.Wait();
                    registroValido = 1;
                }
                return RedirectToAction("Index");
            }
        }
        public ActionResult Modification()
        {
            ////Put
            //using (var client = new HttpClient())
            //{
            //    elemento.Id = id;
            //    client.BaseAddress = new Uri("http://localhost:51508");
            //    var putTask = client.PutAsync("api/LogIn/" + id, new StringContent(new JavaScriptSerializer().Serialize(elemento), Encoding.UTF8, "application/json"));
            //    putTask.Wait();
            //}
            return View();
        }
        public ActionResult DeleteAccount()
        {
            var userName = Request.Form["userName"].ToString();
            var password = Request.Form["password"].ToString();
            var levels = Convert.ToInt32(Request.Form["levels"].ToString());
            //status para cuenta eliminada
            var found = false;
            var User = new Users();
            var login = User.GetLogIn();
            var passwordZigZag = string.Empty;
            foreach (LogInElements elements in login)
            {
                if ((elements.UserName == userName))
                {
                    passwordZigZag = elements.Password;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                var Decipherpassword = User.ZigZagEncryptionDechipher(passwordZigZag, levels);
                if (Decipherpassword == password)
                {
                    return RedirectToAction("Delete", new { userName });

                }
                else
                {
                    registroValido = 5;

                    return RedirectToAction("Index");
                }
            }
            else
            {
                registroValido = 5;
                return RedirectToAction("Index");
            }
        }
        public ActionResult Delete(string userName)
        {
            //Delete
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58992/");
                var deleteTask = client.DeleteAsync("api/LogIn/" + userName);
                deleteTask.Wait();
            }
            registroValido = 4;
            return RedirectToAction("Index");
        }
    }
}