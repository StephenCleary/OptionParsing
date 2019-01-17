namespace Nito.OptionParsing.LowLevel
{
    /// <summary>
    /// An option that has been parsed.
    /// </summary>
    public sealed class ParsedOption
    {
        /// <summary>
        /// The option definition that matched this option. May be <c>null</c> if this is a positional argument.
        /// </summary>
        public OptionDefinition Definition { get; set; }

        /// <summary>
        /// The argument passed to this option. May be <c>null</c> if there is no argument.
        /// </summary>
        public string Argument { get; set; }
    }
}
