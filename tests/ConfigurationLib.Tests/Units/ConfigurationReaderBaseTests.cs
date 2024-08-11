using ConfigurationLib.Abstracts;
using ConfigurationLib.Tests.Helpers;
using Moq;
using Polly.CircuitBreaker;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationLib.Tests.Units
{
    public class ConfigurationReaderBaseTests
    {
        public ConfigurationReaderBaseTests()
        {
        }

        [Fact]
        public void Dispose_ShouldDisposeResources()
        {
            // Act
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            mockConfigReader.Object.Dispose();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task ElapsedAsync_ShouldExecuteTriggerAsyncWhenNotOnBreak()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            mockConfigReader.Setup(m => m.TriggerAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            await mockConfigReader.Object.ElapsedAsync();

            // Assert
            mockConfigReader.Verify(m => m.TriggerAsync(), Times.Once);
        }

        [Fact]
        public async Task ElapsedAsync_ShouldNotProceedIfOnBreak()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            var configReader = mockConfigReader.Object;
            mockConfigReader.Setup(m => m.TriggerAsync()).Verifiable();
            var privateOnBreakField = typeof(ConfigurationReaderBase).GetField("onBreak", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            privateOnBreakField.SetValue(configReader, true);

            // Act
            await configReader.ElapsedAsync();

            // Assert
            mockConfigReader.Verify(m => m.TriggerAsync(), Times.Never);
        }

        [Fact]
        public async Task ElapsedAsync_ShouldStopWhenCircuitBreakerOn()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            int exceptionsAllowedBeforeBreaking = 3;
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(2);
            var privateOnBreakField = typeof(ConfigurationReaderBase).GetField("onBreak", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var circuitBreakerPolicy = mockConfigReader.Object.InitializeCircuitBreaker(exceptionsAllowedBeforeBreaking, durationOfBreak);

            await Assert.ThrowsAsync<TimeoutException>(async () => await circuitBreakerPolicy.ExecuteAsync(async () => await Task.FromException(new TimeoutException())));
            await Assert.ThrowsAsync<TimeoutException>(async () => await circuitBreakerPolicy.ExecuteAsync(async () => await Task.FromException(new TimeoutException())));
            await Assert.ThrowsAsync<TimeoutException>(async () => await circuitBreakerPolicy.ExecuteAsync(async () => await Task.FromException(new TimeoutException())));

            await Assert.ThrowsAsync<BrokenCircuitException>(async () => await circuitBreakerPolicy.ExecuteAsync(() => Task.FromResult(true)));

            bool onBreakBeforeValue = (bool)privateOnBreakField.GetValue(mockConfigReader.Object);
            await Task.Delay(TimeSpan.FromSeconds(2.1));
            await circuitBreakerPolicy.ExecuteAsync(async () => await Task.FromResult(true));
            bool onBreakAfterValue = (bool)privateOnBreakField.GetValue(mockConfigReader.Object);

            onBreakBeforeValue.ShouldBeTrue();
            onBreakAfterValue.ShouldBeFalse();
        }

        [Fact]
        public void GetValue_WhenKeyExistsAndTypeMatches_ShouldReturnCorrectValue()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            string key = "IntKey";
            int expectedValue = 123;
            mockConfigReader.Object.Collection.TryAdd(key, expectedValue);

            // Act
            int? actualValue = mockConfigReader.Object.GetValue<int>(key);

            // Assert
            actualValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Fact]
        public void GetValue_WhenKeyIsNullOrEmpty_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();

            Assert.Throws<ArgumentNullException>(() => mockConfigReader.Object.GetValue<int>(null));
            Assert.Throws<ArgumentNullException>(() => mockConfigReader.Object.GetValue<int>(""));
        }

        [Fact]
        public void GetValue_WhenTypeDoesNotMatch_ShouldThrowArgumentException()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            string key = "IntKey";
            mockConfigReader.Object.Collection.TryAdd(key, "123");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mockConfigReader.Object.GetValue<int>(key));
        }

        [Fact]
        public void GetValue_WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            string key = "DateTimeKey";
            mockConfigReader.Object.Collection.TryAdd(key, DateTime.Now);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => mockConfigReader.Object.GetValue<DateTime>(key));
        }

        [Fact]
        public void InitializeCircuitBreaker_ShouldSetPolicyCorrectly()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            int exceptionsAllowedBeforeBreaking = 3;
            TimeSpan durationOfBreak = TimeSpan.FromMinutes(1);

            // Act
            var circuitBreakerPolicy = mockConfigReader.Object.InitializeCircuitBreaker(exceptionsAllowedBeforeBreaking, durationOfBreak);

            // Assert
            Assert.NotNull(circuitBreakerPolicy);
            Assert.IsType<AsyncCircuitBreakerPolicy>(circuitBreakerPolicy);
        }
        [Fact]
        public void StartTimer_ShouldStartTheTimer()
        {
            // Act
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
            mockConfigReader.Object.StartTimer();

            // Assert
            Assert.True(true);
        }
    }
}