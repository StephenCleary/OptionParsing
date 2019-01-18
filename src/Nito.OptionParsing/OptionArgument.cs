using System;
using System.Collections.Generic;
using System.Text;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Whether an option may take an argument.
    /// </summary>
    public enum OptionArgument
    {
        /// <summary>
        /// The option never takes an argument.
        /// </summary>
        None,

        /// <summary>
        /// The option requires an argument.
        /// </summary>
        Required,

        /// <summary>
        /// The option takes an argument if present. The argument may not start with '-' or '/'.
        /// </summary>
        Optional,
    }
}
