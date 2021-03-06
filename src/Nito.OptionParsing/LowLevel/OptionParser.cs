﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nito.OptionParsing.Lexing;

namespace Nito.OptionParsing.LowLevel
{
    /// <summary>
    /// A type that parses command line options based on a set of option definitions.
    /// </summary>
    public sealed class OptionParser: IEnumerable<ParsedOption>
    {
        /// <summary>
        /// The string comparer to use when parsing options. This is never <c>null</c>.
        /// </summary>
        private readonly StringComparer _stringComparer;

        /// <summary>
        /// The option definitions. This is never <c>null</c>.
        /// </summary>
        private readonly IReadOnlyCollection<OptionDefinition> _definitions;

        /// <summary>
        /// The command line to parse. This is never <c>null</c>.
        /// </summary>
        private readonly IEnumerable<string> _commandLine;

        /// <summary>
        /// Whether slash arguments are enabled.
        /// </summary>
        private readonly bool _slashOptionsEnabled;

        /// <summary>
        /// Whether options should be parsed after positional arguments.
        /// </summary>
        private readonly bool _parseOptionsAfterPositionalArguments;

        /// <summary>
        /// The last option read, or <c>null</c> if there is no option context. If this is not <c>null</c>, then it is an option that can take an argument (i.e., <see cref="OptionDefinition.Argument"/> will not be <see cref="OptionArgument.None"/>). This is always <c>null</c> if <see cref="_done"/> is <c>true</c>.
        /// </summary>
        private OptionDefinition _lastOption;

        /// <summary>
        /// Whether or not the option parsing is done. If this is <c>true</c>, then all remaining command line items are positional parameters, and <see cref="_lastOption"/> will be <c>null</c>.
        /// </summary>
        private bool _done;

        /// <param name="commandLine">The command line to parse, not including the process name. If <c>null</c>, the process' command line is lexed by <see cref="Win32CommandLineLexer"/>.</param>
        /// <param name="definitions">The option definitions. May not be <c>null</c>.</param>
        /// <param name="stringComparer">The string comparer to use when parsing options. If <c>null</c>, then the string comparer for the current culture is used.</param>
        /// <param name="slashOptionsEnabled">Whether slash arguments are enabled.</param>
        /// <param name="parseOptionsAfterPositionalArguments">Whether options should be parsed after positional arguments.</param>
        public OptionParser(StringComparer stringComparer, IReadOnlyCollection<OptionDefinition> definitions, IEnumerable<string> commandLine, bool slashOptionsEnabled, bool parseOptionsAfterPositionalArguments)
        {
            _stringComparer = stringComparer ?? StringComparer.CurrentCulture;
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _commandLine = commandLine ?? Win32CommandLineLexer.Instance.Lex().Skip(1);
            _slashOptionsEnabled = slashOptionsEnabled;
            _parseOptionsAfterPositionalArguments = parseOptionsAfterPositionalArguments;

            // Ensure that the option definitions are valid and names are unique.
            foreach (var definition in _definitions)
                definition.Validate();
            EnsureUnique("short name", _definitions.Where(x => x.ShortName != null).Select(x => x.ShortNameAsString));
            EnsureUnique("long name", _definitions.Where(x => x.LongName != null).Select(x => x.LongName));

            void EnsureUnique(string fieldName, IEnumerable<string> items)
            {
                var values = new HashSet<string>(_stringComparer);
                foreach (var item in items)
                {
                    if (!values.Add(item))
                        throw new InvalidOperationException($"Duplicate {fieldName} found for option {item}.");
                }
            }
        }

