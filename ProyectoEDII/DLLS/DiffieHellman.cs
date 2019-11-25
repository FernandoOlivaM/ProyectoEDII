using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DLLS
{
    public class DiffieHellman
    {
        public int GenerarClaves(int a, int b, int p, int g)
        {
            var A = (BigInteger.Pow(g,a))%p;
            var B = (BigInteger.Pow(g, b)) % p;
            var K = (BigInteger.Pow(B, a)) % p;
            var K2 = (BigInteger.Pow(A, b)) % p;
            return (int)K;
        }
    }
}
