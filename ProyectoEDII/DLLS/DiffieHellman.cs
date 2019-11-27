using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DLLS
{
    public class DiffieHellman
    {
        public int GenerarClaves(int a, int p, int g)
        {
            var A= (BigInteger.Pow(g, a)) % p;
            return (int)A;
        }
        public int GenerarK(int B, int a)
        {
            var p = 23;
            var K = (BigInteger.Pow(B, a)) % p;
            return (int)K;
        }
    }
}
