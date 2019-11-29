using DLLS.Huffman;
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
using DLLS;
namespace TeleCord.Controllers
{
    public class FilesCompressionController : Controller
    {
        static int archivoComprimido = 0;
        static Dictionary<char, charCount> diccionario = new Dictionary<char, charCount>();
        static List<byte> ListaByte = new List<byte>();
        static List<TreeElements> lista = new List<TreeElements>();
        const int bufferLengt = 1000000;
        // GET: FilesCompression
        public ActionResult Index()
        {
            var active = false;
            var result = Request.Cookies["User"]["token"];
            var tokenValidation = TokenManager.ValidateToken(result);
            if(tokenValidation != null && tokenValidation.ValidTo < DateTime.UtcNow)
            {
                    active = true;
            }
            else {
                RedirectToAction("Index", "LogIn");
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
                ViewBag.status = archivoComprimido;
                return View(UsersList);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        public ActionResult Decompress()
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
        public ActionResult DecompressFile(string fileName)
        {
            var rutaDirectorioUsuario = Server.MapPath("");         
            return RedirectToAction("LecturaDescompresion", new { rutaDirectorioUsuario, fileName });
        }
        public ActionResult ShowFiles()
        {
            var ToUser = Request.Form["UserList"].ToString();
            return RedirectToAction("ShowFilesFromUser", new { ToUser });
        }
        public ActionResult ShowFilesFromUser(string ToUser)
        {
            var User = new Users();

            var files = User.GetFiles();
            var StringList = new List<string>();
            var ToRList = new List<string>();
            var byteList = new List<FilesCompressionElements>();
            var counter = 0;
            foreach (FilesCompressionElements elements in files)
            {
                if ((elements.Transmitter == Request.Cookies["User"]["username"]) && (elements.Reciever == ToUser))
                {
                    StringList.Add(elements.direction);
                    ToRList.Add("1");
                }
                else
                {
                    if ((elements.Transmitter == ToUser) && (elements.Reciever == Request.Cookies["User"]["username"]))
                    {
                        StringList.Add(elements.direction);
                        ToRList.Add("0");
                    }
                }
            }

            foreach (string element in ToRList)
            {
                var File = new FilesCompressionElements();
                File.Transmitter = ToRList[counter] == "1" ? Request.Cookies["User"]["username"] : ToUser;
                File.Reciever = ToRList[counter] == "1" ? ToUser : Request.Cookies["User"]["username"];
                File.direction = StringList[counter];
                byteList.Add(File);
                counter++;
            }



                return View(byteList);
        }
        [HttpPost]
        public ActionResult RecieveFile(HttpPostedFileBase postedFile)
        {
            var ToUser = Request.Form["UserList"].ToString();
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
                string ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                Tree send = new Tree();
                var fileName = Path.GetFileName(postedFile.FileName).Substring(0, Path.GetFileName(postedFile.FileName).IndexOf("."));
                send.UserPaths(rutaDirectorioUsuario, fileName);
                postedFile.SaveAs(ArchivoLeido);
                Huffman HuffmanProcess = new Huffman();
                diccionario = HuffmanProcess.LecturaArchivoCompresion(diccionario, ArchivoLeido, bufferLengt, ref ListaByte);
                lista = HuffmanProcess.OrdenamientoDelDiccionario(diccionario, lista, ListaByte);
                var FileElement = new FilesCompressionElements();
                FileElement.direction = postedFile.FileName;
                FileElement.Reciever = ToUser;
                FileElement.Transmitter = Request.Cookies["User"]["username"];
                var User = new Users();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58992");
                    var postjob = client.PostAsync("api/FilesCompression", new StringContent(new JavaScriptSerializer().Serialize(FileElement), Encoding.UTF8, "application/json"));
                    postjob.Wait();
                }
                return RedirectToAction("Arbol", new { rutaDirectorioUsuario, fileName });
            }
            return RedirectToAction("Index");
        }
        public ActionResult Arbol(string rutaDirectorioUsuario, string fileName)
        {
            //creación del árbol
            Huffman HuffmanProcess = new Huffman();
            Tree Arbol = new Tree();
            Arbol.root = HuffmanProcess.TreeCreation(lista);
            string prefíjo = string.Empty;
            diccionario = Arbol.prefixCodes(Arbol.root, diccionario, prefíjo);
            //Escritura del compresor códigos prefíjos convertidos a bytes
            using (var writeStream = new FileStream(rutaDirectorioUsuario + "\\..\\Files\\" + fileName + ".huff", FileMode.Open))
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
            archivoComprimido = 1;
            return RedirectToAction("Index");
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
        static List<byte> usedChars = new List<byte>();
        //Decifrar
        public ActionResult LecturaDescompresion(string rutaDirectorioUsuario, string fileName)
        {
            usedChars =  new List<byte>();
            diccionario = new Dictionary<char, charCount>();
            var fileNameNoExtension = fileName.Substring(0, fileName.IndexOf("."));

            using (var stream = new FileStream(rutaDirectorioUsuario + "\\..\\Files\\" + fileNameNoExtension + ".huff", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    string prefijos = string.Empty;
                    char caracter = ' ';
                    byte[] byteBuffer = new byte[bufferLengt];
                    bool encontrado = false;
                    bool separador = false;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        for (int i = 0; i < byteBuffer.Count(); i++)
                        {
                            if (separador != true)
                            {
                                if ((byteBuffer[i] == 45))
                                {
                                    if ((byteBuffer[i + 1] == 45))
                                    {
                                        separador = true;
                                        i = i + 2;
                                    }
                                }
                                if (encontrado == false)
                                {
                                    if (byteBuffer[i] == 124)
                                    {
                                        caracter = (char)byteBuffer[i - 1];
                                        encontrado = true;
                                    }
                                }
                                else
                                {
                                    if ((byteBuffer[i + 1] != 124) && (byteBuffer[i] != 2))
                                    {
                                        prefijos += (char)byteBuffer[i];
                                    }
                                    else
                                    {
                                        charCount prefijo = new charCount();
                                        prefijo.codPref = prefijos;
                                        if (prefijo.codPref[0] == '|')
                                        {
                                            string prueba = string.Empty;
                                            for (int j = 1; j < prefijo.codPref.Count(); j++)
                                            {
                                                prueba = prueba + prefijo.codPref[j];
                                            }
                                            prefijo.codPref = prueba;
                                        }
                                        diccionario.Add(caracter, prefijo);
                                        encontrado = false;
                                        prefijos = "";
                                    }
                                }
                            }
                            else
                            {
                                usedChars.Add(byteBuffer[i]);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            {
                usedChars.Remove(usedChars[0]);
            }
            return RedirectToAction("GeneraciónDelArchivoOriginal", new { rutaDirectorioUsuario, fileName });
        }
        public string Convertir(byte bit, string binario)
        {
            bit = Convert.ToByte(int.Parse(Convert.ToString(bit)));
            while (true)
            {
                if ((bit % 2) != 0)
                {
                    binario = "1" + binario;

                }
                else
                {
                    binario = "0" + binario;
                }
                bit /= 2;
                if (bit <= 0)
                {
                    break;
                }
            }
            if (binario.Count() <= 8)
            {
                while (binario.Count() != 8)
                {
                    binario = "0" + binario;
                }
            }
            return binario;
        }
        public ActionResult GeneraciónDelArchivoOriginal(string rutaDirectorioUsuario, string fileName)
        {
            var fileNameNoExtension = fileName.Substring(0, fileName.IndexOf("."));
            var binario = string.Empty;
            var texto = string.Empty;
            charCount valor = new charCount();
            foreach (byte bit in usedChars)
            {
                binario = string.Empty;
                binario = binario + Convertir(bit, binario);
                foreach (char car in binario)
                {
                    valor.codPref = valor.codPref + car;
                    foreach (char Key in diccionario.Keys)
                    {
                        charCount valor2 = GetAnyValue<charCount>(Convert.ToByte(Key));
                        if (valor.codPref == valor2.codPref)
                        {
                            texto = texto + Key;
                            valor.codPref = string.Empty;
                        }
                    }
                }
            }
            using (var writeStream = new FileStream(rutaDirectorioUsuario + "\\..\\Files\\" + fileName, FileMode.Create))
            {
                using (var writer = new BinaryWriter(writeStream))
                {
                    int cantidadvecesbuffer = 0;
                    byte[] byteBufferfinal = new byte[100];
                    int cantidad = 0;
                    foreach (char carfinal in texto)
                    {
                        byteBufferfinal[cantidad] = Convert.ToByte(carfinal);
                        cantidad++;
                        if (cantidad == 100)
                        {
                            if (cantidadvecesbuffer == 0)
                            {
                                writer.Write(byteBufferfinal);
                                byteBufferfinal = new byte[100];
                                cantidadvecesbuffer++;
                                cantidad = 0;
                            }
                            else
                            {
                                writer.Seek(0, SeekOrigin.End);
                                writer.Write(byteBufferfinal);
                                byteBufferfinal = new byte[100];
                                cantidad = 0;
                            }
                        }
                    }
                    if ((byteBufferfinal[0] != 0) && (byteBufferfinal[1] != 0))
                    {
                        int contador0 = 0;
                        List<byte> ListAux = new List<byte>();
                        foreach (byte bit in byteBufferfinal)
                        {
                            if (bit == 0)
                            {
                                contador0++;
                            }
                            else
                            {
                                contador0 = 0;
                            }
                            if (contador0 != 3)
                            {
                                ListAux.Add(bit);
                            }
                            else
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    ListAux.Remove(ListAux.Last());
                                }
                                break;
                            }
                        }
                        byteBufferfinal = new byte[ListAux.Count()];
                        int j = 0;
                        foreach (byte bite in ListAux)
                        {
                            byteBufferfinal[j] = bite;
                            j++;
                        }
                        writer.Seek(0, SeekOrigin.End);
                        writer.Write(byteBufferfinal);
                    }
                }
            }
            return RedirectToAction("Download", new { rutaDirectorioUsuario, fileName });
        }
        public ActionResult Download(string rutaDirectorioUsuario, string fileName)
        {
            var fullPath = rutaDirectorioUsuario + "\\..\\Files\\" + fileName;
            return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

    }
}