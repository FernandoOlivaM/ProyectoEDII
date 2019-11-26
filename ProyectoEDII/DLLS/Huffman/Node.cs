namespace DLLS.Huffman
{
    public class Node
    {
        public string character;
        public double probability;
        public Node rightChild;
        public Node leftChild;
        public Node()
        {
            rightChild = null;
            leftChild = null;
        }
    }

}
