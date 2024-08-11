using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ConfigurationLib.Dashboard.Models.Inputs
{
    /// <summary>
    /// The application configuration.
    /// </summary>
    public class ApplicationConfigurationInput
    {
        /// <summary>
        /// Gets or sets the ıd.
        /// </summary>
        /// <value>An <see cref="ObjectId"/></value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        /// <value>A <see cref="bool"/></value>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        [Required]
        public string ApplicationName { get; set; }
    }
}