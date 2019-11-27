using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DLLS.Huffman
{
   public class Huffman
    {
        public Dictionary<char, charCount> LecturaArchivoCompresion(Dictionary<char, charCount> diccionario, string ArchivoLeido, int bufferLengt, ref List<byte> ListaByte)
        {
            //el siguiente if permite seleccionar un archivo en específico
            using (var stream = new FileStream(ArchivoLeido, FileMode.Open))
            {
                //te va a devolver un numero cualquiera
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLengt];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLengt);
                        foreach (byte bit in byteBuffer)
                        {
                            charCount cantidad = new charCount();

                            if (!diccionario.ContainsKey((char)bit))
                            {
                                cantidad.cantidad = 1;
                                diccionario.Add((char)bit, cantidad);
                            }
                            else
                            {
                                diccionario[(char)bit].cantidad++;
                            }
                            ListaByte.Add(bit);
                        }
                    }
                }
            }
            return diccionario;
        }

        public List<TreeElement> OrdenamientoDelDiccionario(Dictionary<char, charCount> diccionario, List<TreeElement> ListaProbabilidades, List<byte> ListaByte)
        {
            //se ordenará por orden ascendente la lista
            var sorted = from entrada in diccionario orderby entrada.Value ascending select entrada;
            //se introducirán los porcentajes de los caracteres en la tabla
            foreach (var caracter in sorted)
            {
                TreeElement elemento = new TreeElement();
                double aux = (Convert.ToDouble(caracter.Value.cantidad));
                elemento.character = caracter.Key;
                elemento.probability = Convert.ToDouble((aux / ListaByte.Count()));
                ListaProbabilidades.Add(elemento);
            }
            ListaProbabilidades.Sort();
            return ListaProbabilidades;
        }

        public Node TreeCreation(List<TreeElement> lista)
        {
            Tree Auxiliar = new Tree();
            Node Aux = new Node();
            Node izquierdo = new Node();
            Node derecho = new Node();
            int Repeticiones = lista.Count();
            for (int i = 0; i < Repeticiones; i++)
            {
                if (lista.Count < 2)
                {
                    break;
                }
                else
                {
                    Auxiliar = new Tree();
                    Aux = new Node();
                    izquierdo = new Node();
                    derecho = new Node();
                    string nombre = "n" + (i + 1);
                    if (lista[0].Aux == null && lista[1].Aux == null)
                    {
                        //hijo izquierdo
                        izquierdo.character = Convert.ToString(lista[0].character);
                        izquierdo.probability = lista[0].probability;
                        //hijo derecho
                        derecho.character = Convert.ToString(lista[1].character);
                        derecho.probability = lista[1].probability;
                    }
                    else
                    {
                        if (lista[0].Aux != null && lista[1].Aux == null)
                        {
                            //hijo izquierdo
                            izquierdo = lista[0].Aux;
                            //hijo derecho
                            derecho.character = Convert.ToString(lista[1].character);
                            derecho.probability = lista[1].probability;
                        }
                        else
                        {
                            if (lista[0].Aux == null && lista[1].Aux != null)
                            {
                                //hijo izquierdo
                                izquierdo.character = Convert.ToString(lista[0].character);
                                izquierdo.probability = lista[0].probability;
                                //hijo derecho
                                derecho = lista[1].Aux;
                            }
                            else
                            {
                                //hijo izquierdo
                                izquierdo = lista[0].Aux;
                                //hijo derecho
                                derecho = lista[1].Aux;
                            }
                        }
                    }
                    lista.Remove(lista[0]);
                    lista[0] = null;
                    Aux = Auxiliar.insert(izquierdo, derecho, nombre);
                    TreeElement elemento = new TreeElement();
                    elemento.Aux = Aux;
                    elemento.probability = Aux.probability;
                    if (lista.Count() > 1)
                    {
                        for (int j = 1; j < lista.Count(); j++)
                        {
                            if (lista[j].probability > elemento.probability)
                            {
                                lista[j - 1] = elemento;
                                break;
                            }
                            else
                            {
                                lista[j - 1] = lista[j];
                                lista[j] = null;
                                if (lista[lista.Count() - 1] == null)
                                {
                                    lista[lista.Count() - 1] = elemento;
                                }
                            }
                        }
                    }
                    else
                    {
                        lista[0] = elemento;
                    }
                }
            }
            return lista[0].Aux;
        }

    }
}
