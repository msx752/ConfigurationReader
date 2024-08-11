using ConfigurationLib.Models;
using ConfigurationLib.Tests.Helpers;
using MongoDB.Driver;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConfigurationLib.Tests.Units
{
    public class MongoDbConfigurationReaderTests
    {
        [Fact]
        public void InitializeMongoClient_ShouldReturnMongoClient()
        {
            // Arrange
            var mockConfigReader = MockHelper.CreateDefaultMongoDbConfigurationReader();

            // Act
            var client = mockConfigReader.Object.InitializeMongoClient("mongodb://localhost:27017");

            // Assert
            client.ShouldNotBeNull();
            Assert.IsType<MongoClient>(client);
        }

        [Fact]
        public async Task ListConfigurationByApplicationNameAsync_ShouldReturnCursor()
        {
            // Arrange
            var applicationName = "TestApp";

            var mockCursor = MockHelper.CreateDefaultCursor(new List<ApplicationConfiguration>
            {
                new()
                {
                    Name = "TestConfig",
                    Type = "Int",
                    Value = "123",
                    IsActive = true,
                    ApplicationName = "TestApp"
                }
            });

            var mockConfigReader = MockHelper.CreateDefaultMongoDbConfigurationReader();

            mockConfigReader.Setup(m => m.ListConfigurationByApplicationNameAsync(applicationName))
                            .ReturnsAsync(mockCursor.Object);

            // Act
            var cursor = await mockConfigReader.Object.ListConfigurationByApplicationNameAsync(applicationName);

            // Assert
            cursor.ShouldNotBeNull();
            mockCursor.Object.ShouldBeEquivalentTo(cursor);
        }
    }
}