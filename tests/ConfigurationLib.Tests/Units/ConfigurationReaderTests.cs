using ConfigurationLib.Models;
using ConfigurationLib.Tests.Helpers;
using MongoDB.Driver;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationLib.Tests.Units
{
    public class ConfigurationReaderTests
    {
        public ConfigurationReaderTests()
        {
        }

        [Fact]
        public async Task TriggerAsync_ShouldUpdateConfigurationCollection()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            var mockCursor = MockHelper.CreateDefaultCursor(new() {
                new()
                {
                    Name = "TestConfigKey",
                    Type = "Int",
                    Value = "123",
                    IsActive = true,
                    ApplicationName = "TestApp"
                }
            });

            mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(It.IsAny<string>()))
                             .ReturnsAsync(mockCursor.Object);

            // Act
            await mockConfigReader.Object.TriggerAsync();

            // Assert
            mockConfigReader.Object.Collection.ContainsKey("TestConfigKey").ShouldBeTrue();
            mockConfigReader.Object.Collection["TestConfigKey"].ShouldBe(123);
        }

        [Fact]
        public async Task TriggerAsync_ShouldNotReturnInActiveConfigurationCollection()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            var mockCursor = MockHelper.CreateDefaultCursor(new() { });

            mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(It.IsAny<string>()))
                             .ReturnsAsync(mockCursor.Object);

            // Act
            await mockConfigReader.Object.TriggerAsync();

            // Assert
            mockConfigReader.Object.Collection.Count.ShouldBe(0);
        }

        [Fact]
        public async Task TriggerAsync_ShouldRemoveDeletedConfigurations()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            var mockCursor = MockHelper.CreateDefaultCursor(new());

            mockConfigReader.Object.Collection.TryAdd("DeletedConfigkey", 456);

            mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(It.IsAny<string>()))
                             .ReturnsAsync(mockCursor.Object);

            // Act
            await mockConfigReader.Object.TriggerAsync();

            // Assert
            mockConfigReader.Object.Collection.ContainsKey("DeletedConfigkey").ShouldBeFalse();
        }
    }
}