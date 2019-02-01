using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Utility methods for parsing command lines into command line option instances.
    /// </summary>
    public static class CommandLineOptionsParser
    {
        /// <summary>
        /// Parses the command line into this command line options object. Option definitions are determined by the attributes on the properties of <paramref name="commandLineOptions"/>. This method will call <see cref="ICommandLineOptions.Validate"/>.
        /// </summary>
        /// <typeparam name="T">The type of command line options object to initialize.</typeparam>
        /// <param name="commandLineOptions">The command line options object that is initialized. May not be <c>null</c>.</param>
        /// <param name="commandLine">The command line to parse, not including the process name. If <c>null</c>, the process' command line is lexed by <see cref="Nito.OptionParsing.Lexing.Win32CommandLineLexer"/>.</param>
        public static void Apply<T>(this T commandLineOptions, IEnumerable<string> commandLine = null)
            where T : class, ICommandLineOptions
        {
            if (commandLineOptions == null)
                throw new ArgumentNullException(nameof(commandLineOptions));
            new CommandLineOptionsContext(commandLineOptions.CommandLineOptionsSettings).ParseCommandLineOptions(commandLineOptions, commandLine);
        }

        /// <summary>
        /// Parses the command line into a new command line options object. Option definitions are determined by the attributes on the properties of the command line options type. This method will call <see cref="ICommandLineOptions.Validate"/>.
        /// </summary>
        /// <typeparam name="T">The type of command line options object to initialize.</typeparam>
        /// <param name="commandLine">The command line to parse, not including the process name. If <c>null</c>, the process' command line is lexed by <see cref="Nito.OptionParsing.Lexing.Win32CommandLineLexer"/>.</param>
        public static T Parse<T>(IEnumerable<string> commandLine = null)
            where T : class, ICommandLineOptions
        {
            var result = Activator.CreateInstance<T>();
            result.Apply(commandLine);
            return result;
        }
    }
}
