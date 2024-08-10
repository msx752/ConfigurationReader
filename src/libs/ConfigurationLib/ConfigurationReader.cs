using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigLib.Abstracts;
using ConfigLib.Interfaces;

namespace ConfigLib
{
    /// <summary>
    /// The configuration reader.
    /// </summary>
    public sealed class ConfigurationReader : MongoDbConfigurationReader, IConfigurationReader
    {
        /// <summary>
        /// The application name.
        /// </summary>
        private readonly string _applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReader"/> class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="refreshTimerIntervalInMs">The refresh timer ınterval ın ms.</param>
        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
            : base(connectionString, refreshTimerIntervalInMs)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or empty.", nameof(applicationName));

            _applicationName = applicationName;

            StartTimer();
        }

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        protected override async Task TriggerAsync()
        {
            using var cursor = await ListConfigurationByApplicationNameAsync(_applicationName);

            var previousKeys = Collection.Keys.ToHashSet();
            HashSet<string> newKeys = new();

            while (await cursor.MoveNextAsync())
            {
                foreach (var applicationConfiguration in cursor.Current)
                {
                    try
                    {
                        string configName = applicationConfiguration.Name?.Trim();

                        if (string.IsNullOrWhiteSpace(configName))
                            continue;

                        Type type = GetSupportedTypeByStringType(applicationConfiguration.Type);

                        if (type == null)
                            continue;

                        object configValue = Convert.ChangeType(applicationConfiguration.Value, type);

                        Collection.AddOrUpdate(configName, configValue, (k, v) => configValue);

                        newKeys.Add(configName);
                    }
                    catch
                    {
                    }
                }
            }

            var listDeleteKeys = previousKeys.Except(newKeys);

            foreach (var deleteKey in listDeleteKeys)
                Collection.TryRemove(deleteKey, out _);

        }
    }
}