using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Nito.OptionParsing.Lexing;
using Xunit;

namespace Nito.OptionParsing.UnitTests.UseCases
{
    public class MultipleGroupsUseCase
    {
        // Illustrates the use of option "groups" that apply to the next positional argument.
        // This is quite advanced and not common, but is an approach used by some programs such as ffmpeg.

        public sealed class Options : CommandLineOptionsBase
        {
            public Options() => CommandLineOptionsSettings.ParseOptionsAfterPositionalArguments = false;

            [Option("length", 'l')] public int? Length { get; set; }
            [Option("tag", 't')] public string Tag { get; set; }
            [PositionalArgument(0)] public string Filename { get; set; }

            public Options Next { get; private set; }

            public override void Done()
            {
                var args = GetAndResetAdditionalArguments();
                if (args.Count != 0)
                    Next = CommandLineOptionsParser.Parse<Options>(args);
            }
        }

        [Fact]
        public void ExampleUsage()
        {
            // Note: In real-world code, don't parse the command line yourself; just call CommandLineOptionsParser.Parse without any arguments.
            var commandLine = Win32CommandLineLexer.Instance.Lex("first.mp4 -t tag1 file1.mp4 -l 13 -t tag2 file2.mp4 file3.mp4");
            var options = CommandLineOptionsParser.Parse<Options>(commandLine);
            Assert.Null(options.Length);
            Assert.Null(options.Tag);
            Assert.Equal("first.mp4", options.Filename);
            Assert.NotNull(options.Next);
            Assert.Null(options.Next.Length);
            Assert.Equal("tag1", options.Next.Tag);
            Assert.Equal("file1.mp4", options.Next.Filename);
            Assert.NotNull(options.Next.Next);
            Assert.Equal(13, options.Next.Next.Length);
            Assert.Equal("tag2", options.Next.Next.Tag);
            Assert.Equal("file2.mp4", options.Next.Next.Filename);
            Assert.NotNull(options.Next.Next.Next);
            Assert.Null(options.Next.Next.Next.Length);
            Assert.Null(options.Next.Next.Next.Tag);
            Assert.Equal("file3.mp4", options.Next.Next.Next.Filename);
            Assert.Null(options.Next.Next.Next.Next);
        }
    }
}
