## Terminology

A *parameter* is a string that is passed to your program. These parameters are then parsed into *options*, *option arguments*, and *positional arguments*.

An *option* is a setting that your program gets from its command line, that instructs it on how to behave. Options may have *option arguments*, or they may not.

A *positional argument* is a parameter that is not associated with an option. These are sometimes used to indicate a "target" on which to operate, e.g., a filename.

## Option Names

Options may have a short name, a long name, or both. Short names are passed with a single dash (e.g., `-l`); long names are passed with a double dash (e.g., `--label`).

Design Tip: Less-common options should have only a long name.

Option names may not contain `:` or `=`. While it's possible to include spaces in an option name, please don't. That's just crazy.

Option names must be unique within the scope of an options class. This is true for both short names and long names; each short name must be unique compared to all other short names, and each long name must be unique compared to all other long names.

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("label")] // Option with only a long name
  public string Label { get; set; }

  [Option('f')] // Option with only a short name
  public string Frob { get; set; }

  [Option("test", 't')] // Option with both long and short name
  public string Test { get; set; }
}
```

## Flag Options

"Flag" options are options that do not take an option argument. As such, their only "value" is whether they are passed or not.

Flag options may only be on `bool` properties. The property is set to `true` if the flag is passed.

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option('f', OptionArgument.None)]
  public bool Flag { get; set; }
}
```

Also see [inverse aliases](#inverse-aliases).

## Option Arguments

Any options that are not [flag options](#flag-options) take an argument.

### Default Option Argument Values

At the beginning of option parsing, the options type is default-constructed. If you want a default value for an option argument that is not `default(T)`, then you can initialize that argument to that value:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", 'l')]
  public int Level { get; set; } // Defaults to 0

  [Option("strength", 's')]
  public int Strength { get; set; } = 3; // Defaults to 3
}
```

Sometimes a default value doesn't make sense; you want to know whether the user passed a value, no matter what the value was. In this case, you can use nullable value types to detect whether the user passed a value at all:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", 'l')]
  public int? Level { get; set; } // Defaults to null
}
```

With the type above, `MyApp.exe` will result in `{ Level = null }`, and `MyApp.exe -l 0` will result in `{ Level = 0 }`.

### Parsing Option Argument Values

Option arguments passed on the command line are, of course, strings. Nito.OptionParsing will attempt to parse option arguments to the type of the property.

Out of the box, Nito.OptionParsing supports:
- `string` - value is just copied straight through.
- Enumerations (and nullable enumerations) - parsed in a case-insensitive manner.
- Any type with a static member `bool T.TryParse(string value, out T result)` (and nullable versions of those types).

This is sufficient to support enums, `string`, `char`, `int`, `uint`, `char`, `bool`, `byte`, `sbyte`, `short`, `ushort`, `long`, `ulong`, `float`, `double`, `decimal`, `BigInteger`, `Guid`, `DateTime`, `DateTimeOffset`, `TimeSpan`, etc. However, some of these default parsings may not have the desired semantics; e.g., `TimeSpan` is pretty strict with what strings it considers valid, and doesn't allow a nice shorthand notation like `"3h"`.

If you have more advanced parsing requirements, you can [define your own converter](#custom-converters).

### Optional Option Arguments

Most options either [take no arguments](#flag-options) or require an argument (the default behavior). However, some options may have an *optional argument*. The semantics are that if an argument follows the option, then it has an argument; but if another option follows the option, then it doesn't.

Optional arguments look similar to regular arguments:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", OptionArgument.Optional)]
  public int? Level { get; set; }
}
```

The problem with optional arguments is that if no argument is passed, then the option property isn't set. So for the example above, `MyApp.exe --level 13` will result in `{ Level = 13 }`, but both `MyApp.exe` and `MyApp.exe --level` will result in `{ Level = null }`.

To resolve this ambiguity, you should use `OptionPresentAttribute`:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", OptionArgument.Optional)]
  public int? Level { get; set; }
  
  [OptionPresent("level")]
  public bool LevelPresent { get; set; }
}
```

Now, `MyApp.exe` will result in `{ Level = null, LevelPresent = false }`; `MyApp.exe --level` will result in `{ Level = null, LevelPresent = true }`, and `MyApp.exe --level 13` will result in `{ Level = 13, LevelPresent = true }`.

## Positional Arguments

Positional arguments are parameters passed on the command line that are not associated with an option. These often have a "target" kind of meaning, e.g., a filename on which to operate.

Basic positional arguments are indicated by `PositionalArgumentAttribute`:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [PositionalArgument(0)]
  public string Filename { get; set; }
}
```

Usually, there's only one positional argument, but if you need more, just specify a different index:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [PositionalArgument(0)]
  public string From { get; set; }
  
  [PositionalArgument(1)]
  public string To { get; set; }
}
```

In addition, every `ICommandLineOptions` class must have a single `[PositionalArguments]` (plural) property, which collects any additional positional arguments. The `CommandLineOptionsBase` type includes a `protected` property `[PositionalArguments] List<string> AdditionalArguments { get; }`, so all unexpected arguments end up there. Note that unexpected arguments are considered a usage error by default; see [validation](#validation).

## Validation

Every `ICommandLineOptions` class must define a `Validate` method which it uses to validate its parameters. This method should throw an `OptionParsingException` to indicate user error if there is anything wrong with its option values.

The `CommandLineOptionsBase.Validate` method just checks its `AdditionalArguments` member and throws if there is anything there. You can override this method to do your own validation.

### Example: Range Values

You should use validation to ensure your options are within a valid range.

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", 'l')]
  public int Level { get; set; }

  public override void Validate()
  {
    base.Validate();
    if (Level < 0 || Level > 10)
      throw new OptionParsingException("You must specify a level between 0 and 10, inclusive.")
  }
}
```

### Example: Required Options

You can use validation to require an option. This is not recommended (options should be optional), but it is possible:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", 'l')]
  public int? Level { get; set; }

  public override void Validate()
  {
    base.Validate();
    if (Level == null)
      throw new OptionParsingException("You must specify a level by passing --level or -l.")
  }
}
```

## Pipeline

When your application calls `CommandLineOptionsParser.Parse<Options>()`, this is what happens:

1. The application's command line is lexed. Alternatively, you can pass in the command line arguments yourself (as a collection of `string`s) to skip this step.
1. The `Options` type is examined and the various attributes are used to build option definitions. Any invalid configuration is detected and an exception is thrown if there will be a problem (e.g., if more than one option has the same name, or if a converter cannot be found for a specific type).
1. An instance of the `Options` type is default-created.
1. The command line arguments are interpreted according to the options defined. As each command line option is encountered, its corresponding property is set (or added to, in the case of `PositionalArguments`).
1. `ICommandLineOptions.Done` is invoked. This gives the `Options` type a last chance to clean up (useful for some advanced scenarios).
1. `ICommandLineOptions.Validate` is invoked.
1. The fully parsed, validated `Options` instance is returned to the caller.

Once you know this pipeline (particularly how options are expressed through property setters), you're ready for more advanced techniques.

# Cookbook

## Multiple argument values

By default, property values overwrite each other, so later option argument values merely overwrite earlier ones:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", 'l')]
  public int Level { get; set; }
}
```

With the type above, `MyApp.exe -l 3 -l 7 -l 13` will result in `{ Level = 13 }`.

This is desirable behavior for most applications. However, if you want the user to be able to specify multiple values, you can do this by collecting them from the property setter, as such:

```C#
private sealed class Options : CommandLineOptionsBase
{
  public List<int> Levels = new List<int>();

