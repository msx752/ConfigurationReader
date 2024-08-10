using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigLib.Abstracts
{
    /// <summary>
    /// The configuration reader base.
    /// </summary>
    public abstract class ConfigurationReaderBase : IDisposable
    {
        #region fields

        /// <summary>
        /// Supported types.
        /// </summary>
        private static readonly Dictionary<Type, string> _supportedTypes = new(new KeyValuePair<Type, string>[4] {
            new(typeof(int), "Int"),
            new(typeof(string), "String"),
            new(typeof(double), "Double"),
            new(typeof(bool), "Boolean"),
        });

        /// <summary>
        /// The circuit breaker policy.
        /// </summary>
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        /// <summary>
        /// The configuration collection.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _configurationCollection = new(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Lock timer.
        /// </summary>
        private readonly object _lock_timer = new();

        /// <summary>
        /// The refresh timer ınterval ın ms.
        /// </summary>
        private readonly int _refreshTimerIntervalInMs;

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// The disposed value.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        /// The in progress.
        /// </summary>
        private bool inProgress = false;

        /// <summary>
        /// On break.
        /// </summary>
        private bool onBreak = false;

        #endregion fields

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReaderBase"/> class.
        /// </summary>
        /// <param name="refreshTimerIntervalInMs">The refresh timer ınterval ın ms.</param>
        protected ConfigurationReaderBase(int refreshTimerIntervalInMs)
        {
            if (refreshTimerIntervalInMs < 100)
                throw new ArgumentException($"'{nameof(refreshTimerIntervalInMs)}' cannot be less than 100ms.", nameof(refreshTimerIntervalInMs));

            _refreshTimerIntervalInMs = refreshTimerIntervalInMs;

            _circuitBreakerPolicy = InitializeCircuitBreaker(3, TimeSpan.FromMinutes(1));

            _timer = InitializeTimer(async _ => await ElapsedAsync(), _refreshTimerIntervalInMs);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>A dictionary with a key of type string and a value of type object.</value>
        protected ConcurrentDictionary<string, object> Collection => _configurationCollection;

        #region public mehods

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the value.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A <see cref="T? "/></returns>
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

        /// <summary>
        /// Get supported type by string type.
        /// </summary>
        /// <param name="stringType">The string type.</param>
        /// <returns>A <see cref="Type? "/></returns>
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

        /// <summary>
        /// Checks if is supported type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="bool"/></returns>
        internal static bool IsSupportedType(Type type)
        {
            if (type == null)
                return false;

            return _supportedTypes.ContainsKey(type);
        }

        #endregion static methods

        #region protected methods

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <param name="disposing">If true, disposing.</param>
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

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        /// <param name="callbackTask">The callback task.</param>
        /// <param name="refreshTimerIntervalInMs">The refresh timer ınterval ın ms.</param>
        /// <returns>A <see cref="Timer"/></returns>
        protected Timer InitializeTimer(TimerCallback callbackTask, int refreshTimerIntervalInMs)
                    => new(callbackTask, null, Timeout.Infinite, _refreshTimerIntervalInMs);

        /// <summary>
        /// Initializes circuit breaker.
        /// </summary>
        /// <param name="exceptionsAllowedBeforeBreaking">The exceptions allowed before breaking.</param>
        /// <param name="durationOfBreak">The duration of break.</param>
        /// <returns>An <see cref="AsyncCircuitBreakerPolicy"/></returns>
        protected AsyncCircuitBreakerPolicy InitializeCircuitBreaker(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            var circuitBreakerPolicy = Policy
                .Handle<TimeoutException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking,
                    durationOfBreak,
                    onBreak: (exception, timespan) =>
                    {
                        onBreak = true;
                    },
                    onReset: () =>
                    {
                        onBreak = false;
                    }
                );

            return circuitBreakerPolicy;
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        protected void StartTimer()
        {
            _timer?.Change(0, _refreshTimerIntervalInMs);
        }

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        protected abstract Task TriggerAsync();

        #endregion protected methods

        #region private methods

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        private async Task ElapsedAsync()
        {
            if (onBreak || inProgress)
            {
                lock (_lock_timer)
                {
                    if (onBreak || inProgress)
                    {
                        _timer?.Change(_refreshTimerIntervalInMs, _refreshTimerIntervalInMs);
                        return;
                    }
                }
            }

            inProgress = true;

            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () => await TriggerAsync());
            }
            catch (Exception ex)
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