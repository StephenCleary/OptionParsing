namespace Nito.OptionParsing
{
    /// <summary>
    /// An unknown option was encountered during command-line option parsing.
    /// </summary>
    public class UnknownOptionException: OptionParsingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionParsingException.UnknownOptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownOptionException(string message)
            : base(message)
        {
        }
    }
}
