using System;
using System.Collections.Generic;

namespace ConfigurationLib
{
    public static class Extensions
    {
        /// <summary>
        /// Supported types.
        /// </summary>
        private static readonly Dictionary<Type, string> _supportedTypes = new(new KeyValuePair<Type, string>[4] {
            new(typeof(int), "Int"),
            new(typeof(string), "String"),
            new(typeof(double), "Double"),
            new(typeof(bool), "Boolean"),
        });

        /// <summary>
        /// Checks if is supported type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="bool"/></returns>
        public static bool IsSupportedType(Type type)
        {
            if (type == null)
                return false;

            return _supportedTypes.ContainsKey(type);
        }

        /// <summary>
        /// Get supported type by string type.
        /// </summary>
        /// <param name="stringType">The string type.</param>
        /// <returns>A <see cref="Type? "/></returns>
        public static Type? GetSupportedTypeByStringType(string stringType)
        {
            if (string.IsNullOrWhiteSpace(stringType))
                return null;

            foreach (var item in _supportedTypes)
            {
                if (string.Equals(stringType, item.Value, StringComparison.InvariantCultureIgnoreCase))
                    return item.Key;
            }

            return null;
        }
    }
}