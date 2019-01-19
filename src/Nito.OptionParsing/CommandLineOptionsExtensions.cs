using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nito.OptionParsing.LowLevel;

namespace Nito.OptionParsing
{
    // TODO: split into different methods:
    // 0) Validate attribute usage.
    // 1) Generate definitions from attributes.
    // 2) Parse command line.
    // 3) Use ParsedOptions to fill out properties (allow injecting IEnumerable<T> for OptionParser).
    // 4) End-user validation.

    // Perhaps a CommandLineOptionsParsingContext?
    // - List of OptionArgumentConverters.
    // - Generated definitions.
    // - Generated list of actions to take per ParsedOption.

    // Converters:
    // - Allow user to define (or override) a default converter for a given type.
    // - Allow user to define a specific converter type for a given property.
    // - We maintain a collection of user-specified converters.
    //   - If property-specific converter, we use the first instance of that type without checking CanConvert.
    //     - Or create a new instance?
    //   - Otherwise, we use the first converter where CanConvert returns true for the property type.

    public static class CommandLineOptionsExtensions
    {
        /// <summary>
        /// Parses the command line into an arguments object. Option definitions are determined by the attributes on the properties of <paramref name="argumentsObject"/>. This method will call <see cref="IOptionArguments.Validate"/>.
        /// </summary>
        /// <typeparam name="T">The type of arguments object to initialize.</typeparam>
        /// <param name="argumentsObject">The arguments object that is initialized. May not be <c>null</c>.</param>
        /// <param name="commandLine">The command line to parse, not including the process name. If <c>null</c>, the process' command line is lexed by <see cref="Nito.OptionParsing.Lexing.Win32CommandLineLexer"/>.</param>
        /// <param name="stringComparer">The string comparison to use when parsing options. If <c>null</c>, then the string comparer for the current culture is used.</param>
        /// <param name="slashArgumentsEnabled">Whether slash arguments are enabled.</param>
        public static void Parse<T>(this T argumentsObject, IEnumerable<string> commandLine = null, StringComparer stringComparer = null, bool slashArgumentsEnabled = false)
            where T : class, ICommandLineOptions
        {
            if (stringComparer == null)
                stringComparer = StringComparer.CurrentCulture;

            // Generate option definitions from OptionAttributes on the arguments object.
            var options = new Dictionary<OptionDefinition, Action<string>>();
            var positionalArguments = new List<Action<string>>();
            Action<string> remainingPositionalArguments = null;
            var argumentsObjectType = argumentsObject.GetType();
            foreach (var property in argumentsObjectType.GetProperties())
            {
                var localProperty = property;

                // If the property specifies a [SimpleParser], then create a parser for that property.
                //var parserOverrideAttribute = property.GetCustomAttributes(typeof(SimpleParserAttribute), true).OfType<SimpleParserAttribute>().FirstOrDefault();
                //var parserOverride = ((parserOverrideAttribute == null) ? null : Activator.CreateInstance(parserOverrideAttribute.ParserType)) as ISimpleParser;

                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    // Handle [Option] attributes.
                    var optionAttribute = attribute as OptionAttribute;
                    if (optionAttribute != null)
                    {
                        var optionDefinition = new OptionDefinition { LongName = optionAttribute.LongName, ShortName = optionAttribute.ShortName, Argument = optionAttribute.Argument };
                        if (optionDefinition.Argument == OptionArgument.None)
                        {
                            // If the option takes no arguments, it must be applied to a boolean property.
                            if (localProperty.PropertyType != typeof(bool))
                                throw new InvalidOperationException($"OptionAttribute {optionDefinition.Name ?? ""} with no Argument may only be applied to a boolean property.");

                            // If the option is specified, set the property to true.
                            options.Add(optionDefinition, _ => localProperty.SetOptionProperty(argumentsObject, true));
                        }
                        else
                        {
                            // If the option takes an argument, then attempt to parse it to the correct type.
                            options.Add(optionDefinition, parameter =>
                            {
                                if (parameter == null)
                                    return;

                                //var value = parserOverride != null ? parserOverride.TryParse(parameter) : parserCollection.TryParse(parameter, localProperty.PropertyType);
                                //if (value == null)
                                //{
                                //    throw new OptionParsingException.OptionArgumentException("Could not parse  " + parameter + "  as " + FriendlyTypeName(Nullable.GetUnderlyingType(localProperty.PropertyType) ?? localProperty.PropertyType));
                                //}

                                //localProperty.SetOptionProperty(argumentsObject, value);
                            });
                        }
                    }

                    // Handle [PositionalArgument] attributes.
                    var positionalArgumentAttribute = attribute as PositionalArgumentAttribute;
                    if (positionalArgumentAttribute != null)
                    {
                        if (positionalArguments.Count <= positionalArgumentAttribute.Index)
                            positionalArguments.AddRange(new Action<string>[positionalArgumentAttribute.Index - positionalArguments.Count + 1]);

                        if (positionalArguments[positionalArgumentAttribute.Index] != null)
                            throw new InvalidOperationException($"More than one property has a PositionalArgumentAttribute.Index of {positionalArgumentAttribute.Index}.");

                        // If the positional argument is specified, then attempt to parse it to the correct type.
                        positionalArguments[positionalArgumentAttribute.Index] = parameter =>
                        {
                            //var value = parserOverride != null ? parserOverride.TryParse(parameter) : parserCollection.TryParse(parameter, localProperty.PropertyType);
                            //if (value == null)
                            //    throw new OptionParsingException.OptionArgumentException("Could not parse  " + parameter + "  as " + FriendlyTypeName(Nullable.GetUnderlyingType(localProperty.PropertyType) ?? localProperty.PropertyType));

                            //localProperty.SetOptionProperty(argumentsObject, value);
                        };
                    }

                    // Handle [PositionalArguments] attributes.
                    var positionalArgumentsAttribute = attribute as PositionalArgumentsAttribute;
                    if (positionalArgumentsAttribute != null)
                    {
                        if (remainingPositionalArguments != null)
                            throw new InvalidOperationException("More than one property has a PositionalArgumentsAttribute.");

                        var addMethods = localProperty.PropertyType.GetMethods().Where(x => x.Name == "Add" && x.GetParameters().Length == 1);
                        if (!addMethods.Any())
                            throw new InvalidOperationException("Property with PositionalArgumentsAttribute does not implement an Add method taking exactly one parameter.");

                        if (addMethods.Count() != 1)
                            throw new InvalidOperationException("Property with PositionalArgumentsAttribute has more than one Add method taking exactly one parameter.");

                        var addMethod = addMethods.First();

                        // As the remaining positional arguments are specified, then attempt to parse it to the correct type and add it to the collection.
                        remainingPositionalArguments = parameter =>
                        {
                            //var value = parserOverride != null ? parserOverride.TryParse(parameter) : parserCollection.TryParse(parameter, addMethod.GetParameters()[0].ParameterType);
                            //if (value == null)
                            //{
                            //    throw new OptionParsingException.OptionArgumentException("Could not parse  " + parameter + "  as " + FriendlyTypeName(Nullable.GetUnderlyingType(addMethod.GetParameters()[0].ParameterType) ?? addMethod.GetParameters()[0].ParameterType));
                            //}

                            //addMethod.Invoke(localProperty.GetValue(argumentsObject, null), new[] { value });
                        };
                    }
                }
            }

