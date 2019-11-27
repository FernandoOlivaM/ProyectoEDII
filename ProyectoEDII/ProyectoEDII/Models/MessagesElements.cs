using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
namespace ProyectoEDII.Models
{
    public class MessagesElements
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Transmitter { get; set; }
        public string Reciever { get; set; }
        public string text { get; set; }
    }
}
