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
using TeleCord.datos;
using TeleCord.Models;

namespace TeleCord.Controllers
{
    public class FilesCompressionController : Controller
    {
        static Dictionary<char, charCount> diccionario = new Dictionary<char, charCount>();
        static string RutaArchivos = string.Empty;
        static string nombreArchivo = string.Empty;
        static List<byte> ListaByte = new List<byte>();
        static List<TreeElements> lista = new List<TreeElements>();
        const int bufferLengt = 1000000;
        // GET: FilesCompression
        public ActionResult Index()
        {
            var UsersList = new List<Users>();
            var User = new Users();
            var login = User.GetLogIn();
            foreach (LogInElements loggers in login)
            {
                var Users = new Users();
                Users.UserName = loggers.UserName;
                UsersList.Add(Users);
            }
            return View(UsersList);
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
                nombreArchivo = Path.GetFileName(postedFile.FileName).Substring(0, Path.GetFileName(postedFile.FileName).IndexOf("."));
                RutaArchivos = rutaDirectorioUsuario;
                send.UserPaths(rutaDirectorioUsuario, nombreArchivo);
                postedFile.SaveAs(ArchivoLeido);
                Huffman HuffmanProcess = new Huffman();
                diccionario = HuffmanProcess.LecturaArchivoCompresion(diccionario, ArchivoLeido, bufferLengt, ref ListaByte);
                lista = HuffmanProcess.OrdenamientoDelDiccionario(diccionario, lista, ListaByte);
                var FileElement = new FilesCompressionElements();
                FileElement.direction = postedFile.FileName;
                FileElement.Reciever = ToUser;
                FileElement.Transmitter = datosSingelton.Datos.Nombre;
                var User = new Users();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51508");
                    var postjob = client.PostAsync("api/FilesCompression", new StringContent(new JavaScriptSerializer().Serialize(FileElement), Encoding.UTF8, "application/json"));
                    postjob.Wait();
                }
                return RedirectToAction("Arbol");
            }
            return RedirectToAction("Index");
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
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\" + nombreArchivo + ".huff", FileMode.Open))
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
        [HttpPost]
        public ActionResult LecturaDescompresión(HttpPostedFileBase postedFile)
        {
            nombreArchivo = Path.GetFileName(postedFile.FileName).Substring(0, Path.GetFileName(postedFile.FileName).IndexOf("."));

            diccionario = new Dictionary<char, charCount>();
            using (var stream = new FileStream(RutaArchivos + "\\..\\Files\\" + nombreArchivo + ".huff", FileMode.Open))
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
            return RedirectToAction("GeneraciónDelArchivoOriginal");
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
        public ActionResult GeneraciónDelArchivoOriginal()
        {
            string binario = string.Empty;
            string texto = string.Empty;
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
            using (var writeStream = new FileStream(RutaArchivos + "\\..\\Files\\" + nombreArchivo + ".huff", FileMode.OpenOrCreate))
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
            return RedirectToAction("Download");
        }
    }
}