  [Option("level", 'l')]
  private int Level { set => Levels.Add(value); }
}
```

Now the input `MyApp.exe -l 3 -l 7 -l 13` will result in `{ Levels = [3, 7, 13] }`. Note that the `Level` property is `private` so that it's only used by `CommandLineOptionsParser`, and the rest of the application doesn't need to know about it.

## Preventing multiple argument values

The opposite scenario of [allowing multiple argument values](#multiple-argument-values) is *preventing* multiple argument values. In this case, you want to actively prevent the user from specifying the option more than once. Again, you don't want the overwrite-value behavior that is the default. And again, you can solve this with a custom property setter:

```C#
private sealed class Options : CommandLineOptionsBase
{
  private int? _level;

  [Option("level", 'l')]
  public int? Level
  {
    get => _level;
    set
    {
        if (_level == null)
            _level = value;
        else
            throw new InvalidOperationException("Level passed multiple times");
    }
  }
}
```

With the type above, `MyApp.exe -l 3 -l 7` will result in an `OptionParsingException`; the exception thrown by the setter is wrapped in an `OptionParsingException`.

## Custom converters

To handle advanced parsing needs for option arguments, you can define your own converter by deriving from `IOptionArgumentValueConverter`:

```C#
public sealed class Level { public int Value { get; set; } }

public sealed class LevelConverter : IOptionArgumentValueConverter
{
  public bool CanConvert(Type type) => type == typeof(Level);
  public object TryConvert(Type type, string text)
  {
    return int.TryParse(text, out var value) ? new Level { Value = value } : null;
  }
}
```

You can then specify that converter on a specific option:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("level", Converter = typeof(LevelConverter))]
  public Level Level { get; set; }
}
```

Or, if you have many options of this type, you can include your custom converter in the parser settings:

```C#
private sealed class Options : CommandLineOptionsBase
{
  public Options()
  {
    CommandLineOptionsSettings.OptionArgumentValueConverters = new List<IOptionArgumentValueConverter>
    {
      new LevelConverter()
    };
  }
  
  [Option("level1")]
  public Level Level1 { get; set; }
  
  [Option("level2")]
  public Level Level2 { get; set; }
}
```

## Inverse aliases

Inverse aliases are sometimes useful with [flag options](#flag-options); the idea is that you have *two* options which mean the opposite of each other. Example:

```C#
private sealed class Options : CommandLineOptionsBase
{
  [Option("force", OptionArgument.None)]
  public bool Force { get; set; }

  [Option("no-force", OptionArgument.None)]
  private bool NoForce { set => Force = !value; }
}
```

With the options above, the user can pass either `--force` or `--no-force`, and they act as opposites of each other.

## Case sensitivity

Options are case sensitive by default, so `-f` is distinct from `-F`. Some applications with a Windows history prefer case-insensitive option names. This can be done by specifying a `StringComparer` in the parsing settings:

```C#
private sealed class Options : CommandLineOptionsBase
{
  public Options()
  {
    CommandLineOptionsSettings.StringComparer = StringComparer.InvariantCultureIgnoreCase;
  }

  [Option("level", 'l')]
  public int Level { get; set; }
}
```

With the options above, `MyApp.exe -l 13` and `MyApp.exe -L 13` are equivalent.

## Command Options

TODO - [see unit tests](https://github.com/StephenCleary/OptionParsing/blob/master/test/Nito.OptionParsing.UnitTests/UseCases/CommandsUseCase.cs)

## Multiple Option Groups

TODO - [see unit tests](https://github.com/StephenCleary/OptionParsing/blob/master/test/Nito.OptionParsing.UnitTests/UseCases/MultipleGroupsUseCase.cs)