            // Handle [OptionPresent] attributes.
            var optionDefinitions = options.Keys;
            foreach (var property in argumentsObjectType.GetProperties())
            {
                var localProperty = property;

                var optionPresentAttribute = property.GetCustomAttributes(typeof(OptionPresentAttribute), true).OfType<OptionPresentAttribute>().FirstOrDefault();
                if (optionPresentAttribute == null)
                    continue;

                // This attribute must be applied to a boolean property.
                if (localProperty.PropertyType != typeof(bool))
                {
                    throw new InvalidOperationException("An OptionPresentAttribute may only be applied to a boolean property.");
                }

                OptionDefinition optionDefinition = null;
                if (optionPresentAttribute.LongName != null)
                {
                    optionDefinition = optionDefinitions.FirstOrDefault(x => stringComparer.Equals(x.LongName, optionPresentAttribute.LongName));
                }
                else
                {
                    optionDefinition = optionDefinitions.FirstOrDefault(x => stringComparer.Equals(x.ShortNameAsString, optionPresentAttribute.ShortNameAsString));
                }

                if (optionDefinition == null)
                {
                    throw new InvalidOperationException("OptionPresentAttribute does not refer to an existing OptionAttribute for option " + optionPresentAttribute.Name);
                }

                // If the option is specified, set the property to true.
                options[optionDefinition] = (Action<string>)Delegate.Combine(options[optionDefinition], (Action<string>)(_ => localProperty.SetOptionProperty(argumentsObject, true)));
            }

            // Verify options.
            for (int i = 0; i != positionalArguments.Count; ++i)
            {
                if (positionalArguments[i] == null)
                {
                    throw new InvalidOperationException("No property has a PositionalArgumentAttribute with Index of " + i + ".");
                }
            }

            if (remainingPositionalArguments == null)
            {
                throw new InvalidOperationException("No property has a PositionalArgumentsAttribute.");
            }

            // Parse the command line, filling in the property values.
            var parser = new OptionParser(stringComparer, optionDefinitions, commandLine, slashArgumentsEnabled);
            int positionalArgumentIndex = 0;
            foreach (var option in parser)
            {
                if (option.Definition == null)
                {
                    if (positionalArgumentIndex < positionalArguments.Count)
                    {
                        var action = positionalArguments[positionalArgumentIndex];
                        action(option.Argument);
                    }
                    else
                    {
                        remainingPositionalArguments(option.Argument);
                    }

                    ++positionalArgumentIndex;
                }
                else
                {
                    var action = options[option.Definition];
                    action(option.Argument);
                }
            }

            // Have the arguments object perform its own validation.
            argumentsObject.Validate();
        }

        /// <summary>
        /// Sets a property on an arguments object, translating exceptions.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="argumentsObject">The arguments object.</param>
        /// <param name="value">The value for the property.</param>
        private static void SetOptionProperty(this PropertyInfo property, object argumentsObject, object value)
        {
            try
            {
                property.SetValue(argumentsObject, value, null);
            }
            catch (TargetInvocationException ex)
            {
                throw new OptionArgumentException(ex.InnerException?.Message ?? ex.Message, ex.InnerException);
            }
        }
    }
}
