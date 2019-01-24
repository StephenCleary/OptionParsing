using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nito.OptionParsing.Converters;
using Nito.OptionParsing.LowLevel;

namespace Nito.OptionParsing
{
    internal sealed class CommandLineOptionsContext
    {
        /// <param name="settings">May be <c>null</c>.</param>
        public CommandLineOptionsContext(CommandLineOptionsSettings settings)
        {
            OptionArgumentValueConverters = settings?.OptionArgumentValueConverters ?? new IOptionArgumentValueConverter[0];
            StringComparer = settings?.StringComparer ?? StringComparer.CurrentCulture;
            SlashOptionsEnabled = settings?.SlashOptionsEnabled ?? false;
        }

        /// <summary>
        /// Custom value converters to use when parsing option arguments.
        /// </summary>
        private IReadOnlyCollection<IOptionArgumentValueConverter> OptionArgumentValueConverters { get; }

        /// <summary>
        /// The string comparer to use when parsing options (i.e., for parsing options in a case-insensitive manner).
        /// </summary>
        private StringComparer StringComparer { get; }

        /// <summary>
        /// Whether options can be passed with '/'.
        /// </summary>
        private bool SlashOptionsEnabled { get; }

        /// <summary>
        /// Actions to take as options are seen.
        /// </summary>
        private Dictionary<OptionDefinition, Action<string>> OptionActions { get; } = new Dictionary<OptionDefinition, Action<string>>();

        /// <summary>
        /// Actions to take as positional options are seen.
        /// </summary>
        private List<Action<string>> IndexedPositionalArgumentActions { get; } = new List<Action<string>>();

        /// <summary>
        /// The action to take as additional positional arguments are seen.
        /// </summary>
        private Action<string> AdditionalPositionalArgumentAction { get; set; }

        /// <summary>
        /// Creates a parsing delegate for a specified type. Uses <paramref name="converter"/> if it's compatible; otherwise uses the first compatible match from <see cref="OptionArgumentValueConverters"/>; otherwise attempts to call a <c>TryParse</c> method via reflection.
        /// If no parser can be found, returns <c>null</c>.
        /// </summary>
        /// <param name="type">The type to convert to. May not be <c>null</c>.</param>
        /// <param name="converter">The custom converter, if any. May be <c>null</c>.</param>
        private Func<string, object> TryGetExactParser(Type type, IOptionArgumentValueConverter converter)
        {
            if (converter != null && converter.CanConvert(type))
                return MakeAction(type, converter);
            var selected = OptionArgumentValueConverters.FirstOrDefault(x => x.CanConvert(type));
            if (selected != null)
                return MakeAction(type, selected);
            if (type == typeof(string))
                return Identity;

            var tryParse = type.GetMethod(
                "TryParse",
                BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(string), type.MakeByRefType() },
                null);
            if (tryParse != null && tryParse.ReturnType == typeof(bool))
            {
                return value =>
                {
                    var arguments = new object[] { value, null };
                    var result = (bool)tryParse.Invoke(null, arguments);
                    return result ? arguments[1] : null;
                };
            }

            if (type.IsEnum)
            {
                return value =>
                {
                    try
                    {
                        return Enum.Parse(type, value);
                    }
                    catch (OverflowException)
                    {
                        return null;
                    }
                };
            }

            return null;

            Func<string, object> MakeAction(Type t, IOptionArgumentValueConverter parser) => value => parser.TryConvert(t, value);
        }

        /// <summary>
        /// Creates a parsing delegate for a specified type. Uses <paramref name="converter"/> if it's compatible; otherwise uses the first compatible match from <see cref="OptionArgumentValueConverters"/>; otherwise attempts to call a <c>TryParse</c> method via reflection.
        /// If no parser can be found, throws <see cref="InvalidOperationException"/> eagerly (i.e., before parsing user input).
        /// </summary>
        /// <param name="type">The type to convert to. May not be <c>null</c>.</param>
        /// <param name="converter">The custom converter, if any. May be <c>null</c>.</param>
        private Func<string, object> GetParser(Type type, IOptionArgumentValueConverter converter)
        {
            var result = TryGetExactParser(type, converter);
            if (result != null)
                return result;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                result = TryGetExactParser(type.GenericTypeArguments[0], converter);
                if (result != null)
                    return result;
            }