        /// <summary>
        /// Parses the command line into options.
        /// </summary>
        /// <returns>The sequence of options specified on the command line.</returns>
        public IEnumerator<ParsedOption> GetEnumerator()
        {
            foreach (var value in _commandLine)
            {
                // If the option parsing is done, then all remaining command line elements are positional arguments.
                if (_done)
                {
                    yield return new ParsedOption { Argument = value };
                    continue;
                }

                if (_lastOption != null && _lastOption.Argument == OptionArgument.Required)
                {
                    // Argument for a preceding option
                    yield return new ParsedOption { Definition = _lastOption, Argument = value };
                    _lastOption = null;
                    continue;
                }

                // Either there is no last option, or the last option's argument is optional.

                if (value == "--")
                {
                    // End-of-options marker

                    if (_lastOption != null)
                    {
                        // The last option was an option that takes an optional argument, without an argument.
                        yield return new ParsedOption { Definition = _lastOption };
                        _lastOption = null;
                    }

                    // All future parameters are positional arguments.
                    _done = true;
                    continue;
                }

                if (value.StartsWith("--"))
                {
                    // Long option

                    if (_lastOption != null)
                    {
                        // The last option was an option that takes an optional argument, without an argument.
                        yield return new ParsedOption { Definition = _lastOption };
                    }

                    string option;
                    string argument = null;
                    var argumentIndex = value.IndexOfAny(Constants.ArgumentDelimiters, 2);
                    if (argumentIndex == -1)
                    {
                        // No argument delimiters were found; the command line element is an option.
                        option = value.Substring(2);
                    }
                    else
                    {
                        // An argument delimiter was found; split the command line element into an option and its argument.
                        option = value.Substring(2, argumentIndex - 2);
                        argument = value.Substring(argumentIndex + 1);
                    }

                    // Find the option in the option definitions.
                    _lastOption = _definitions.FirstOrDefault(x => _stringComparer.Equals(x.LongName, option));
                    if (_lastOption == null)
                        throw new UnknownOptionException($"Unknown option {option} in parameter {value}");

                    if (argument != null)
                    {
                        // There is an argument with this long option, so it is complete.
                        if (_lastOption.Argument == OptionArgument.None)
                            throw new OptionArgumentException($"Option {option} cannot take an argument in parameter {value}");

                        yield return new ParsedOption { Definition = _lastOption, Argument = argument };
                        _lastOption = null;
                        continue;
                    }

                    if (_lastOption.Argument == OptionArgument.None)
                    {
                        // This long option does not take an argument, so it is complete.
                        yield return new ParsedOption { Definition = _lastOption };
                        _lastOption = null;
                    }

                    // This long option does take an argument.
                    continue;
                }

                if (value.StartsWith("-"))
                {
                    // Short option or short option run

                    if (_lastOption != null)
                    {
                        // The last option was an option that takes an optional argument, without an argument.
                        yield return new ParsedOption { Definition = _lastOption };
                    }

                    if (value.Length < 2)
                        throw new InvalidParameterException($"Invalid parameter {value}");

                    var option = value[1].ToString();
                    _lastOption = _definitions.FirstOrDefault(x => _stringComparer.Equals(x.ShortNameAsString, option));
                    if (_lastOption == null)
                        throw new UnknownOptionException($"Unknown option {option} in parameter {value}");

                    // The first short option may either have an argument or start a short option run
                    var argumentIndex = value.IndexOfAny(Constants.ArgumentDelimiters, 2);
                    if (argumentIndex == 2)
                    {
                        // The first short option has an argument.
                        if (_lastOption.Argument == OptionArgument.None)
                            throw new OptionArgumentException($"Option {option} cannot take an argument in parameter {value}");

                        yield return new ParsedOption { Definition = _lastOption, Argument = value.Substring(3) };
                        _lastOption = null;
                    }
                    else if (argumentIndex != -1)
                    {
                        // There is an argument delimiter somewhere else in the parameter string.
                        throw new InvalidParameterException($"Invalid parameter {value}");
                    }
                    else if (value.Length == 2)
                    {
                        // The first short option is the only one.
                        if (_lastOption.Argument == OptionArgument.None)
                        {
                            yield return new ParsedOption { Definition = _lastOption };
                            _lastOption = null;
                        }
                    }
                    else
                    {
                        // This is a short option run; they must not take arguments.
                        for (var i = 1; i != value.Length; ++i)
                        {
                            option = value[i].ToString();
                            _lastOption = _definitions.FirstOrDefault(x => _stringComparer.Equals(x.ShortNameAsString, option));
                            if (_lastOption == null)
                                throw new UnknownOptionException($"Unknown option {option} in parameter {value}");

                            if (_lastOption.Argument == OptionArgument.Required)
                                throw new InvalidParameterException($"Option {option} cannot be in a short option run (because it takes an argument) in parameter {value}");

                            yield return new ParsedOption { Definition = _lastOption };
                            _lastOption = null;
                        }
                    }

                    continue;
                }

                if (_slashOptionsEnabled && value.StartsWith("/"))
                {
                    // Short or long option

                    if (_lastOption != null)
                    {
                        // The last option was an option that takes an optional argument, without an argument.
                        yield return new ParsedOption { Definition = _lastOption };
                    }

                    if (value.Length < 2)
                        throw new InvalidParameterException($"Invalid parameter {value}");

                    string option;
                    string argument = null;
                    var argumentIndex = value.IndexOfAny(Constants.ArgumentDelimiters, 2);
                    if (argumentIndex == -1)
                    {
                        option = value.Substring(1);
                    }
                    else
                    {
                        option = value.Substring(1, argumentIndex - 1);
                        argument = value.Substring(argumentIndex + 1);
                    }

                    _lastOption = _definitions.FirstOrDefault(x => _stringComparer.Equals(x.LongName, option) || _stringComparer.Equals(x.ShortNameAsString, option));
                    if (_lastOption == null)
                        throw new UnknownOptionException($"Unknown option {option} in parameter {value}");

                    if (argument != null)
                    {
                        // There is an argument with this option, so it is complete.
                        if (_lastOption.Argument == OptionArgument.None)
                            throw new OptionArgumentException($"Option {option} cannot take an argument in parameter {value}");

                        yield return new ParsedOption { Definition = _lastOption, Argument = argument };
                        _lastOption = null;
                        continue;
                    }

                    if (_lastOption.Argument == OptionArgument.None)
                    {
                        // This option does not take an argument, so it is complete.
                        yield return new ParsedOption { Definition = _lastOption };
                        _lastOption = null;
                    }

                    // This option does take an argument.
                    continue;
                }

                if (_lastOption != null)
                {
                    // The last option was an option that takes an optional argument, with an argument.
                    yield return new ParsedOption { Definition = _lastOption, Argument = value };
                    _lastOption = null;
                    continue;
                }

                // A positional argument
                if (!_parseOptionsAfterPositionalArguments)
                    _done = true;
                yield return new ParsedOption { Argument = value };
            }

            if (_lastOption != null)
            {
                if (_lastOption.Argument == OptionArgument.Required)
                    throw new OptionArgumentException($"Missing argument for option {_lastOption.Name}");

                yield return new ParsedOption { Definition = _lastOption };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
