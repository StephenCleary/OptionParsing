using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nito.OptionParsing
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a friendly name for a type (using angle brackets with generic parameters instead of backticks). Nullable types are treated as their underlying type.
        /// </summary>
        /// <param name="type">The type whose name is returned.</param>
        /// <returns>The friendly name for the type.</returns>
        public static string FriendlyTypeName(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return FriendlyTypeName(underlyingType);

            if (type.IsGenericType)
            {
                var backtickIndex = type.Name.IndexOf('`');
                return type.Name.Substring(0, backtickIndex) + "<" + string.Join(", ", type.GetGenericArguments().Select(FriendlyTypeName)) + ">";
            }

            return type.Name;
        }

        /// <summary>
        /// Sets a property on a command line options object, translating exceptions.
        /// </summary>
        /// <param name="commandLineOptions">The command line options object.</param>
        /// <param name="property">The property to set.</param>
        /// <param name="value">The value for the property.</param>
        public static void SetOptionProperty(this PropertyInfo property, object commandLineOptions, object value)
        {
            try
            {
                property.SetValue(commandLineOptions, value, null);
            }
            catch (TargetInvocationException ex)
            {
                throw new OptionArgumentException(ex.InnerException?.Message ?? ex.Message, ex.InnerException);
            }
        }
    }
}
