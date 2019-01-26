using System;
using System.Collections.Generic;
using System.Text;
using Nito.OptionParsing.Lexing;
using Xunit;

namespace Nito.OptionParsing.UnitTests.UseCases
{
    public class CommandsUseCase
    {
        // Illustrates the use of a "command" parmaeter that then drives the meanings of following options.
        // Command options are somewhat common (e.g., "git status", "docker pull"), but please consider carefully before adopting this pattern.
        // This example has a single command which must be the first parameter.
        // It's possible, but more complex, to also:
        // - Have "global" options that come before the command parameter.
        // - Have sub-commands. Though really, at that point, how confusing would that be for your users?

        private sealed class Options: CommandLineOptionsBase
        {
            // We have to set ParseOptionsAfterPositionalArguments to false so that all the subcommand options are collected together
            //  in AdditionalArguments and passed to the subcommand option parsing.
            public Options() => CommandLineOptionsSettings.ParseOptionsAfterPositionalArguments = false;

            public MeasureOptions Measure { get; set; }
            public CutOptions Cut { get; set; }
            public HelpOptions Help { get; set; }

            public enum Commands
            {
                Unspecified,
                Help,
                Measure,
                Cut,
            };

            [PositionalArgument(0)]
            private Commands Command { get; set; }

            public override void Done()
            {
                if (Command == Commands.Measure)
                    Measure = CommandLineOptionsParser.Parse<MeasureOptions>(GetAndResetAdditionalArguments());
                else if (Command == Commands.Cut)
                    Cut = CommandLineOptionsParser.Parse<CutOptions>(GetAndResetAdditionalArguments());
                else if (Command == Commands.Help)
                    Help = CommandLineOptionsParser.Parse<HelpOptions>(GetAndResetAdditionalArguments());
            }

            public override void Validate()
            {
                base.Validate();
                if (Command == Commands.Unspecified)
                    throw new OptionParsingException("No command specified.");
            }

            // Options common to all commands
            public abstract class CommonOptions : CommandLineOptionsBase
            {
                [Option("verbose", 'v', OptionArgument.None)] public bool Verbose { get; set; }
            }

            public sealed class MeasureOptions: CommonOptions
            {
                [Option("length", 'l')] public int Length { get; set; }
                [Option("width", 'w')] public int Width { get; set; }
                [Option("height", 'h')] public int Height { get; set; }
            }

            public sealed class CutOptions : CommonOptions
            {
                [Option("Label", 'l')] public string Label { get; set; }
                [Option("Force", 'f')] public decimal Force { get; set; }
            }

            public sealed class HelpOptions : CommandLineOptionsBase
            {
                [Option("long", 'l', OptionArgument.None)] public bool Long { get; set; }
                [PositionalArgument(0)] public Commands Command { get; set; }
            }
        }

        [Fact]
        public void CommandNotSpecified_Throws()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("");
            Assert.Throws<OptionParsingException>(() => CommandLineOptionsParser.Parse<Options>(commandLine));
        }

        [Fact]
        public void MeasureCommand_Example()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("measure --length 13 -w 7 -h 12 -v");
            var options = CommandLineOptionsParser.Parse<Options>(commandLine);
            Assert.Null(options.Cut);
            Assert.Null(options.Help);
            Assert.NotNull(options.Measure);
            Assert.Equal(13, options.Measure.Length);
            Assert.Equal(7, options.Measure.Width);
            Assert.Equal(12, options.Measure.Height);
            Assert.True(options.Measure.Verbose);
        }

        [Fact]
        public void CutCommand_Example()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("cut -l bob -v");
            var options = CommandLineOptionsParser.Parse<Options>(commandLine);
            Assert.Null(options.Measure);
            Assert.Null(options.Help);
            Assert.NotNull(options.Cut);
            Assert.Equal("bob", options.Cut.Label);
            Assert.True(options.Cut.Verbose);
        }

        [Fact]
        public void HelpCommand_Example()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("help cut -l");
            var options = CommandLineOptionsParser.Parse<Options>(commandLine);
            Assert.Null(options.Measure);
            Assert.Null(options.Cut);
            Assert.NotNull(options.Help);
            Assert.Equal(Options.Commands.Cut, options.Help.Command);
            Assert.True(options.Help.Long);
        }
    }
}
