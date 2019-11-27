using System;
using System.Collections.Generic;
using System.Text;

namespace DLLS.Huffman
{
    public class charCount : IComparable
    {
        public int cantidad { get; set; }
        public string codPref { get; set; }
        public int CompareTo(object obj)
        {
            charCount compareToObj = (charCount)obj;
            return this.cantidad.CompareTo(compareToObj.cantidad);
        }
    }
}
