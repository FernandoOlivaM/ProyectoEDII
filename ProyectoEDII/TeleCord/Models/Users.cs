using DLLS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TeleCord.Models
{
    public class Users
    {
        public string UserName { get; set; }
        public IEnumerable<LogInElements> GetLogIn()
        {
            IEnumerable<LogInElements> login = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58992/");
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
            return login;
        }
        public IEnumerable<FilesCompressionElements> GetFiles()
        {
            IEnumerable<FilesCompressionElements> files = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58992/");
                var responseTask = client.GetAsync("api/FilesCompression");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    files = JsonConvert.DeserializeObject<IList<FilesCompressionElements>>(readTask.Result);
                }
                else
                {
                    files = Enumerable.Empty<FilesCompressionElements>();
                }
            }
            return files;
        }
        public IEnumerable<MessagesElements> GetMessages()
        {
            IEnumerable<MessagesElements> messages = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58992/");
                var responseTask = client.GetAsync("api/Messages");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    messages = JsonConvert.DeserializeObject<IList<MessagesElements>>(readTask.Result);
                }
                else
                {
                    messages = Enumerable.Empty<MessagesElements>();
                }
            }
            return messages;
        }
        public int ObtenerB(string userName)
        {
            IEnumerable<LogInElements> login = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58992/");
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
            var B = 0;
            foreach (LogInElements loggers in login)
            {
                if (loggers.UserName == userName)
                {
                    B = Convert.ToInt32(loggers.A);
                }
            }
            return B;
        }
        public string ZigZagEncryptionDechipher(string Password,int levels)
        {
            ZigZag cifradoZigZag = new ZigZag();
            var CaracterExtra = new byte();
            var BytesList = cifradoZigZag.LecturaDescifrado(Password, ref CaracterExtra);
            var CantidadCaracterExtra = 0;
            var Matrix = cifradoZigZag.MatrixCreationDecryption(BytesList.Count(), levels, ref CantidadCaracterExtra);
            var CaracterExtra2 = new byte();
            if (CantidadCaracterExtra > BytesList.Count())
            {
                BytesList = cifradoZigZag.AgregarCaracterExtra(BytesList, CantidadCaracterExtra, ref CaracterExtra2);
            }
            var Decipherpassword = cifradoZigZag.DecifrarMensaje(levels, BytesList, Matrix, CaracterExtra);
            return Decipherpassword;
        }
        public string ZigZagEncryptionCipher(string a, int levels)
        {
            ZigZag cifradoZigZag = new ZigZag();
            var BytesList = cifradoZigZag.LecturaCifrado(a);
            var CantidadCaracterExtra = 0;
            var Matrix = cifradoZigZag.MatrixCreation(BytesList.Count(), levels, ref CantidadCaracterExtra);
            var CaracterExtra = new byte();
            if (CantidadCaracterExtra > BytesList.Count())
            {
                BytesList = cifradoZigZag.AgregarCaracterExtra(BytesList, CantidadCaracterExtra, ref CaracterExtra);
            }
            BytesList = cifradoZigZag.CifrarMensaje(Matrix, levels, BytesList, CaracterExtra);
            a = string.Empty;
            foreach (byte bit in BytesList)
            {
                a += (char)bit;
            }
            return a;
        }
    }
}