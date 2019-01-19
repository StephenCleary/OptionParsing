using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.OptionParsing.Converters;

namespace Nito.OptionParsing
{
    /// <summary>
    /// Settings that control how command-line options are parsed.
    /// </summary>
    public sealed class CommandLineOptionsSettings
    {
        /// <summary>
        /// Custom value converters to use when parsing option arguments. May be <c>null</c>.
        /// </summary>
        public IReadOnlyCollection<IOptionArgumentValueConverter> OptionArgumentValueConverters { get; set; }

        /// <summary>
        /// The string comparer to use when parsing options (i.e., for parsing options in a case-insensitive manner). If <c>null</c>, then the string comparer for the current culture is used.
        /// </summary>
        public StringComparer StringComparer { get; set; }

        /// <summary>
        /// Whether options can be passed with '/'. Note that if you set this to <c>true</c>, then you cannot treat filenames starting with a '/' as positional arguments.
        /// </summary>
        public bool SlashOptionsEnabled { get; set; }
    }
}
