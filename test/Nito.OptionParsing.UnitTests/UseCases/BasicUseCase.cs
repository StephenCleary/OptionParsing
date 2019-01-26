using System;
using System.Collections.Generic;
using System.Text;
using Nito.OptionParsing.Lexing;
using Xunit;

namespace Nito.OptionParsing.UnitTests.UseCases
{
    public class BasicUseCase
    {
        private sealed class Options : CommandLineOptionsBase
        {
            [PositionalArgument(0)]
            public string Filename { get; set; }

            [Option("flag", 'f', Argument = OptionArgument.None)]
            public bool Flag { get; set; }

            [Option("label", 'l')]
            public string Label { get; set; }

            // Allow --help, -h, -? to all set this Help property.
            [Option("help", 'h', Argument = OptionArgument.None), Option('?', Argument = OptionArgument.None)]
            public bool Help { get; set; }
        }

        [Fact]
        public void SimpleExample()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("test.mp4 -l smith --flag");
            var options = CommandLineOptionsParser.Parse<Options>(commandLine);
            Assert.Equal("test.mp4", options.Filename);
            Assert.True(options.Flag);
            Assert.Equal("smith", options.Label);
        }
    }
}
