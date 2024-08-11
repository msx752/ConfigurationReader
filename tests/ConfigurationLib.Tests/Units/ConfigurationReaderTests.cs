using ConfigurationLib.Models;
using ConfigurationLib.Tests.Helpers;
using MongoDB.Driver;
using Moq;
using Polly.CircuitBreaker;
using Shouldly;
using System;
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

        [Fact]
        public async Task GetValue_ShouldReturnInt()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            int expectedValue = 456;

            // act
            mockConfigReader.Object.Collection.TryAdd("testing_number", expectedValue);
            var actualValue = mockConfigReader.Object.GetValue<int>("testing_number");

            // Assert
            actualValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task GetValue_ShouldReturnString()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            string expectedValue = "hello";

            // act
            mockConfigReader.Object.Collection.TryAdd("testing_string", expectedValue);
            var actualValue = mockConfigReader.Object.GetValue<string>("testing_string");

            // Assert
            actualValue.ShouldBeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task GetValue_Object_ShouldThrowNotSupportedException()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            object expectedValue = new { property = "123" };

            // act
            mockConfigReader.Object.Collection.TryAdd("testing_object", expectedValue);

            Assert.Throws<NotSupportedException>(() => mockConfigReader.Object.GetValue<object>("testing_object"));
        }

        [Fact]
        public async Task GetValue_Decimal_ShouldThrowNotSupportedException()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultConfigurationReader();
            decimal expectedValue = 123.4M;
            // act
            mockConfigReader.Object.Collection.TryAdd("testing_decimal", expectedValue);

            Assert.Throws<NotSupportedException>(() => mockConfigReader.Object.GetValue<decimal>("testing_decimal"));
        }
    }
}