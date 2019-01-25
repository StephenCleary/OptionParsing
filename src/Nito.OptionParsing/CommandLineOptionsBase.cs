using System.Collections.Generic;

namespace Nito.OptionParsing
{
    /// <summary>
    /// A base class for arguments classes, which use option attributes on their properties. Note that arguments classes do not <i>need</i> to derive from <see cref="CommandLineOptionsBase"/>, but they do need to implement <see cref="ICommandLineOptions"/>.
    /// </summary>
    public abstract class CommandLineOptionsBase: ICommandLineOptions
    {
        /// <inheritdoc />
        public CommandLineOptionsSettings CommandLineOptionsSettings { get; } = new CommandLineOptionsSettings();

        /// <summary>
        /// The list of additional positional arguments after those specified by <see cref="PositionalArgumentAttribute"/>.
        /// </summary>
        [PositionalArguments]
        protected List<string> AdditionalArguments { get; private set; } = new List<string>();

        /// <summary>
        /// Retrieves <see cref="AdditionalArguments"/> and then sets <see cref="AdditionalArguments"/> to a new empty list.
        /// </summary>
        protected List<string> GetAndResetAdditionalArguments()
        {
            var result = AdditionalArguments;
            AdditionalArguments = new List<string>();
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// This implementation checks to ensure that <see cref="P:Nito.OptionParsing.CommandLineOptionsBase.AdditionalArguments" /> is empty. Derived classes do not have to invoke the base method, if they wish to allow additional positional arguments.
        /// </summary>
        public virtual void Validate()
        {
            if (AdditionalArguments.Count != 0)
                throw new UnknownOptionException($"Unknown parameter {AdditionalArguments[0]}");
        }

        /// <inheritdoc />
        /// <summary>
        /// This implementation does nothing.
        /// </summary>
        public virtual void Done()
        {
        }
    }
}
