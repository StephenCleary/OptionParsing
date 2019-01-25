using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nito.OptionParsing.UnitTests.UseCases
{
    public class CommandsUseCase
    {
        private sealed class Options: CommandLineOptionsBase
        {
            // We have to set ParseOptionsAfterPositionalArguments to false so that all the subcommand options are collected together
            //  in AdditionalArguments and passed to the subcommand option parsing.
            public Options() => CommandLineOptionsSettings.ParseOptionsAfterPositionalArguments = false;

            public MeasureOptions Measure { get; set; }
            public CutOptions Cut { get; set; }

            private enum Commands
            {
                None,
                Measure,
                Cut
            };
            [PositionalArgument(0)]
            private Commands Command { get; set; }

            public override void Done()
            {
                if (Command == Commands.Measure)
                    Measure = CommandLineOptionsParser.Parse<MeasureOptions>(GetAndResetAdditionalArguments());
                else if (Command == Commands.Cut)
                    Cut = CommandLineOptionsParser.Parse<CutOptions>(GetAndResetAdditionalArguments());
            }

            public override void Validate()
            {
                base.Validate();
                if (Command == Commands.None)
                    throw new OptionParsingException("No command specified.");
            }

            public sealed class MeasureOptions: CommandLineOptionsBase
            {
                [Option("length", 'l')] public int Length { get; set; }
                [Option("width", 'w')] public int Width { get; set; }
                [Option("height", 'h')] public int Height { get; set; }
            }

            public sealed class CutOptions : CommandLineOptionsBase
            {
                [Option("Label", 'l')] public string Label { get; set; }
                [Option("Force", 'f')] public decimal Force { get; set; }
            }
        }
    }
}
