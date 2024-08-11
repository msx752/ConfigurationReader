using ConfigurationLib.Abstracts;
using Moq;
using Polly.CircuitBreaker;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationLib.Tests
{
    public class ConfigurationReaderBaseTests
    {
        private readonly Mock<ConfigurationReaderBase> _mockConfigReader;

        public ConfigurationReaderBaseTests()
        {
            _mockConfigReader = MockHelper.CreateDefaultConfigurationReaderBase();
        }

        [Fact]
        public void InitializeCircuitBreaker_ShouldSetPolicyCorrectly()
        {
            // Arrange
            int exceptionsAllowedBeforeBreaking = 3;
            TimeSpan durationOfBreak = TimeSpan.FromMinutes(1);

            // Act
            var circuitBreakerPolicy = _mockConfigReader.Object.InitializeCircuitBreaker(exceptionsAllowedBeforeBreaking, durationOfBreak);

            // Assert
            Assert.NotNull(circuitBreakerPolicy);
            Assert.IsType<AsyncCircuitBreakerPolicy>(circuitBreakerPolicy);
        }

        [Fact]
        public void GetValue_WhenKeyIsNullOrEmpty_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mockConfigReader.Object.GetValue<int>(null));
            Assert.Throws<ArgumentNullException>(() => _mockConfigReader.Object.GetValue<int>(""));
        }

        [Fact]
        public void GetValue_WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
        {
            // Arrange
            string key = "TestKey";
            _mockConfigReader.Object.Collection.TryAdd(key, "someValue");

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => _mockConfigReader.Object.GetValue<DateTime>(key));
        }

        [Fact]
        public void GetValue_WhenTypeDoesNotMatch_ShouldThrowArgumentException()
        {
            // Arrange
            string key = "TestKey";
            _mockConfigReader.Object.Collection.TryAdd(key, "someValue");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _mockConfigReader.Object.GetValue<int>(key));
        }

        [Fact]
        public void GetValue_WhenKeyExistsAndTypeMatches_ShouldReturnCorrectValue()
        {
            // Arrange
            string key = "TestKey";
            int expectedValue = 123;
            _mockConfigReader.Object.Collection.TryAdd(key, expectedValue);

            // Act
            int? actualValue = _mockConfigReader.Object.GetValue<int>(key);

            // Assert
            actualValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task ElapsedAsync_ShouldNotProceedIfOnBreak()
        {
            // Arrange
            var configReader = _mockConfigReader.Object;
            _mockConfigReader.Setup(m => m.TriggerAsync()).Verifiable();
            var privateOnBreakField = typeof(ConfigurationReaderBase).GetField("onBreak", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            privateOnBreakField.SetValue(configReader, true);

            // Act
            await configReader.ElapsedAsync();

            // Assert
            _mockConfigReader.Verify(m => m.TriggerAsync(), Times.Never);
        }

        [Fact]
        public async Task ElapsedAsync_ShouldExecuteTriggerAsyncWhenNotOnBreak()
        {
            // Arrange
            _mockConfigReader.Setup(m => m.TriggerAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _mockConfigReader.Object.ElapsedAsync();

            // Assert
            _mockConfigReader.Verify(m => m.TriggerAsync(), Times.Once);
        }

        [Fact]
        public void StartTimer_ShouldStartTheTimer()
        {
            // Act
            _mockConfigReader.Object.StartTimer();

            // Assert
            Assert.True(true); // Asserting that no exceptions occur
        }

        [Fact]
        public void Dispose_ShouldDisposeResources()
        {
            // Act
            _mockConfigReader.Object.Dispose();

            // Assert
            Assert.True(true); // Asserting that no exceptions occur during disposal
        }
    }
}