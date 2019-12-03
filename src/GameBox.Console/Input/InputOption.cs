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
using System.Text;
using System.Text.RegularExpressions;

namespace GameBox.Console.Input
{
    /// <summary>
    /// Represents a command line option.
    /// What is option : console init -v.
    /// Where -v is option.
    /// </summary>
    public sealed class InputOption : IInputDefinition
    {
        /// <summary>
        /// The shortcut for current option.
        /// </summary>
        private readonly string[] shortcut;

        /// <summary>
        /// The shortcut for current option.
        /// </summary>
        private readonly string shortcutString;

        /// <summary>
        /// The option mode.
        /// </summary>
        private readonly InputOptionModes mode;

        /// <summary>
        /// The default value.
        /// </summary>
        private Mixture @default;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOption"/> class.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <param name="shortcut">The shortcuts, can be null, a string of shortcuts delimited by | or an array of shortcuts.</param>
        /// <param name="mode">The option mode.</param>
        /// <param name="description">A description text.</param>
        /// <param name="defaultValue">The default value(must be null for <see cref="InputOptionModes.ValueNone"/>).</param>
        public InputOption(string name, string shortcut = null,
            InputOptionModes mode = InputOptionModes.ValueNone,
            string description = null, Mixture defaultValue = null)
        {
            Guard.Requires<ArgumentNullException>(name != null);

            if (name.IndexOf("--", StringComparison.Ordinal) == 0)
            {
                name = name.Substring(2);
            }

            if (string.IsNullOrEmpty(name.Trim()))
            {
                throw new InvalidArgumentException("An option name cannot be empty.", nameof(name));
            }

            InvalidName(name, shortcut);

            Name = name;
            Description = description ?? string.Empty;
            this.mode = mode;
            this.shortcut = Arr.Map((shortcut ?? string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries), (item) => item.TrimStart('-'));
            shortcutString = string.Join("|", this.shortcut);

            if (IsArray && !IsValueAccept)
            {
                throw new InvalidArgumentException(
                    $"Impossible to have an option mode {InputOptionModes.IsArray} if the option does not accept a value.");
            }

            SetDefault(defaultValue);
        }

        /// <summary>
        /// Gets the option name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the option description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the option is optional value.
        /// </summary>
        public bool IsValueOptional => (InputOptionModes.ValueOptional & mode) == InputOptionModes.ValueOptional;

        /// <summary>
        /// Gets a value indicating whether the option is accept value.
        /// </summary>
        public bool IsValueAccept => IsValueOptional || IsValueRequired;

        /// <summary>
        /// Gets a value indicating whether the option value is required.
        /// </summary>
        public bool IsValueRequired => (InputOptionModes.ValueRequired & mode) == InputOptionModes.ValueRequired;

        /// <summary>
        /// Gets a value indicating whether the option is array.
        /// </summary>
        public bool IsArray => (InputOptionModes.IsArray & mode) == InputOptionModes.IsArray;

        /// <summary>
        /// Check the option's name and shortcut is invalid.
        /// </summary>
        /// <param name="name">The name of option.</param>
        /// <param name="shortcut">The shortcut of option.</param>
        public static void InvalidName(string name, string shortcut)
        {
            Guard.Requires<ArgumentNullException>(name != null);

            if (name.IndexOf("-", StringComparison.Ordinal) == 0)
            {
                throw new InvalidArgumentException("The name of option can not start '-'.", nameof(name));
            }

            if (name.IndexOf("_", StringComparison.Ordinal) == 0)
            {
                throw new InvalidArgumentException("The name of option can not start '_'.", nameof(name));
            }

            var nameRule = new Regex("^[a-zA-Z][a-zA-Z0-9-_]*$");
            if (!nameRule.IsMatch(name))
            {
                throw new InvalidArgumentException("The name of option need character range of [a-z or A-Z or 0-9 or - or _]. ", nameof(name));
            }

            if (string.IsNullOrEmpty(shortcut))
            {
                return;
            }

            var shortcutRule = new Regex("^-?[a-zA-Z0-9|]*$");
            if (!shortcutRule.IsMatch(shortcut))
            {
                throw new InvalidArgumentException("The name of shortcut need character range of [a-z or A-Z or 0-9 or |]. ", nameof(shortcut));
            }
        }

        /// <summary>
        /// Sets the default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public void SetDefault(Mixture defaultValue)
        {
            if ((InputOptionModes.ValueNone & mode) == InputOptionModes.ValueNone
                && !(defaultValue is null))
            {
                throw new ConsoleLogicException(
                    $"Cannot set a default value when using {InputOptionModes.ValueNone} mode.");
            }

            if (!IsValueAccept)
            {
                @default = null;
                return;
            }

            if (IsArray)
            {
                defaultValue = defaultValue ?? new Mixture(Array.Empty<string>());
                if (!defaultValue.IsArray)
                {
                    throw new ConsoleLogicException("A default value for an array option must be an array.");
                }
            }

            @default = defaultValue;
        }

        /// <summary>
        /// Get the option shortcut array.
        /// </summary>
        /// <returns>The option shortcut array.</returns>
        public string[] GetShortcut()
        {
            return shortcut;
        }

        /// <summary>
        /// Get the option shortcut string.
        /// </summary>
        /// <returns>The shortcut string.</returns>
        public string GetShortcutString()
        {
            return shortcutString;
        }

        /// <summary>
        /// Gets the option default value.
        /// </summary>
        /// <returns>null if the option default value not exists.</returns>
        public Mixture GetDefault()
        {
            return @default;
        }

        /// <summary>
        /// Checks whether the given option equals this one.
        /// </summary>
        /// <param name="obj">Value to compare.</param>
        /// <returns>Is equals.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is InputOption option))
            {
                return false;
            }

            return option.Name == Name
                   && option.GetDefault() == GetDefault()
                   && option.IsArray == IsArray
                   && option.IsValueOptional == IsValueOptional
                   && option.IsValueRequired == IsValueRequired
                   && option.shortcutString == shortcutString;
        }

        /// <summary>
        /// Get option hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            var hashString = new StringBuilder();
            hashString.Append(Name)
                .Append(GetDefault() is null ? "null" : GetDefault().ToString())
                .Append(IsArray)
                .Append(IsValueOptional)
                .Append(IsValueRequired)
                .Append(shortcutString);

            return hashString.ToString().GetHashCode();
        }
    }
}