            throw new InvalidOperationException($"Cannot determine how to parse option argument value of type {type.Name}.");
        }

        private static readonly Func<string, object> Identity = x => x;

        /// <summary>
        /// Creates a converter instance if one is specified. Only returns <c>null</c> if <paramref name="type"/> is <c>null</c>.
        /// </summary>
        /// <param name="type">The type of the custom converter, if any. May be <c>null</c>.</param>
        private IOptionArgumentValueConverter CreateConverter(Type type)
        {
            return type == null ? null :
                (Activator.CreateInstance(type) as IOptionArgumentValueConverter ??
                   throw new InvalidOperationException($"Option argument value converter {type.Name} does not implement {nameof(IOptionArgumentValueConverter)}."));
        }

        private void ApplyAttribute(ICommandLineOptions commandLineOptions, PropertyInfo property, OptionAttribute optionAttribute)
        {
            var optionDefinition = new OptionDefinition { LongName = optionAttribute.LongName, ShortName = optionAttribute.ShortName, Argument = optionAttribute.Argument };
            if (optionDefinition.Argument == OptionArgument.None)
            {
                // If the option takes no arguments, it must be applied to a boolean property.
                if (property.PropertyType != typeof(bool))
                    throw new InvalidOperationException($"OptionAttribute {optionDefinition.Name ?? ""} with no Argument may only be applied to a boolean property.");

                // If the option is specified, set the property to true.
                OptionActions.Add(optionDefinition, _ => property.SetOptionProperty(commandLineOptions, true));
            }
            else
            {
                // If the option takes an argument, then attempt to parse it to the correct type.
                var parser = GetParser(property.PropertyType, CreateConverter(optionAttribute.Converter));
                OptionActions.Add(optionDefinition, parameter =>
                {
                    if (parameter == null)
                        return;
                    var value = parser(parameter);
                    if (value == null)
                        throw new OptionArgumentException($"Could not parse {parameter} as {property.PropertyType.FriendlyTypeName()}.");
                    property.SetOptionProperty(commandLineOptions, value);
                });
            }
        }

        private void ApplyAttribute(ICommandLineOptions commandLineOptions, PropertyInfo property, PositionalArgumentAttribute positionalArgumentAttribute)
        {
            if (IndexedPositionalArgumentActions.Count <= positionalArgumentAttribute.Index)
                IndexedPositionalArgumentActions.AddRange(new Action<string>[positionalArgumentAttribute.Index - IndexedPositionalArgumentActions.Count + 1]);

            if (IndexedPositionalArgumentActions[positionalArgumentAttribute.Index] != null)
                throw new InvalidOperationException($"More than one property has a {nameof(PositionalArgumentAttribute)}.{nameof(PositionalArgumentAttribute.Index)} of {positionalArgumentAttribute.Index}.");

            // If the positional argument is specified, then attempt to parse it to the correct type.
            var parser = GetParser(property.PropertyType, CreateConverter(positionalArgumentAttribute.Converter));
            IndexedPositionalArgumentActions[positionalArgumentAttribute.Index] = parameter =>
            {
                var value = parser(parameter);
                if (value == null)
                    throw new OptionArgumentException($"Could not parse {parameter} as {property.PropertyType.FriendlyTypeName()}.");
                property.SetOptionProperty(commandLineOptions, value);
            };
        }

        private void ApplyAttribute(ICommandLineOptions commandLineOptions, PropertyInfo property, PositionalArgumentsAttribute positionalArgumentsAttribute)
        {
            if (AdditionalPositionalArgumentAction != null)
                throw new InvalidOperationException($"More than one property has a {nameof(PositionalArgumentsAttribute)}.");

            var addMethods = property.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters().Length == 1).ToList();
            if (!addMethods.Any())
                throw new InvalidOperationException($"Property with {nameof(PositionalArgumentsAttribute)} does not implement an Add method taking exactly one parameter.");

            if (addMethods.Count != 1)
                throw new InvalidOperationException($"Property with {nameof(PositionalArgumentsAttribute)} has more than one Add method taking exactly one parameter.");

            var addMethod = addMethods[0];

            // As the remaining positional arguments are specified, then attempt to parse it to the correct type and add it to the collection.
            var propertyType = addMethod.GetParameters()[0].ParameterType;
            var parser = GetParser(propertyType, CreateConverter(positionalArgumentsAttribute.Converter));
            AdditionalPositionalArgumentAction = parameter =>
            {
                var value = parser(parameter);
                if (value == null)
                    throw new OptionArgumentException($"Could not parse {parameter} as {propertyType.FriendlyTypeName()}.");
                addMethod.Invoke(property.GetValue(commandLineOptions, null), new[] { value });
            };
        }

        private void ApplyAttribute(ICommandLineOptions commandLineOptions, PropertyInfo property, OptionPresentAttribute optionPresentAttribute)
        {
            // This attribute must be applied to a boolean property.
            if (property.PropertyType != typeof(bool))
                throw new InvalidOperationException($"{nameof(OptionPresentAttribute)} may only be applied to a boolean property.");

            var optionDefinition = optionPresentAttribute.LongName != null ?
                OptionActions.FirstOrDefault(x => StringComparer.Equals(x.Key.LongName, optionPresentAttribute.LongName)).Key :
                OptionActions.FirstOrDefault(x => StringComparer.Equals(x.Key.ShortNameAsString, optionPresentAttribute.ShortNameAsString)).Key;
            if (optionDefinition == null)
                throw new InvalidOperationException($"{nameof(OptionPresentAttribute)} does not refer to an existing {nameof(OptionAttribute)} for option {optionPresentAttribute.Name}.");

            // If the option is specified, set the property to true.
            OptionActions[optionDefinition] = (Action<string>)Delegate.Combine(OptionActions[optionDefinition],
                (Action<string>)(_ => property.SetOptionProperty(commandLineOptions, true)));
        }

        private void ApplyAttributes(ICommandLineOptions commandLineOptions)
        {
            var commandLineOptionsType = commandLineOptions.GetType();
            foreach (var property in commandLineOptionsType.GetProperties())
            {
                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    // Handle [Option] attributes.
                    if (attribute is OptionAttribute optionAttribute)
                        ApplyAttribute(commandLineOptions, property, optionAttribute);

                    // Handle [PositionalArgument] attributes.
                    if (attribute is PositionalArgumentAttribute positionalArgumentAttribute)
                        ApplyAttribute(commandLineOptions, property, positionalArgumentAttribute);

                    // Handle [PositionalArguments] attributes.
                    if (attribute is PositionalArgumentsAttribute positionalArgumentsAttribute)
                        ApplyAttribute(commandLineOptions, property, positionalArgumentsAttribute);
                }
            }

            // At this point, all the option definitions are known, so we can process the [OptionPresent] attributes.
            foreach (var property in commandLineOptionsType.GetProperties())
            {
                var optionPresentAttribute = property.GetCustomAttributes(typeof(OptionPresentAttribute), true).OfType<OptionPresentAttribute>().FirstOrDefault();
                if (optionPresentAttribute == null)
                    continue;
                ApplyAttribute(commandLineOptions, property, optionPresentAttribute);
            }
        }

        private void ValidateAttributes()
        {
            for (var i = 0; i != IndexedPositionalArgumentActions.Count; ++i)
            {
                if (IndexedPositionalArgumentActions[i] == null)
                    throw new InvalidOperationException($"No property has a {nameof(PositionalArgumentAttribute)} with {nameof(PositionalArgumentAttribute.Index)} of {i}.");
            }

            if (AdditionalPositionalArgumentAction == null)
                throw new InvalidOperationException($"No property has a {nameof(PositionalArgumentsAttribute)}.");
        }

        private void ApplyCommandLineOptions(IEnumerable<ParsedOption> options)
        {
            var positionalArgumentIndex = 0;
            foreach (var option in options)
            {
                if (option.Definition == null)
                {
                    if (positionalArgumentIndex < IndexedPositionalArgumentActions.Count)
                        IndexedPositionalArgumentActions[positionalArgumentIndex](option.Argument);
                    else
                        AdditionalPositionalArgumentAction(option.Argument);

                    ++positionalArgumentIndex;
                }
                else
                {
                    OptionActions[option.Definition](option.Argument);
                }
            }
        }

        /// <summary>
        /// Parses the command-line options from <paramref name="commandLine"/> into <paramref name="commandLineOptions"/>.
        /// </summary>
        /// <param name="commandLineOptions">The command line options object. May not be <c>null</c>.</param>
        /// <param name="commandLine">The command line. May be <c>null</c>.</param>
        public void ParseCommandLineOptions(ICommandLineOptions commandLineOptions, IEnumerable<string> commandLine)
        {
            ApplyAttributes(commandLineOptions);
            ValidateAttributes();
            var parser = new OptionParser(StringComparer, OptionActions.Keys, commandLine, SlashOptionsEnabled);
            ApplyCommandLineOptions(parser);
            commandLineOptions.Validate();
        }
    }
}
