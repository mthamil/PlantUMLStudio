using System.Collections.Generic;
using System.IO;
using SharpEssentials.Collections;

namespace PlantUmlStudio.Core
{
    /// <summary>
    /// Helps with constructing command line arguments for an external process.
    /// </summary>
    public class ArgumentsBuilder
    {
        private readonly string _flagPrefix;
        private readonly ICollection<Argument> _arguments = new List<Argument>();

        /// <summary>
        /// Initializes a new <see cref="ArgumentsBuilder"/>.
        /// </summary>
        /// <param name="flagPrefix">The string that indicates a flag, such as "-" or "/". The default is "-".</param>
        public ArgumentsBuilder(string flagPrefix = "-")
        {
            _flagPrefix = flagPrefix;
        }

        /// <summary>
        /// Applies a flag argument and an optional parameter value.
        /// </summary>
        public ArgumentsBuilder Arg(string flag, string value = null)
        {
            _arguments.Add(new Argument(_flagPrefix, flag, value));
            return this;
        }

        /// <summary>
        /// Applies a flagless value as an argument.
        /// </summary>
        public ArgumentsBuilder Value(string value) => Arg(null, value);

        /// <summary>
        /// Returns the constructed arguments string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _arguments.ToDelimitedString(" ");

        /// <summary>
        /// Converts a builder to its constructed string form.
        /// </summary>
        public static implicit operator string(ArgumentsBuilder builder) => builder.ToString();

        private struct Argument
        {
            private readonly string _prefix;
            private readonly string _flag;
            private readonly string _value;

            public Argument(string prefix, string flag, string value)
            {
                _prefix = prefix;
                _flag = flag;
                _value = value;
            }

            public override string ToString()
            {
                return _flag == null
                    ? _value
                    : $"{_prefix}{_flag}{(_value == null ? "" : " ")}{_value ?? ""}";
            }
        }
    }

    /// <summary>
    /// Provides additional convenience methods for <see cref="ArgumentsBuilder"/>.
    /// </summary>
    public static class ArgumentsBuilderExtensions
    {
        /// <summary>
        /// Conditionally applies an argument.
        /// </summary>
        public static ArgumentsBuilder ArgIf(this ArgumentsBuilder builder, bool precondition, string flag, string value = null) 
            => precondition ? builder.Arg(flag, value) : builder;

        /// <summary>
        /// Applies a file argument. Quotes will surround the file name.
        /// </summary>
        public static ArgumentsBuilder Arg(this ArgumentsBuilder builder, string flag, FileInfo value) 
            => builder.Arg(flag, $@"""{value.FullName}""");

        /// <summary>
        /// Applies a file as a flagless argument value. Quotes will surround the file name.
        /// </summary>
        public static ArgumentsBuilder Value(this ArgumentsBuilder builder, FileInfo value) 
            => builder.Value($@"""{value.FullName}""");
    }
}