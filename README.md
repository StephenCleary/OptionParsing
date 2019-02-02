# Nito.OptionParsing
A flexible command-line parsing library

[![AppVeyor](https://img.shields.io/appveyor/ci/StephenCleary/OptionParsing.svg?style=plastic)](https://ci.appveyor.com/project/StephenCleary/OptionParsing) [![codecov](https://img.shields.io/codecov/c/github/StephenCleary/OptionParsing.svg?style=plastic)](https://codecov.io/gh/StephenCleary/OptionParsing)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Nito.OptionParsing.svg?style=plastic)](https://www.nuget.org/packages/Nito.OptionParsing/)

[Conceptual Docs](./doc/README.md) - [API Docs](http://dotnetapis.com/pkg/Nito.OptionParsing)

## Quick Start

Define an `Options` class like this:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("label", 'l')]
  public string Label { get; set; }

  [Option("flag", 'f', OptionArgument.None)]
  public bool Flag { get; set; }
}
```

Then parse your command line arguments like this:

```C#
try
{
  var options = CommandLineOptionsParser.Parse<Options>();
}
catch (OptionParsingException ex) // OptionParsingException indicates user error
{
  Console.Error.WriteLine(ex.Message);
  Console.Error.WriteLine("Usage: MyApp.exe [OPTIONS]...");
  ...
}
```

Out of the box, the following command line argument styles are all supported:

```
// { Label = null, Flag = false }
MyApp.exe
```

```
// { Label = null, Flag = true }
MyApp.exe --flag
```

```
// { Label = "bob", Flag = true }
MyApp.exe -l bob -f
```

```
// { Label = "bob", Flag = false }
MyApp.exe --label:bob
MyApp.exe --label=bob
MyApp.exe --label bob
MyApp.exe -l=bob
MyApp.exe -l:bob
MyApp.exe -l bob
```

[More details](./doc/README.md)