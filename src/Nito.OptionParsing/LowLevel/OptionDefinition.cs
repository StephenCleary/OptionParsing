using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.OptionParsing.LowLevel
{
    /// <summary>
    /// The definition of an option that may be passed to a console program.
    /// </summary>
    public sealed class OptionDefinition
    {
        /// <summary>
        /// The long name of the option, if any. The long name may not contain ':' or '=' characters. May be <c>null</c>.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// The short name of the option, if any. The short name may not be ':' or '='. May be <c>null</c>.
        /// </summary>
        public char? ShortName { get; set; }

        /// <summary>
        /// The short name of the option as a string. May return <c>null</c>.
        /// </summary>
        public string ShortNameAsString => ShortName?.ToString();

        /// <summary>
        /// Returns the long name, if any; otherwise, returns the short name, if any. May return <c>null</c>.
        /// </summary>
        public string Name => LongName ?? ShortNameAsString;

        /// <summary>
        /// Whether this option takes an argument.
        /// </summary>
        public OptionArgument Argument { get; set; }

        /// <summary>
        /// Ensures that the option definition is valid. Throws <see cref="InvalidOperationException"/> if this option definition is invalid.
        /// </summary>
        public void Validate()
        {
            var name = Name;
            if (name == null)
                throw new InvalidOperationException("Option must have either a long name or a short name.");
            if (name.IndexOfAny(Constants.ArgumentDelimiters) != -1)
                throw new InvalidOperationException($"Option {name} may not have ':' or '=' in its name.");
        }
    }
}
