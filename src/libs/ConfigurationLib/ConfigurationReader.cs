using System;
using System.Threading.Tasks;
using ConfigLib.Abstracts;
using ConfigLib.Interfaces;

namespace ConfigLib
{
    public sealed class ConfigurationReader : MongoDbConfigurationReader, IConfigurationReader
    {
        private readonly string _applicationName;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
            : base(connectionString, refreshTimerIntervalInMs)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or empty.", nameof(applicationName));

            _applicationName = applicationName;

            StartTimer();
        }

        protected override async Task TickAsync()
        {
            try
            {
                //TODO: fetch mongodb on here and update local collection
                //we need to remove InActive status after fetch
                //use _InternalSetObject(key, val); after fecthing
                /*
                    if (!IsSupportedType(typeof(T)))
                        throw new ArgumentNullException(typeof(T).Name, "specified type not supported.");

                    //TODO: update mongodb here

                    _ = Collection.AddOrUpdate(key, val, (k, v) => val);
                 */
            }
            catch (Exception e)
            {
            }
        }
    }
}