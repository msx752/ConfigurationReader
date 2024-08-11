using MongoDB.Bson;
using System;

namespace ConfigurationLib.Dashboard.Models.Dtos
{
    public class ExceptionLogDto
    {
        public string ApplicationName { get; set; }

        public Guid CorrelationId { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }

        public ObjectId Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}