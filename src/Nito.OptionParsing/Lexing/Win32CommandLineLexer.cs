using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Nito.OptionParsing.Lexing
{
    /// <summary>
    /// Provides the default Win32 Console command-line lexing. Use this type if you need backwards-compatibility for your command-line parsing.
    /// </summary>
    public sealed class Win32CommandLineLexer: ICommandLineLexer
    {
        public static Win32CommandLineLexer Instance { get; } = new Win32CommandLineLexer();

        private enum LexerState
        {
            /// <summary>
            /// The default state; no data exists in the argument character buffer.
            /// </summary>
            Default,

            /// <summary>
            /// An argument has been started.
            /// </summary>
            Argument,

            /// <summary>
            /// A quote character has been seen, and we are now parsing quoted data.
            /// </summary>
            Quoted,

            /// <summary>
            /// The quote has just been closed, but the argument is still being parsed.
            /// </summary>
            EndQuotedArgument
        }

        /// <summary>
        /// A string buffer combined with a backslash count.
        /// </summary>
        private sealed class Buffer
        {
            private string _result;
            private int _backslashes;

            public Buffer()
            {
                _result = string.Empty;
                _backslashes = 0;
            }

            /// <summary>
            /// Adds any outstanding backslashes to the result, and resets the backslash count.
            /// </summary>
            private void Normalize()
            {
                _result += new string('\\', _backslashes);
                _backslashes = 0;
            }

            /// <summary>
            /// Appends a character to the buffer. If the character is a double-quote, it is treated like an ordinary character. The character may not be a backslash.
            /// </summary>
            /// <param name="ch">The character. May not be a backslash.</param>
            public void AppendNormalChar(char ch)
            {
                Debug.Assert(ch != '\\');
                Normalize();
                _result += ch;
            }

            /// <summary>
            /// Appends a backslash to the buffer.
            /// </summary>
            private void AppendBackslash()
            {
                ++_backslashes;
            }

            /// <summary>
            /// Processes a double-quote, which may add it to the buffer. Returns <c>true</c> if there were an even number of backslashes.
            /// </summary>
            /// <returns><c>true</c> if there were an even number of backslashes.</returns>
            public bool AppendQuote()
            {
                _result += new string('\\', _backslashes / 2);
                var ret = ((_backslashes % 2) == 0);
                _backslashes = 0;
                if (!ret)
                {
                    // An odd number of backslashes means the double-quote is escaped.
                    _result += '"';
                }

                return ret;
            }

            /// <summary>
            /// Appends a regular character or backslash to the buffer.
            /// </summary>
            /// <param name="ch">The character to append. May not be a double quote.</param>
            public void AppendChar(char ch)
            {
                Debug.Assert(ch != '"');
                if (ch == '\\')
                    AppendBackslash();
                else
                    AppendNormalChar(ch);
            }

            /// <summary>
            /// Consumes the buffer so far, resetting the buffer and backslash count.
            /// </summary>
            /// <returns>The buffer.</returns>
            public string Consume()
            {
                Normalize();
                var ret = _result;
                _result = string.Empty;
                return ret;
            }
        }

        /// <summary>
        /// Lexes the command line, using the same rules as <see cref="Environment.GetCommandLineArgs"/>.
        /// </summary>
        /// <param name="commandLine">The command line to parse.</param>
        /// <returns>The lexed command line.</returns>
        public IEnumerable<string> Lex(string commandLine)
        {
            if (commandLine == null)
                throw new ArgumentNullException(nameof(commandLine));

            // The MSDN information for <see cref="Environment.GetCommandLineArgs"/> is incomplete.
            // This blog post fills in the gaps: http://hardtoc.com/2010/09/24/quoting-command-line-arguments.html (webcite: http://www.webcitation.org/62LHTVelJ )

            var state = LexerState.Default;

            var buffer = new Buffer();
            foreach (var ch in commandLine)
            {
                switch (state)
                {
                    case LexerState.Default:
                        if (ch == '"')
                        {
                            // Enter the quoted state, without placing anything in the buffer.
                            state = LexerState.Quoted;
                            break;
                        }

                        // Whitespace is ignored.
                        if (ch == ' ' || ch == '\t')
                        {
                            break;
                        }

                        buffer.AppendChar(ch);
                        state = LexerState.Argument;
                        break;

                    case LexerState.Argument:
                        // We have an argument started, though it may be just an empty string for now.
                        if (ch == '"')
                        {
                            // Handle the special rules for any backslashes preceding a double-quote.
                            if (buffer.AppendQuote())
                            {
                                // An even number of backslashes means that this is a normal double-quote.
                                state = LexerState.Quoted;
                            }

                            break;
                        }

                        if (ch == ' ' || ch == '\t')
                        {
                            // Whitespace ends this argument, so publish it and restart in the default state.
                            yield return buffer.Consume();
                            state = LexerState.Default;
                            break;
                        }

                        // Count backslashes; put other characters directly into the buffer.
                        buffer.AppendChar(ch);
                        break;

                    case LexerState.Quoted:
                        // We are within quotes, but may already have characters in the argument buffer.
                        if (ch == '"')
                        {
                            // Handle the special rules for any backslashes preceding a double-quote.
                            if (buffer.AppendQuote())
                            {
                                // An even number of backslashes means that this is a normal double-quote.
                                state = LexerState.EndQuotedArgument;
                            }

                            break;
                        }

                        // Any non-quote character (including whitespace) is appended to the argument buffer.
                        buffer.AppendChar(ch);
                        break;

                    case LexerState.EndQuotedArgument:
                        // This is a special state that is treated like Argument or Quoted depending on whether the next character is a quote. It's not possible to stay in this state.
                        if (ch == '"')
                        {
                            // We just read a double double-quote within a quoted context, so we add the quote to the buffer and re-enter the quoted state.
                            buffer.AppendNormalChar(ch);
                            state = LexerState.Quoted;
                        }
                        else if (ch == ' ' || ch == '\t')
                        {
                            // In this case, the double-quote we just read did in fact end the quotation, so we publish the argument and restart in the default state.
                            yield return buffer.Consume();
                            state = LexerState.Default;
                        }
                        else
                        {
                            // If the double-quote is followed by a non-quote, non-whitespace character, then it's considered a continuation of the argument (leaving the quoted state).
                            buffer.AppendChar(ch);
                            state = LexerState.Argument;
                        }

                        break;
                }
            }

            // If we end in the middle of an argument (or even a quotation), then we just publish what we have.
            if (state != LexerState.Default)
            {
                yield return buffer.Consume();
            }
        }

        /// <summary>
        /// Takes a list of arguments to pass to a program, and quotes them. This method does not quote or escape special shell characters.
        /// </summary>
        /// <param name="arguments">The arguments to quote (if necessary) and concatenate into a command line.</param>
        /// <returns>The command line.</returns>
        public string Escape(IEnumerable<string> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            // Escape each argument (if necessary) and join them with spaces.
            return string.Join(" ", arguments.Select(argument =>
            {
                // An argument does not need escaping if it does not have any whitespace or quote characters.
                if (!argument.Any(ch => ch == ' ' || ch == '\t' || ch == '"') && argument != string.Empty)
                {
                    return argument;
                }

                // To escape the argument, wrap it in double-quotes and escape existing double-quotes, doubling any existing escape characters but only if they precede a double-quote.
                var ret = new StringBuilder();
                ret.Append('"');
                var backslashes = 0;
                foreach (var ch in argument)
                {
                    if (ch == '\\')
                    {
                        ++backslashes;
                    }
                    else if (ch == '"')
                    {
                        ret.Append(new string('\\', 2 * backslashes + 1));
                        backslashes = 0;
                        ret.Append(ch);
                    }
                    else
                    {
                        ret.Append(new string('\\', backslashes));
                        backslashes = 0;
                        ret.Append(ch);
                    }
                }

                ret.Append(new string('\\', backslashes));
                ret.Append('"');
                return ret.ToString();
            }));
        }
    }
}
