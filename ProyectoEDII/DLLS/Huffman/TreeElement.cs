using System;
namespace DLLS.Huffman
{
    public class TreeElement : IComparable
    {
        public char character { get; set; }
        public double probability { get; set; }
        public Node Aux { get; set; }
        public int CompareTo(object obj)
        {
            TreeElement compareToObj = (TreeElement)obj;
            return this.probability.CompareTo(compareToObj.probability);
        }
    }
}
