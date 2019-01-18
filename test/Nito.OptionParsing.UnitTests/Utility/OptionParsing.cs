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
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        public static List<ParsedOption> ParseOptionsIgnoringCase(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCultureIgnoreCase, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        public static readonly IEqualityComparer<ParsedOption> ParsedOptionComparer = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition).ThenEquateBy(x => x.Argument);

        public static readonly IEqualityComparer<ParsedOption> CompareDefinitionsOnly = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition);
    }
}
