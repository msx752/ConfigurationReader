using Shouldly;
using System;
using Xunit;

namespace ConfigurationLib.Tests.Units
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
            result.ShouldBeTrue();
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
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("int")]
        [InlineData("strIng")]
        [InlineData("booleaN")]
        [InlineData("DOUBLE")]
        public void GetSupportedTypeByStringType_ShouldReturnCorrectType(string stringType)
        {
            // Act
            var result = Extensions.GetSupportedTypeByStringType(stringType);

            // Assert
            result.ShouldNotBeNull();
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
            result.ShouldBeNull();
        }
    }
}