namespace Nito.OptionParsing
{
    /// <summary>
    /// An invalid option or argument was encountered during command-line option parsing.
    /// </summary>
    public class InvalidParameterException: OptionParsingException
    {
        /// <param name="message">The message.</param>
        public InvalidParameterException(string message)
            : base(message)
        {
        }
    }
}
