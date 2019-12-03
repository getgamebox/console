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
using GameBox.Console.Util;
using System;
using System.Text.RegularExpressions;

namespace GameBox.Console.Input
{
    /// <summary>
    /// Represents a command line argument.
    /// What is argument : console install GameBox.Core.
    /// Where GameBox.Core is argument.
    /// </summary>
    public sealed class InputArgument : IInputDefinition
    {
        /// <summary>
        /// The argument mode.
        /// </summary>
        private readonly InputArgumentModes mode;

        /// <summary>
        /// The default value(<see cref="InputArgumentModes.Optional"/> mode only).
        /// </summary>
        private Mixture @default;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputArgument"/> class.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="mode">The argument mode.</param>
        /// <param name="description">A description text.</param>
        /// <param name="defaultValue">The default value(<see cref="InputArgumentModes.Optional"/> mode only).</param>
        public InputArgument(string name, InputArgumentModes mode = InputArgumentModes.Optional,
            string description = null, Mixture defaultValue = null)
        {
            Guard.Requires<ArgumentNullException>(name != null);

            InvalidName(name);

            Name = name;
            Description = description ?? string.Empty;
            this.mode = mode;
            SetDefault(defaultValue);
        }

        /// <summary>
        /// Gets the argument name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a description text.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the argument is required.
        /// </summary>
        public bool IsRequired => (InputArgumentModes.Required & mode) == InputArgumentModes.Required;

        /// <summary>
        /// Gets a value indicating whether the argument is array.
        /// </summary>
        public bool IsArray => (InputArgumentModes.IsArray & mode) == InputArgumentModes.IsArray;

        /// <summary>
        /// Convert <see cref="InputArgument"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="content">The source content.</param>
        public static implicit operator string(InputArgument content)
        {
            return content.ToString();
        }

        /// <summary>
        /// Check the argument's name and shortcut is invalid.
        /// </summary>
        /// <param name="name">The name of option.</param>
        public static void InvalidName(string name)
        {
            Guard.Requires<ArgumentNullException>(name != null);

            if (name.IndexOf("-", StringComparison.Ordinal) == 0)
            {
                throw new InvalidArgumentException("The name of argument can not start '-'.", nameof(name));
            }

            if (name.IndexOf("_", StringComparison.Ordinal) == 0)
            {
                throw new InvalidArgumentException("The name of argument can not start '_'.", nameof(name));
            }

            var nameRule = new Regex("^[a-zA-Z][a-zA-Z0-9-_]*$");
            if (!nameRule.IsMatch(name))
            {
                throw new InvalidArgumentException("The name of argument need character range of [a-z or A-Z or 0-9 or - or _]. ", nameof(name));
            }
        }

        /// <summary>
        /// Gets the argument default value.
        /// </summary>
        /// <returns>null if the argument default value not exists.</returns>
        public Mixture GetDefault()
        {
            return @default;
        }

        /// <summary>
        /// Get the argument name.
        /// </summary>
        /// <returns>The argument name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Sets the default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public void SetDefault(Mixture defaultValue)
        {
            if (IsRequired && !(defaultValue is null))
            {
                throw new ConsoleLogicException(
                    $"Cannot set a default value except for {InputArgumentModes.Optional} mode.");
            }

            if (IsArray)
            {
                defaultValue = defaultValue ?? new Mixture(Array.Empty<string>());
                if (!defaultValue.IsArray)
                {
                    throw new ConsoleLogicException("A default value for an array argument must be an array.");
                }
            }
            else
            {
                if (!(defaultValue is null) && defaultValue.IsArray)
                {
                    throw new ConsoleLogicException("The Argument mode is not array but default value is array.");
                }
            }

            this.@default = defaultValue;
        }
    }
}
