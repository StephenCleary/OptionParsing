namespace Nito.OptionParsing
{
    /// <summary>
    /// A parser for option values.
    /// </summary>
    public interface IOptionValueParser
    {
        /// <summary>
        /// Attempts to parse a string value into a value. Returns <c>null</c> if parsing was not possible.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        object TryParse(string value);
    }
}
