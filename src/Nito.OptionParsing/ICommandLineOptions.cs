namespace Nito.OptionParsing
{
    /// <summary>
    /// An arguments class, which uses option attributes on its properties.
    /// </summary>
    public interface ICommandLineOptions
    {
        /// <summary>
        /// Invoked when the arguments have all been applied using the specified settings.
        /// </summary>
        void Done(CommandLineOptionsSettings settings);

        /// <summary>
        /// Validates the arguments by throwing <see cref="OptionParsingException"/> errors as necessary.
        /// </summary>
        void Validate();
    }
}
