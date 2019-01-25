namespace Nito.OptionParsing
{
    /// <summary>
    /// An arguments class, which uses option attributes on its properties.
    /// </summary>
    public interface ICommandLineOptions
    {
        /// <summary>
        /// Invoked when the arguments have all been applied.
        /// </summary>
        void Done();

        /// <summary>
        /// Validates the arguments by throwing <see cref="OptionParsingException"/> errors as necessary.
        /// </summary>
        void Validate();
    }
}
