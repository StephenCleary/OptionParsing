# Nito.OptionParsing
A flexible command-line parsing library

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

Out of the box, the following command line arguments are all supported:

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

[Conceptual docs](./doc/README.md)