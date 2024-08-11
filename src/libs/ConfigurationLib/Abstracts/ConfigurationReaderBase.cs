using Polly;
using Polly.CircuitBreaker;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigurationLib.Abstracts
{
    /// <summary>
    /// The configuration reader base.
    /// </summary>
    public abstract class ConfigurationReaderBase : IDisposable
    {
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
        protected internal ConcurrentDictionary<string, object> Collection => _configurationCollection;

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        protected internal async Task ElapsedAsync()
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
            catch (BrokenCircuitException ex)
            {

            }
            catch (Exception ex)
            {
            }
            finally
            {
                inProgress = false;
            }
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

                if (!Extensions.IsSupportedType(typeof(T)))
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

        /// <summary>
        /// Start the timer.
        /// </summary>
        protected internal void StartTimer()
        {
            _timer?.Change(0, _refreshTimerIntervalInMs);
        }

        /// <summary>
        /// TODO: Add Summary.
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        protected internal abstract Task TriggerAsync();

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

                    _configurationCollection.Clear();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Initializes circuit breaker.
        /// </summary>
        /// <param name="exceptionsAllowedBeforeBreaking">The exceptions allowed before breaking.</param>
        /// <param name="durationOfBreak">The duration of break.</param>
        /// <returns>An <see cref="AsyncCircuitBreakerPolicy"/></returns>
        protected internal AsyncCircuitBreakerPolicy InitializeCircuitBreaker(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            var circuitBreakerPolicy = Policy
                .Handle<TimeoutException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking,
                    durationOfBreak,
                    onBreak: (exception, timespan) =>
                    {
                        onBreak = true;
                        if (Debugger.IsAttached)
                            Debug.WriteLine($"PolicyException: {exception}");
                    },
                    onReset: () =>
                    {
                        onBreak = false;
                    }
                );

            return circuitBreakerPolicy;
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        /// <param name="callbackTask">The callback task.</param>
        /// <param name="refreshTimerIntervalInMs">The refresh timer ınterval ın ms.</param>
        /// <returns>A <see cref="Timer"/></returns>
        protected internal Timer InitializeTimer(TimerCallback callbackTask, int refreshTimerIntervalInMs)
                    => new(callbackTask, null, Timeout.Infinite, _refreshTimerIntervalInMs);
    }
}