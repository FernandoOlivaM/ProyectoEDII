using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DLLS.Huffman
{
    public class TreeElements:IComparable
    {
        public char character { get; set; }
        public double probability { get; set; }
        public Node Aux { get; set; }
        public int CompareTo(object obj)
        {
            TreeElements compareToObj = (TreeElements)obj;
            return this.probability.CompareTo(compareToObj.probability);
        }
    
    }
}
