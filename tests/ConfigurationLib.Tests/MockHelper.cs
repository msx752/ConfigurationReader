using ConfigurationLib.Abstracts;
using ConfigurationLib.Models;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;

namespace ConfigurationLib.Tests
{
    public static class MockHelper
    {
        public static Mock<ConfigurationReader> CreateDefaultConfigurationReader()
        {
            return new Mock<ConfigurationReader>("TestApp", "mongodb://localhost:27017", 1000) { CallBase = true };
        }

        public static Mock<MongoDbConfigurationReader> CreateDefaultMongoDbConfigurationReader()
        {
            return new Mock<MongoDbConfigurationReader>("mongodb://localhost:27017", 1000) { CallBase = true };
        }

        public static Mock<ConfigurationReaderBase> CreateDefaultConfigurationReaderBase()
        {
            return new Mock<ConfigurationReaderBase>(1000) { CallBase = true };
        }

        public static Mock<MockAsyncCursor<ApplicationConfiguration>> CreateDefaultCursor(List<ApplicationConfiguration> configurations)
        {
            return new Mock<MockAsyncCursor<ApplicationConfiguration>>(configurations) { CallBase = true };
        }

        public static Mock<IMongoDatabase> CreateDefaultIMongoDatabase()
        {
            return new Mock<IMongoDatabase>();
        }

        public static Mock<IMongoCollection<ApplicationConfiguration>> CreateDefaultIMongoCollection()
        {
            return new Mock<IMongoCollection<ApplicationConfiguration>>();
        }
    }
}