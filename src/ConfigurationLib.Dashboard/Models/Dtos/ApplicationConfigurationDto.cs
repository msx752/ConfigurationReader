using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationLib.Models.Dtos
{
    public class ApplicationConfigurationDto
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        /// <value>A <see cref="bool"/></value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        /// <value>A <see cref="string"/></value>
        public string ApplicationName { get; set; }
    }
}
