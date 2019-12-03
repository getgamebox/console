/*
 * This file is part of the GameBox package.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

using GameBox.Console.Exception;
using GameBox.Console.Process;
using GameBox.Console.Util;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GameBox.Console.Input
{
    /// <summary>
    /// <see cref="AbstractInput"/> is the default base class for all concrete Input classes.
    /// </summary>
    /// <remarks><seealso cref="InputArgs"/>The input comes from the CLI arguments (argv).</remarks>
    public abstract class AbstractInput : IInputStreamable
    {
        /// <summary>
        /// Command line given arguments.
        /// </summary>
        private Mixture[] arguments;

        /// <summary>
        /// Command line given options.
        /// </summary>
        private Mixture[] options;

        /// <summary>
        /// The input stream to read from when interacting with the user.
        /// </summary>
        private Stream inputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractInput"/> class.
        /// </summary>
        protected AbstractInput()
        {
            // if the definition is null then we do not need to Bind and Validate
            arguments = Array.Empty<Mixture>();
            options = Array.Empty<Mixture>();
            Definition = new InputDefinition();
        }

        /// <inheritdoc />
        public bool IsInteractive { get; private set; } = true;

        /// <inheritdoc />
        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets current input definition.
        /// </summary>
        protected InputDefinition Definition { get; private set; }

        /// <summary>
        /// Escapes a token through escapeshellarg if it contains unsafe chars.
        /// </summary>
        /// <param name="token">The specified token.</param>
        /// <returns>safe token.</returns>
        public static string EscapesToken(string token)
        {
            if (Regex.IsMatch(token, @"^[\w-]+$"))
            {
                return token;
            }

            return ProcessExecutor.Escape(token);
        }

        /// <inheritdoc />
        public void Bind(InputDefinition definition)
        {
            arguments = Array.Empty<Mixture>();
            options = Array.Empty<Mixture>();
            Definition = definition ?? new InputDefinition();

            Parse();
        }

        /// <inheritdoc />
        public void Validate()
        {
            var missingArguments = Arr.Filter(Definition.GetArguments(), argument =>
            {
                return argument.IsRequired &&
                       !Array.Exists(GetArguments(), givenArgument => givenArgument.Name == argument.Name);
            });

            if (missingArguments.Length > 0)
            {
                throw new ValidationException(
                    $"Not enough arguments (missing: {string.Join<InputArgument>(", ", missingArguments)}).");
            }
        }

        /// <inheritdoc />
        public Stream GetInputStream()
        {
            return inputStream;
        }

        /// <inheritdoc />
        public abstract Mixture GetFirstArgument();

        /// <inheritdoc />
        public Mixture GetArgument(string name)
        {
            if (!HasArgument(name))
            {
                throw new InvalidArgumentException($"The \"{name}\" argument does not exist.", nameof(name));
            }

            return Array.Find(GetArguments(), argument => argument.Name == name)
                   ?? Definition.GetArgument(name).GetDefault();
        }

        /// <inheritdoc />
        public Mixture GetOption(string name)
        {
            if (!HasOption(name))
            {
                throw new InvalidArgumentException($"The \"{name}\" option does not exist.", nameof(name));
            }

            return Array.Find(GetOptions(), option => option.Name == name)
                   ?? Definition.GetOption(name).GetDefault();
        }

        /// <inheritdoc />
        public virtual Mixture GetRawOption(string name, Mixture defaultValue = null, bool ignoreAdditional = false)
        {
            return defaultValue;
        }

        /// <inheritdoc />
        public void SetArgument(string name, Mixture value)
        {
            if (!HasArgument(name))
            {
                throw new InvalidArgumentException($"The \"{name}\" argument does not exist.", nameof(name));
            }

            value.Name = name;
            Arr.Set(ref arguments, (argument) => argument.Name == name, value);
        }

        /// <inheritdoc />
        public void SetOption(string name, Mixture value)
        {
            if (!HasOption(name))
            {
                throw new InvalidArgumentException($"The \"{name}\" option does not exist.", nameof(name));
            }

            value = value ?? new Mixture();
            value.Name = name;
            Arr.Set(ref options, (option) => option.Name == name, value);
        }

        /// <inheritdoc />
        public void SetInteractive(bool interactive)
        {
            IsInteractive = interactive;
        }

        /// <inheritdoc />
        public void SetInputStream(Stream stream)
        {
            inputStream = stream;
        }

        /// <inheritdoc />
        public bool HasArgument(string name)
        {
            return Definition.HasArgument(name);
        }

        /// <inheritdoc />
        public bool HasOption(string name)
        {
            return Definition.HasOption(name);
        }

        /// <inheritdoc />
        public virtual bool HasRawOption(string name, bool ignoreAdditional = false)
        {
            return false;
        }

        /// <inheritdoc cref="options"/>
        protected Mixture[] GetOptions()
        {
            return options;
        }

        /// <inheritdoc cref="arguments"/>
        protected Mixture[] GetArguments()
        {
            return arguments;
        }

        /// <summary>
        /// Processes command line arguments.
        /// </summary>
        protected abstract void Parse();
    }
}
