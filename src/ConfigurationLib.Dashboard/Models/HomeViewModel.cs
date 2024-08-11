using ConfigurationLib.Dashboard.Models.Dtos;
using Shared.Kernel.Models;
using System.Collections.Generic;

namespace ConfigurationLib.Dashboard.Models
{
    /// <summary>
    /// The home view model.
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// The current environment view model.
        /// </summary>
        public class CurrentEnvironmentViewModel
        {
            /// <summary>
            /// Gets or sets the site name.
            /// </summary>
            /// <value>A <see cref="string"/></value>
            public string SiteName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether basket enabled.
            /// </summary>
            /// <value>A <see cref="bool"/></value>
            public bool IsBasketEnabled { get; set; }

            /// <summary>
            /// Gets or sets the max ıtem count.
            /// </summary>
            /// <value>An <see cref="int"/></value>
            public int MaxItemCount { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether disabled error logging.
            /// </summary>
            /// <value>A <see cref="bool"/></value>
            public bool IsloggingEnabled { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether broker message shown.
            /// </summary>
            /// <value>A <see cref="bool"/></value>
            public bool IsBrokerMessageShown { get; set; }
        }

        /// <summary>
        /// Gets or sets the current environment.
        /// </summary>
        /// <value>A <see cref="CurrentEnvironmentViewModel"/></value>
        public CurrentEnvironmentViewModel CurrentEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the exception logs.
        /// </summary>
        /// <value>A list of exceptionlogdtos.</value>
        public List<ExceptionLogDto> ExceptionLogs { get; set; }
    }
}