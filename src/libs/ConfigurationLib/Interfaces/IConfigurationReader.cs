using System;

namespace ConfigurationLib.Interfaces
{
    public interface IConfigurationReader : IDisposable
    {
        T? GetValue<T>(string key);
    }
}