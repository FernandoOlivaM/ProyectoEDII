using System;
using System.Collections.Generic;
using System.Linq;

namespace DLLS
{
    public class ZigZag
    {
        //Cifrado
        public List<byte> LecturaCifrado(string password)
        {
            var ByteList = new List<byte>();
            foreach(char caracter in password)
            {
                ByteList.Add((byte)caracter);
            }
            return ByteList;
        }
        public byte[,] MatrixCreation(int conteo, int niveles, ref int CantidadCaracterExtra)
        {
            CantidadCaracterExtra = conteo;
            var CharacterArray = new byte[niveles, conteo];
            var retornar = false;
            var i = 0;
            for (int j = 0; j < conteo; j++)
            {
                if (!retornar)
                {
                    CharacterArray[i, j] = Convert.ToByte('_');
                    i++;
                    if (i == niveles)
                    {
                        retornar = true;
                        i -= 2;
                    }
                }
                else
                {
                    CharacterArray[i, j] = Convert.ToByte('_');
                    i--;
                    if (i < 0)
                    {
                        retornar = false;
                        i += 2;
                    }
                }
            }
            if (CharacterArray[0, conteo - 1] != Convert.ToByte('_'))
            {
                CharacterArray = MatrixCreation(conteo + 1, niveles, ref CantidadCaracterExtra);
            }
            else
            {
                return CharacterArray;
            }
            return CharacterArray;
        }
        public List<byte> AgregarCaracterExtra(List<byte> BytesList, int conteo, ref byte CaracterExtra)
        {
            bool encontrado = false;
            var i = 1;
            while (!encontrado)
            {
                if (!BytesList.Contains(Convert.ToByte(i)))
                {
                    encontrado = true;
                }
                else
                {
                    i++;
                }
            }
            CaracterExtra = Convert.ToByte(i);
            while (BytesList.Count() != conteo)
            {
                BytesList.Add(Convert.ToByte(i));
            }
            return BytesList;
        }
        public List<byte> CifrarMensaje(byte[,] CharacterArray, int niveles, List<byte> BytesList, byte CaracterExtra)
        {
            //proceso para introducir los bytes a la matiz
            var listposition = 0;
            var retornar = false;
            var i = 0;
            for (int j = 0; j < BytesList.Count(); j++)
            {
                if (!retornar)
                {
                    CharacterArray[i, j] = BytesList[listposition];
                    listposition++;
                    i++;
                    if (i == niveles)
                    {
                        retornar = true;
                        i -= 2;
                    }
                }
                else
                {
                    CharacterArray[i, j] = BytesList[listposition];
                    listposition++;
                    i--;
                    if (i < 0)
                    {
                        retornar = false;
                        i += 2;
                    }
                }
            }
            //proceso para obtener los bytes cifrados
            var ByteBuffer = new List<byte>();
            ByteBuffer.Add(CaracterExtra);
            for (i = 0; i < niveles; i++)
            {
                for (int j = 0; j < BytesList.Count(); j++)
                {
                    if (CharacterArray[i, j] != 0)
                    {
                        ByteBuffer.Add(CharacterArray[i, j]);
                    }
                }
            }
            return ByteBuffer;
        }
        //Decifrado
        public List<byte> LecturaDescifrado(string chipherpassword, ref byte CaracterExtra)
        {
            var ByteList = new List<byte>();
            var contador = 0;
            foreach (char caracter in chipherpassword)
            {
                if (contador == 0)
                {
                    CaracterExtra = (byte)caracter;
                    contador++;
                }
                else
                {
                    ByteList.Add((byte)caracter);
                }
            }
            return ByteList;
        }
        public byte[,] MatrixCreationDecryption(int conteo, int niveles, ref int CantidadCaracterExtra)
        {
            CantidadCaracterExtra = conteo;
            var CharacterArray = new byte[niveles, conteo];
            var retornar = false;
            var i = 0;
            for (int j = 0; j < conteo; j++)
            {
                if (!retornar)
                {
                    CharacterArray[i, j] = Convert.ToByte('_');
                    i++;
                    if (i == niveles)
                    {
                        retornar = true;
                        i -= 2;
                    }
                }
                else
                {
                    CharacterArray[i, j] = Convert.ToByte('_');
                    i--;
                    if (i < 0)
                    {
                        retornar = false;
                        i += 2;
                    }
                }
            }
            if (CharacterArray[0, conteo - 1] != Convert.ToByte('_'))
            {
                CharacterArray = MatrixCreationDecryption(conteo + 1, niveles, ref CantidadCaracterExtra);
            }
            else
            {
                return CharacterArray;
            }
            return CharacterArray;
        }
        public string DecifrarMensaje(int niveles, List<byte> BytesList, byte[,] Matrix, byte CaracterExtra)
        {
            var n = niveles - 2;
            var m = (BytesList.Count() + 2 * n + 1) / (2 + 2 * n);
            var CaracteresInferiores = m - 1;
            var CaracteresCentrales = 2 * (m - 1);
            var Position = 0;
            //Introducir los caracteres superiores
            for (int i = 0; i < BytesList.Count(); i++)
            {
                if (Matrix[0, i] == Convert.ToByte('_'))
                {
                    Matrix[0, i] = BytesList[Position];
                    Position++;
                }
            }
            //Introducir los caracteres inferiores
            var CaracteresFinales = string.Empty;
            var j = 0;
            var Contador = BytesList.Count() - 1;
            while (j != CaracteresInferiores)
            {
                CaracteresFinales = (char)BytesList[Contador] + CaracteresFinales;
                Contador--;
                j++;
            }
            var Pozition = 0;
            for (int i = 0; i < BytesList.Count(); i++)
            {
                if (Matrix[niveles - 1, i] == Convert.ToByte('_'))
                {
                    Matrix[niveles - 1, i] = (byte)CaracteresFinales[Pozition];
                    Pozition++;
                }
            }
            //Ingresar los caracteres centrales
            for (int i = 1; i < niveles - 1; i++)
            {
                for (int x = 0; x < BytesList.Count(); x++)
                {
                    if ((Matrix[i, x] == Convert.ToByte('_')))
                    {
                        Matrix[i, x] = BytesList[Position];
                        Position++;
                    }
                }
            }
            //Recorrer la matríz para obtener los caracteress
            var retornar = false;
            var q = 0;
            var buffer = new List<byte>();
            var Colocación = 0;
            for (int y = 0; y < BytesList.Count(); y++)
            {
                if (!retornar)
                {
                    if (Matrix[q, y] != CaracterExtra)
                    {
                        buffer.Add(Matrix[q, y]);
                        Colocación++;
                    }
                    q++;
                    if (q == niveles)
                    {
                        retornar = true;
                        q -= 2;
                    }
                }
                else
                {
                    if (Matrix[q, y] != CaracterExtra)
                    {
                        buffer.Add(Matrix[q, y]);
                        Colocación++;
                    }
                    q--;
                    if (q < 0)
                    {
                        retornar = false;
                        q += 2;
                    }
                }
            }
            var cadena = string.Empty;
            foreach(byte bit in buffer)
            {
                cadena += (char)bit;
            }
            return cadena;
        }
    }
}
