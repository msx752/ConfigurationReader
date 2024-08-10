using System;

namespace ConfigLib.Interfaces
{
    public interface IConfigurationReader : IDisposable
    {
        T? GetValue<T>(string key) where T : struct;
    }
}