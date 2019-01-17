using System;

namespace Nito.OptionParsing
{
    /// <summary>
    /// An option argument error was encountered during command-line option parsing. This includes argument parsing errors.
    /// </summary>
    public class OptionArgumentException: OptionParsingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionParsingException.UnknownOptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OptionArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionArgumentException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The exception that is the root cause of the error.</param>
        public OptionArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
