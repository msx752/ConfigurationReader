using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigLib.Abstracts
{
    public abstract class ConfigurationReaderBase : IDisposable
    {
        #region fields

        private static readonly Dictionary<Type, string> _supportedTypes = new(new KeyValuePair<Type, string>[4] {
            new(typeof(int), "Int"),
            new(typeof(string), "String"),
            new(typeof(double), "Double"),
            new(typeof(bool), "Boolean"),
        });

        private readonly ConcurrentDictionary<string, object> _configurationCollection = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly object _lock_timer = new();
        private readonly int _refreshTimerIntervalInMs;
        private readonly Timer _timer;
        private bool _disposedValue;
        private bool inProgress = false;

        #endregion fields

        protected ConfigurationReaderBase(int refreshTimerIntervalInMs)
        {
            if (refreshTimerIntervalInMs < 100)
                throw new ArgumentException($"'{nameof(refreshTimerIntervalInMs)}' cannot be less than 100ms.", nameof(refreshTimerIntervalInMs));

            _refreshTimerIntervalInMs = refreshTimerIntervalInMs;

            _timer = InitializeTimer(async _ => await ElapsedAsync(), _refreshTimerIntervalInMs);
        }

        protected ConcurrentDictionary<string, object> Collection => _configurationCollection;

        #region public mehods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public T? GetValue<T>(string key) where T : struct
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key), "parameter value cannot be null or empty.");

                if (!IsSupportedType(typeof(T)))
                    throw new NotSupportedException($"{typeof(T).Name} type not supported.");

                _ = Collection.TryGetValue(key, out var val);

                if (val == null)
                    return null;
                else if (val is T)
                    return (T)val;
                else
                    throw new ArgumentException($"'{val.GetType().Name}' type doesn't match by requested '{typeof(T).Name}' type.");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion public mehods

        #region static methods

        internal static Type? GetSupportedTypeByStringType(string stringType)
        {
            if (string.IsNullOrWhiteSpace(stringType))
                return null;

            foreach (var item in _supportedTypes)
            {
                if (string.Equals(stringType, item.Value, StringComparison.InvariantCultureIgnoreCase))
                    return item.Key;
            }

            return null;
        }

        internal static bool IsSupportedType(Type type)
        {
            if (type == null)
                return false;

            return _supportedTypes.ContainsKey(type);
        }

        #endregion static methods

        #region protected methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try { _timer?.Dispose(); } catch { }
                    _supportedTypes.Clear();

                    _configurationCollection.Clear();
                }

                _disposedValue = true;
            }
        }

        protected Timer InitializeTimer(TimerCallback callbackTask, int refreshTimerIntervalInMs)
                    => new(callbackTask, null, Timeout.Infinite, _refreshTimerIntervalInMs);

        protected void StartTimer()
        {
            _timer?.Change(0, _refreshTimerIntervalInMs);
        }

        protected abstract Task TriggerAsync();

        #endregion protected methods

        #region private methods

        private async Task ElapsedAsync()
        {
            if (inProgress)
            {
                lock (_lock_timer)
                {
                    if (inProgress)
                    {
                        _timer?.Change(_refreshTimerIntervalInMs, _refreshTimerIntervalInMs);
                        return;
                    }
                }
            }

            inProgress = true;

            try
            {
                await TriggerAsync();
            }
            catch
            {
            }
            finally
            {
                inProgress = false;
            }
        }

        #endregion private methods
    }
}