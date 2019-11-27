using MongoDB.Driver;
using ProyectoEDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.LogInServices
{
    public class FilesCompressionService
    {
        private readonly IMongoCollection<FilesCompressionElements> _files;
        public FilesCompressionService(IFilesCompressionDatabase settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _files = database.GetCollection<FilesCompressionElements>(settings.FilesCompressionCollectionName);
        }
        //Get
        public List<FilesCompressionElements> GetAll()
        {
            return _files.Find(x => true).ToList();
        }
        //Insertar
        public FilesCompressionElements Insertar(FilesCompressionElements file)
        {
            _files.InsertOne(file);
            return file;
        }
    }
}
