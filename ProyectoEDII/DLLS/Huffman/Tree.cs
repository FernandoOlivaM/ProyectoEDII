using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DLLS.Huffman
{
    public class Tree
    {
        static string FileRoute = string.Empty;
        static string FileName = string.Empty;
        public void UserPaths(string route, string name)
        {
            FileRoute = route;
            FileName = name;
        }
        public Node root;
        const int bufferLength = 100000;
        int usedSlots = 0;
        byte[] buffer = new byte[bufferLength];
        int charCount = 0;
        int nodeCount = 0;

        public Tree()
        {
            root = null;
        }
        public Node insert(Node left, Node right, string name)
        {
            root = new Node();
            root.rightChild = right;
            root.leftChild = left;
            root.character = name;
            root.probability = right.probability + left.probability;
            return root;
        }

        public void createDictionaryFile(string character, string prefix, Dictionary<char, charCount> codesDictionary)
        {
            charCount++;
            string line = $"{character}|{prefix}";
            for (int i = 0; i < line.Length; i++)
            {
                buffer[usedSlots] = Convert.ToByte(line[i]);
                usedSlots++;
            }
            if (charCount == codesDictionary.Count())
            {
                buffer[usedSlots] = Convert.ToByte('-');
                buffer[usedSlots + 1] = Convert.ToByte('-');
                int count = 0;
                using (var writeStream = new FileStream(FileRoute + "\\..\\Files\\" + FileName + ".huff", FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        for (int j = 0; j < buffer.Count(); j++)
                        {
                            if (j == usedSlots)
                            {
                                writer.Write("\r\n");
                                writer.Write(buffer[j]);
                                writer.Write(buffer[j + 1]);
                                writer.Write("\r\n");
                                break;
                            }
                            if (buffer[j + 1] == 124)
                            {
                                if (count != 0)
                                {
                                    writer.Write(buffer[j]);
                                }
                                else
                                {
                                    writer.Write(buffer[j]);
                                    count++;
                                }
                            }
                            else
                            {
                                writer.Write(buffer[j]);
                            }
                        }
                    }
                }
            }
        }
        public Dictionary<char, charCount> prefixCodes(Node root, Dictionary<char, charCount> codesDictionary, string prefixCode)
        {
            if (root == null)
            {
                return codesDictionary;
            }
            else
            {
                codesDictionary = prefixCodes(root.leftChild, codesDictionary, prefixCode + 0);
                if (root.rightChild == null && root.leftChild == null)
                {
                    if (codesDictionary.ContainsKey(Convert.ToChar(root.character)))
                    {
                        charCount cantidad = new charCount();
                        cantidad.codPref = prefixCode;
                        codesDictionary.Remove(Convert.ToChar(root.character));
                        codesDictionary.Add(Convert.ToChar(root.character), cantidad);
                        nodeCount++;
                        createDictionaryFile(root.character, prefixCode, codesDictionary);
                    }
                }
                codesDictionary = prefixCodes(root.rightChild, codesDictionary, prefixCode + 1);
            }
            return codesDictionary;
        }
    }
}
