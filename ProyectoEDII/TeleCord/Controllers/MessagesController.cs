using DLLS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TeleCord.Models;
using DLLS.Huffman;
namespace TeleCord.Controllers
{
    public class MessagesController : Controller
    {
        static Dictionary<char, charCount> diccionario = new Dictionary<char, charCount>();
        static string RutaArchivos = string.Empty;
        static string nombreArchivo = string.Empty;
        static List<byte> ListaByte = new List<byte>();
        static List<TreeElement> lista = new List<TreeElement>();
        //largo del buffer al momento de la lectura
        const int bufferLengt = 1000000;
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
            bool Exists;
            string Paths = Server.MapPath("~/Files/");
            Exists = Directory.Exists(Paths);
            if (!Exists)
            {
                Directory.CreateDirectory(Paths);
            }
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath("");
                //se toma la ruta y nombre del archivo
                string ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                Tree send = new Tree();
                nombreArchivo = Path.GetFileName(postedFile.FileName).Substring(0, Path.GetFileName(postedFile.FileName).IndexOf("."));
                RutaArchivos = rutaDirectorioUsuario;
                send.UserPaths(rutaDirectorioUsuario, nombreArchivo);
                postedFile.SaveAs(ArchivoLeido);
                //se aplicó la interfaz y el modelo Huffman
                //La Listabyte se utilizó una referencia debido a que la función retornará el diccionario
                Huffman HuffmanProcess = new Huffman();
                diccionario = HuffmanProcess.LecturaArchivoCompresion(diccionario, ArchivoLeido, bufferLengt, ref ListaByte);
                lista = HuffmanProcess.OrdenamientoDelDiccionario(diccionario,lista,ListaByte);
                return RedirectToAction("Arbol");
            }
            else {
                var a = 22;
                var b = 105;
                var g = 15;
                var p = 23;
                DiffieHellman diffiehellman = new DiffieHellman();
                var K = diffiehellman.GenerarClaves(a, b, g, p);
                var Key = Convert.ToString(a, 2);
                Key = Key.PadLeft(10, '0');
                return RedirectToAction("Cifrar", new { Key, message });
            }
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
        private static charCount GetAnyValue<T>(byte Key)
        {
            charCount obj;
            charCount retType;
            diccionario.TryGetValue((char)Key, out obj);
            try
            {
                retType = (charCount)obj;
            }
            catch
            {
                retType = default(charCount);
            }
            return retType;
        }
        public ActionResult Arbol()
        {
            //creación del árbol
            Huffman HuffmanProcess = new Huffman();
            Tree Arbol = new Tree();
            Arbol.root = HuffmanProcess.TreeCreation(lista);
            string prefíjo = string.Empty;
            diccionario = Arbol.prefixCodes(Arbol.root, diccionario, prefíjo);
            //Escritura del compresor códigos prefíjos convertidos a bytes
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\" + nombreArchivo +".huff", FileMode.Open))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    byte[] bytebuffer = new byte[500];
                    List<char> cadena = new List<char>();
                    int cantidadbuffer = 0;
                    foreach (byte bit in ListaByte)
                    {
                        charCount separación = new charCount();
                        separación = GetAnyValue<int>(bit);
                        foreach (char caracter in separación.codPref)
                        {
                            cadena.Add(caracter);
                        }
                    }
                    string binario = "";
                    foreach (char car in cadena)
                    {
                        if (binario.Count() == 8)
                        {
                            byte DECABYTE = new byte();
                            var pref = binario;
                            decimal x = Convert.ToInt32(pref, 2);
                            DECABYTE = Convert.ToByte(x);
                            bytebuffer[cantidadbuffer] = DECABYTE;
                            cantidadbuffer++;
                            binario = string.Empty;
                            binario = binario + car;
                        }
                        else
                        {
                            binario = binario + car;
                        }
                        if (cantidadbuffer == 500)
                        {
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(bytebuffer);
                            cantidadbuffer = 0;
                            bytebuffer = new byte[500];
                        }
                    }
                    if (binario != string.Empty)
                    {
                        while (binario.Count() != 8)
                        {
                            binario = binario + "0";
                        }
                        byte DECABYTE = new byte();
                        var pref = binario;
                        decimal x = Convert.ToInt32(pref, 2);
                        DECABYTE = Convert.ToByte(x);
                        bytebuffer[cantidadbuffer] = DECABYTE;
                        int contador0 = 0;
                        List<byte> ListAux = new List<byte>();
                        foreach (byte bit in bytebuffer)
                        {
                            if (bit == 0)
                            {
                                contador0++;
                            }
                            else
                            {
                                contador0 = 0;
                            }
                            if (contador0 != 10)
                            {
                                ListAux.Add(bit);
                            }
                            else
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    ListAux.Remove(ListAux.Last());
                                }
                                break;
                            }
                        }
                        bytebuffer = new byte[ListAux.Count()];
                        int j = 0;
                        foreach (byte bit in ListAux)
                        {
                            bytebuffer[j] = bit;
                            j++;
                        }
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(bytebuffer);
                    }

                }
            }
            return RedirectToAction("Download");
        }


    }
}