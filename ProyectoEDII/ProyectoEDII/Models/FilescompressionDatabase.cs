using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.Models
{
    public class FilescompressionDatabase:IFilesCompressionDatabase
    {
        public string FilesCompressionCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IFilesCompressionDatabase
    {
        string FilesCompressionCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
