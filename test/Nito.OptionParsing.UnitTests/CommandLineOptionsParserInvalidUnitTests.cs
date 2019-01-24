using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Nito.OptionParsing.UnitTests
{
    public class CommandLineOptionsParserInvalidUnitTests
    {
        private sealed class OptionPresentNoMatch : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; } = "default";
            [OptionPresent('z')] public bool SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void OptionPresent_NoMatch_Throws() => Assert.Throws<InvalidOperationException>(() => Parse<OptionPresentNoMatch>());

        private sealed class OptionPresentNonBoolean : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; } = "default";
            [OptionPresent('a')] public string SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void OptionPresent_NonBoolean_Throws() => Assert.Throws<InvalidOperationException>(() => Parse<OptionPresentNonBoolean>());
    }
}
