using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.Models
{
    public class FilesCompressionElements
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Transmitter { get; set; }
        public string Reciever { get; set; }
        public string direction { get; set; }
    }
}
