using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nito.Comparers;
using Nito.OptionParsing.LowLevel;

namespace Nito.OptionParsing.UnitTests.Utility
{
    public static class OptionParsing
    {
        public static List<ParsedOption> ParseOptions(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine, false).ToList();
        }

        public static List<ParsedOption> ParseOptionsIgnoringCase(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCultureIgnoreCase, definitions ?? new OptionDefinition[0], commandLine, false).ToList();
        }

        public static List<ParsedOption> ParseSlashOptions(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine, true).ToList();
        }

        public static List<ParsedOption> ParseSlashOptionsIgnoringCase(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCultureIgnoreCase, definitions ?? new OptionDefinition[0], commandLine, true).ToList();
        }

        public static T Parse<T>(params string[] commandLine) where T: class, ICommandLineOptions => CommandLineOptionsParser.Parse<T>(commandLine);

        public static T Parse<T>(CommandLineOptionsSettings settings, params string[] commandLine) where T : class, ICommandLineOptions => CommandLineOptionsParser.Parse<T>(commandLine, settings);

        public static readonly IEqualityComparer<ParsedOption> ParsedOptionComparer = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition).ThenEquateBy(x => x.Argument);

        public static readonly IEqualityComparer<ParsedOption> CompareDefinitionsOnly = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition);
    }
}
