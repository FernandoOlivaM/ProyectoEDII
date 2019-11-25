using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLLS
{
    public class SDES
    {
        //Generar Llaves
        public string GenerarIPInverso(string IP)
        {
            var ReverseIP = string.Empty;
            var contador = 0;
            var Auxiliar = string.Empty;
            for (var i = 0; i < IP.Count(); i++)
            {
                for (var j = 0; j < IP.Count(); j++)
                {
                    Auxiliar += IP[j];
                    if (i != Convert.ToInt32(Auxiliar))
                    {
                        contador++;
                    }
                    else
                    {
                        ReverseIP += Convert.ToString(contador);
                    }
                    Auxiliar = string.Empty;
                }
                contador = 0;
            }
            return ReverseIP;
        }
        public string GenerarLS1(string Key, string P10)
        {
            var resultanteP10 = Permutation(Key, P10);
            var resultanteLS1 = LeftShift1(resultanteP10);
            return resultanteLS1;
        }
        public string GenerarK1(string resultanteLS1, string P8)
        {
            var K1 = Permutation(resultanteLS1, P8);
            return K1;
        }
        public string GenerarK2(string resultanteLS1, string P8)
        {
            var resultanteLS2 = LeftShif2(resultanteLS1);
            var K2 = Permutation(resultanteLS2, P8);
            return K2;
        }
        private string LeftShift1(string resultanteP10)
        {
            var LS1 = resultanteP10.Substring(0, 5);
            var LS2 = resultanteP10.Substring(5);
            //LS al primer substring
            var resultadoLS1 = string.Empty;
            for (var i = 1; i < LS1.Count(); i++)
            {
                resultadoLS1 += LS1[i];
            }
            resultadoLS1 += LS1[0];
            //LS al segundo substring
            var resultadoLS2 = string.Empty;
            for (var i = 1; i < LS2.Count(); i++)
            {
                resultadoLS2 += LS2[i];
            }
            resultadoLS2 += LS2[0];
            var resultanteLS1 = resultadoLS1 + resultadoLS2;
            return resultanteLS1;
        }
        private string LeftShif2(string resultanteLS1)
        {
            var LS1 = resultanteLS1.Substring(0, 5);
            var LS2 = resultanteLS1.Substring(5);
            //LS al primer substring
            var resultadoLS1 = string.Empty;
            for (var i = 2; i < LS1.Count(); i++)
            {
                resultadoLS1 += LS1[i];
            }
            for (var i = 0; i < 2; i++)
            {
                resultadoLS1 += LS1[i];
            }
            //LS al segundo substring
            var resultadoLS2 = string.Empty;
            for (var i = 2; i < LS2.Count(); i++)
            {
                resultadoLS2 += LS2[i];
            }
            for (var i = 0; i < 2; i++)
            {
                resultadoLS2 += LS2[i];
            }
            var resultanteLS2 = resultadoLS1 + resultadoLS2;
            return resultanteLS2;
        }
        private string Permutation(string Key, string permutation)
        {
            var chain = string.Empty;
            var resultanteP = string.Empty;
            foreach (char caracter in permutation)
            {
                chain += caracter;
                resultanteP += Key[Convert.ToInt32(chain)];
                chain = string.Empty;
            }
            return resultanteP;
        }
        //Cifrar O Decifrar
        public List<string> LecturaArchivo(string text)
        {
            var BytesList = new List<string>();
            foreach (char caracter in text)
            {
                var bit = (byte)caracter;
                var binary = Convert.ToString(bit, 2);
                binary = binary.PadLeft(8, '0');
                BytesList.Add(binary);
            }
            return BytesList;
        }
        public byte CifrarODecifrar(string binary, string IP, string EP, string K1, string P4, string K2, string ReverseIP, bool cifrado)
        {
            var resultanteIP = Permutation(binary, IP);
            var resultanteIP1 = resultanteIP.Substring(0, 4);
            var resultanteIP2 = resultanteIP.Substring(4);
            var resultanteEP = Permutation(resultanteIP2, EP);
            var resultanteXOR = cifrado ? XOR(resultanteEP, K1) : XOR(resultanteEP, K2);
            var S1 = resultanteXOR.Substring(0, 4);
            var S2 = resultanteXOR.Substring(4);
            var Sboxes = SBoxes(S1, S2);
            var resultanteP4 = Permutation(Sboxes, P4);
            resultanteXOR = XOR(resultanteP4, resultanteIP1);
            var union = resultanteXOR + resultanteIP2;
            var resultanteSWAP1 = union.Substring(4);
            var resultanteSWAP2 = union.Substring(0, 4);
            resultanteEP = Permutation(resultanteSWAP2, EP);
            resultanteXOR = cifrado ? XOR(resultanteEP, K2) : XOR(resultanteEP, K1);
            S1 = resultanteXOR.Substring(0, 4);
            S2 = resultanteXOR.Substring(4);
            Sboxes = SBoxes(S1, S2);
            resultanteP4 = Permutation(Sboxes, P4);
            resultanteXOR = XOR(resultanteP4, resultanteSWAP1);
            union = resultanteXOR + resultanteSWAP2;
            var resultanteReverseIP = Permutation(union, ReverseIP);
            byte bytefinal = Convert.ToByte(Convert.ToInt32(resultanteReverseIP, 2));
            return bytefinal;
        }
        private string XOR(string resultante, string clave)
        {
            var resultanteXOR = string.Empty;
            for (var i = 0; i < resultante.Count(); i++)
            {
                resultanteXOR += resultante[i] == clave[i] ? "0" : "1";
            }
            return resultanteXOR;
        }
        private string SBoxes(string S1, string S2)
        {
            string[,] matrizS0 = { { "01", "00", "11", "10" }, { "11", "10", "01", "00" }, { "00", "10", "01", "11" }, { "11", "01", "11", "10" } };
            string[,] matrizS1 = { { "00", "01", "10", "11" }, { "10", "00", "01", "11" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" } };
            //valores S1
            var FS0 = string.Empty;
            FS0 += S1[0];
            FS0 += S1[3];
            var F0 = Convert.ToInt32(FS0, 2);
            var CS0 = string.Empty;
            CS0 += S1[1];
            CS0 += S1[2];
            var C0 = Convert.ToInt32(CS0, 2);
            //valores S2
            var FS1 = string.Empty;
            FS1 += S2[0];
            FS1 += S2[3];
            var F1 = Convert.ToInt32(FS1, 2);
            var CS1 = string.Empty;
            CS1 += S1[1];
            CS1 += S2[2];
            var C1 = Convert.ToInt32(CS1, 2);
            var Sboxes = matrizS0[F0, C0] + matrizS1[F1, C1];
            return Sboxes;
        }
    }
}
