using System;
using System.Collections.Generic;

namespace Nito.OptionParsing.Lexing
{
    /// <summary>
    /// Extension methods for command-line lexers.
    /// </summary>
    public static class CommandLineLexerExtensions
    {

        /// <summary>
        /// Lexes the command line for this process. The returned command line includes the process name.
        /// </summary>
        /// <returns>The lexed command line.</returns>
        public static IEnumerable<string> Lex(this ICommandLineLexer @this) => @this.Lex(Environment.CommandLine);
    }
}
