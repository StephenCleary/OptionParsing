using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nito.OptionParsing.UnitTests
{
    public class CommandLineOptionsParserUnitTests
    {
        [Fact]
        public void DefaultValidate_ThrowsOnAdditionalPositionalParameters()
        {
            Assert.Throws<UnknownOptionException>(() => CommandLineOptionsParser.Parse<NoOptions>(new[] { "bob" }));
        }

        private sealed class NoOptions: CommandLineOptionsBase { }
    }
}
