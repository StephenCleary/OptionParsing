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

## Option Names

Options may have a short name, a long name, or both. Short names are passed with a single dash (e.g., `-l`); long names are passed with a double dash (e.g., `--label`).

Less-common options should have only a long name.

## Flag Options

## Option Arguments

### Default Option Argument Values

// default
// nullable

### Parsing Option Argument Values

// Enums, TryParse, custom converters

### Optional Option Arguments

// OptionPresent

## Validation

// required options, range values required, etc.

## Positional Arguments

## Pipeline

for advanced usage

# Cookbook

Multiple argument values

Preventing multiple arguments values

Custom converters

Inverse aliases

Case sensitivity

Command Options

Multiple Option Groups