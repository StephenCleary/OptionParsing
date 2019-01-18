using System;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Specifies that the command-line sets this property to the remaining positional arguments. The property type must be a collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositionalArgumentsAttribute: Attribute
    {
    }
}
