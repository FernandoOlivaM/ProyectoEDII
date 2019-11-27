using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.Models
{
    public class MessagesElements
    {
        [BsonElement("Transmitter")]
        public string Transmitter { get; set; }
        [BsonElement("Reciever")]
        public string Reciever { get; set; }
        [BsonElement("text")]
        public string text { get; set; }
    }
}
