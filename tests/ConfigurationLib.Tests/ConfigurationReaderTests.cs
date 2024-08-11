using ConfigurationLib.Models;
using MongoDB.Driver;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationLib.Tests
{
    public class ConfigurationReaderTests
    {
        private readonly Mock<ConfigurationReader> _mockConfigReader;
        private readonly Mock<IAsyncCursor<ApplicationConfiguration>> _mockCursor;

        public ConfigurationReaderTests()
        {
        }

        [Fact]
        public async Task TriggerAsync_ShouldUpdateConfigurationCollection()
        {
            // Arrange
            var _mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            var _mockCursor = MockHelper.CreateDefaultCursor(new() {
                new()
                {
                    Name = "TestConfigKey",
                    Type = "Int",
                    Value = "123",
                    IsActive = true,
                    ApplicationName = "TestApp"
                },
                new()
                {
                    Name = "TestConfigKey",
                    Type = "Int",
                    Value = "123",
                    IsActive = false,
                    ApplicationName = "TestApp"
                }
            });

            _mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(It.IsAny<string>()))
                             .ReturnsAsync(_mockCursor.Object);

            // Act
            await _mockConfigReader.Object.TriggerAsync();

            // Assert
            _mockConfigReader.Object.Collection.ContainsKey("TestConfigKey").ShouldBeTrue();
            _mockConfigReader.Object.Collection["TestConfigKey"].ShouldBe(123);
        }

        [Fact]
        public async Task TriggerAsync_ShouldRemoveDeletedConfigurations()
        {
            // Arrange
            var _mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            var _mockCursor = MockHelper.CreateDefaultCursor(new());

            _mockConfigReader.Object.Collection.TryAdd("OldConfigkey", 456);

            _mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(It.IsAny<string>()))
                             .ReturnsAsync(_mockCursor.Object);

            // Act
            await _mockConfigReader.Object.TriggerAsync();

            // Assert
            _mockConfigReader.Object.Collection.ContainsKey("OldConfigkey").ShouldBeFalse();
        }
    }
}