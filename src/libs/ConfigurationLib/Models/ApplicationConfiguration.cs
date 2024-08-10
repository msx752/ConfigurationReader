using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConfigurationLib.Models
{
    /// <summary>
    /// The application configuration.
    /// </summary>
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Gets or sets the ıd.
        /// </summary>
        /// <value>An <see cref="ObjectId"/></value>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [BsonElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [BsonElement("Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [BsonElement("Value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        /// <value>A <see cref="bool"/></value>
        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [BsonElement("ApplicationName")]
        public string ApplicationName { get; set; }
    }
}