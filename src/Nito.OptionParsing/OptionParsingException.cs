using System;

namespace Nito.OptionParsing
{
    /// <summary>
    /// A usage error was encountered during command-line option parsing.
    /// </summary>
    public class OptionParsingException: Exception
    {
        /// <param name="message">The message.</param>
        public OptionParsingException(string message)
            : base(message)
        {
        }

        /// <param name="message">The message.</param>
        /// <param name="innerException">The exception that is the root cause of the error.</param>
        public OptionParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
