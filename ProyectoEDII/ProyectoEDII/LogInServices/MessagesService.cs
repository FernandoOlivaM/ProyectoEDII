using MongoDB.Driver;
using ProyectoEDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.LogInServices
{
    public class MessagesService
    {
        private readonly IMongoCollection<MessagesElements> _messages;
        public MessagesService(IMessagesDatabase settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _messages = database.GetCollection<MessagesElements>(settings.MessagesCollectionName);
        }
        //Get
        public List<MessagesElements> GetAll()
        {
            return _messages.Find(x => true).ToList();
        }
        //Insertar
        public MessagesElements Insertar(MessagesElements message)
        {
            _messages.InsertOne(message);
            return message;
        }
    }
}
