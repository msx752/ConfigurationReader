using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Shared.Kernel.Models
{
    public class ExceptionLog
    {
        [BsonElement("ApplicationName")]
        public string ApplicationName { get; set; }

        [BsonElement("CorrelationId")]
        public Guid CorrelationId { get; set; }

        [BsonElement("ExceptionMessage")]
        public string ExceptionMessage { get; set; }

        [BsonElement("ExceptionStackTrace")]
        public string ExceptionStackTrace { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}