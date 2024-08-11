using MassTransit;
using MongoDB.Bson;
using System;

namespace Shared.Kernel.Messages
{
    public class ExceptionLogEvent : CorrelatedBy<Guid>
    {
        public ExceptionLogEvent()
        {
            Timestamp = DateTime.UtcNow;
        }

        public string ApplicationName { get; set; }

        public Guid CorrelationId { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }

        public ObjectId Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}