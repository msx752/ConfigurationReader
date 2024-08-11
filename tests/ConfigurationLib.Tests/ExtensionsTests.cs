using System;
using Xunit;

namespace ConfigurationLib.Tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(double))]
        [InlineData(typeof(bool))]
        public void IsSupportedType_ShouldReturnTrueForSupportedTypes(Type type)
        {
            // Act
            var result = Extensions.IsSupportedType(type);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(long))]
        [InlineData(typeof(float))]
        [InlineData(typeof(object))]
        public void IsSupportedType_ShouldReturnFalseForUnsupportedTypes(Type type)
        {
            // Act
            var result = Extensions.IsSupportedType(type);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("int")]
        [InlineData("string")]
        [InlineData("boolean")]
        [InlineData("DOUBLE")]
        public void GetSupportedTypeByStringType_ShouldReturnCorrectType(string stringType)
        {
            // Act
            var result = Extensions.GetSupportedTypeByStringType(stringType);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("DateTime")]
        [InlineData("Long")]
        [InlineData("Int64")]
        [InlineData("Int32")]
        [InlineData("")]
        [InlineData("-")]
        public void GetSupportedTypeByStringType_ShouldReturnNullForUnsupportedStringType(string stringType)
        {
            // Act
            var result = Extensions.GetSupportedTypeByStringType(stringType);

            // Assert
            Assert.Null(result);
        }
    }
}