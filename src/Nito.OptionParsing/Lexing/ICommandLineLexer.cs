using System.Collections.Generic;

namespace Nito.OptionParsing.Lexing
{
    /// <summary>
    /// A system for parsing (and quoting/escaping) command-line arguments.
    /// </summary>
    public interface ICommandLineLexer
    {
        /// <summary>
        /// Lexes the command line.
        /// </summary>
        /// <param name="commandLine">The command line to parse. May not be <c>null</c>.</param>
        /// <returns>The lexed command line.</returns>
        IEnumerable<string> Lex(string commandLine);

        /// <summary>
        /// Takes a list of arguments to pass to a program, and quotes them.
        /// </summary>
        /// <param name="arguments">The arguments to quote/escape (if necessary) and concatenate into a command line. May not be <c>null</c>.</param>
        /// <returns>The command line.</returns>
        string Escape(IEnumerable<string> arguments);
    }
}
