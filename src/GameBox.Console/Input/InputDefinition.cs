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
using System.Collections.Generic;
using System.Text;

namespace GameBox.Console.Input
{
    /// <summary>
    /// A <see cref="InputDefinition"/> represents a set of valid command line arguments and options.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// var definition = new InputDefinition(
    ///     new InputArgument("namespace", InputArgumentModes.Optional, "The namespace name."),
    ///     new InputOption("raw", null, InputOptionModes.ValueOptional, "To output raw command list.")
    /// );
    /// </code>
    /// </remarks>
    public sealed class InputDefinition : IInputDefinition
    {
        /// <summary>
        /// The options shortcuts mapping. key is shortcut ,value is option name.
        /// </summary>
        private readonly Dictionary<string, string> shortcuts;

        /// <summary>
        /// The array of InputArgument objects.
        /// </summary>
        private InputArgument[] arguments;

        /// <summary>
        /// The array of InputOption objects.
        /// </summary>
        private InputOption[] options;

        /// <summary>
        /// How many modes of the <see cref="InputArgument"/> are required(<see cref="InputArgumentModes.Required"/>).
        /// </summary>
        private int requiredCount;

        /// <summary>
        /// The <see cref="InputArgument"/> has optional.
        /// </summary>
        private bool hasOptional;

        /// <summary>
        /// One of the <see cref="InputArgument"/> has array.
        /// </summary>
        private bool hasAnArrayArgument;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDefinition"/> class.
        /// </summary>
        /// <param name="definition">An array of <see cref="InputArgument"/> and <see cref="InputOption"/> instance.</param>
        public InputDefinition(params IInputDefinition[] definition)
        {
            shortcuts = new Dictionary<string, string>();
            SetDefinition(definition);
        }

        /// <summary>
        /// Sets the command line definition of the input.
        /// </summary>
        /// <param name="definition">command line definition.</param>
        public void SetDefinition(params IInputDefinition[] definition)
        {
            var inputArguments = Array.Empty<InputArgument>();
            var inputOptions = Array.Empty<InputOption>();

            foreach (var item in definition)
            {
                if (item is InputArgument inputArgument)
                {
                    Arr.Push(ref inputArguments, inputArgument);
                    continue;
                }

                if (item is InputOption inputOption)
                {
                    Arr.Push(ref inputOptions, inputOption);
                    continue;
                }

                throw new ConsoleException(
                    $"Invalid definition, only allowed: {nameof(InputArgument)} and {nameof(InputOption)}");
            }

            SetArguments(inputArguments);
            SetOptions(inputOptions);
        }

        /// <summary>
        /// Sets an array of InputOption objects.
        /// </summary>
        /// <param name="options">An array of InputOption objects.</param>
        public void SetOptions(params InputOption[] options)
        {
            this.options = Array.Empty<InputOption>();
            shortcuts.Clear();
            AddOptions(options);
        }

        /// <summary>
        /// Adds an array of <see cref="InputOption"/> objects.
        /// </summary>
        /// <param name="options">An array of <see cref="InputOption"/> objects.</param>
        public void AddOptions(params InputOption[] options)
        {
            if (options == null)
            {
                return;
            }

            foreach (var option in options)
            {
                AddOption(option);
            }
        }

        /// <summary>
        /// Gets the array of InputOption objects.
        /// </summary>
        /// <returns>An array of InputOption objects.</returns>
        public InputOption[] GetOptions()
        {
            return options;
        }

        /// <summary>
        /// Gets the <see cref="InputOption"/> object by name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The <see cref="InputOption"/> object.</returns>
        public InputOption GetOption(string name)
        {
            return Array.Find(options, (option) => option.Name == name);
        }

        /// <summary>
        /// Returns true if an <see cref="InputOption"/> object exists by name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>true if the <see cref="InputOption"/> object exists, false otherwise.</returns>
        public bool HasOption(string name)
        {
            return Array.Exists(options, option => option.Name == name);
        }

        /// <summary>
        /// Sets an array of InputArgument objects.
        /// </summary>
        /// <param name="arguments">An array of InputArgument objects.</param>
        public void SetArguments(params InputArgument[] arguments)
        {
            Guard.Requires<ArgumentNullException>(arguments != null);
            this.arguments = Array.Empty<InputArgument>();
            requiredCount = 0;
            hasOptional = false;
            hasAnArrayArgument = false;

            AddArguments(arguments);
        }

        /// <summary>
        /// Adds an array of <see cref="InputArgument"/> objects.
        /// </summary>
        /// <param name="arguments">An array of InputArgument objects.</param>
        public void AddArguments(params InputArgument[] arguments)
        {
            if (arguments == null)
            {
                return;
            }

            foreach (var argument in arguments)
            {
                AddArgument(argument);
            }
        }

        /// <summary>
        /// Gets the array of <see cref="InputArgument"/> objects.
        /// </summary>
        /// <returns>An array of <see cref="InputArgument"/> objects.</returns>
        public InputArgument[] GetArguments()
        {
            return arguments;
        }

        /// <summary>
        /// Gets the <see cref="InputArgument"/> object by name.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <returns>The <see cref="InputArgument"/> object.</returns>
        public InputArgument GetArgument(string name)
        {
            return Array.Find(arguments, (argument) => argument.Name == name);
        }

        /// <summary>
        /// Gets the <see cref="InputArgument"/> object by index.
        /// </summary>
        /// <param name="index">The index of arguments.</param>
        /// <returns>The <see cref="InputArgument"/> object.</returns>
        public InputArgument GetArgument(int index)
        {
            return index >= 0 && index < arguments.Length ? arguments[index] : null;
        }

        /// <summary>
        /// Returns the number of required <see cref="InputArgument"/>s.
        /// </summary>
        /// <returns> The number of required <see cref="InputArgument"/>s.</returns>
        public int GetArgumentRequiredCount()
        {
            return requiredCount;
        }

        /// <summary>
        /// Returns true if an <see cref="InputArgument"/> object exists by name.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <returns>true if the <see cref="InputArgument"/> object exists, false otherwise.</returns>
        public bool HasArgument(string name)
        {
            return Array.Exists(arguments, (argument) => argument.Name == name);
        }

        /// <summary>
        /// Returns true if an <see cref="InputArgument"/> object exists by index.
        /// </summary>
        /// <param name="index">The argument index.</param>
        /// <returns>true if the <see cref="InputArgument"/> object exists, false otherwise.</returns>
        public bool HasArgument(int index = 0)
        {
            return index < arguments.Length;
        }

        /// <summary>
        /// Gets the array of the <see cref="InputArgument"/> object default value.
        /// </summary>
        /// <returns>The array if <see cref="InputArgument"/> default value.</returns>
        public Mixture[] GetArgumentDefaults()
        {
            return Arr.Map(arguments, argument => argument.GetDefault());
        }

        /// <summary>
        /// Gets the array of the <see cref="InputOption"/> object default value.
        /// </summary>
        /// <returns>The array if <see cref="InputOption"/> default value.</returns>
        public Mixture[] GetOptionsDefaults()
        {
            return Arr.Map(options, option => option.GetDefault());
        }

        /// <summary>
        /// Returns true if an <see cref="InputOption"/> object exists by shortcut.
        /// </summary>
        /// <param name="name">The <see cref="InputOption"/> shortcut.</param>
        /// <returns>true if has shortcut.</returns>
        public bool HasShortcut(string name)
        {
            return shortcuts.ContainsKey(name);
        }

        /// <summary>
        /// Returns true if an <see cref="InputOption"/> object exists by shortcut.
        /// </summary>
        /// <param name="name">The <see cref="InputOption"/> shortcut.</param>
        /// <returns>true if has shortcut.</returns>
        public bool HasShortcut(char name)
        {
            return HasShortcut(name.ToString());
        }

        /// <summary>
        /// Gets an <see cref="InputOption"/> by shortcut.
        /// </summary>
        /// <param name="name">The <see cref="InputOption"/> shortcut.</param>
        /// <returns>An <see cref="InputOption"/> object.</returns>
        public InputOption GetOptionForShortcut(string name)
        {
            return GetOption(ShortcutToOptionName(name));
        }

        /// <summary>
        /// Gets an <see cref="InputOption"/> by shortcut.
        /// </summary>
        /// <param name="name">The <see cref="InputOption"/> shortcut.</param>
        /// <returns>An <see cref="InputOption"/> object.</returns>
        public InputOption GetOptionForShortcut(char name)
        {
            return GetOptionForShortcut(name.ToString());
        }

        /// <summary>
        /// Returns the synopsis for the command line arguments and options definition.
        /// </summary>
        /// <param name="isShort">Whether to show the short version of the synopsis (without options) or not.</param>
        /// <returns>The synopsis for the command.</returns>
        public string GetSynopsis(bool isShort)
        {
            var synopsis = new StringBuilder();

            // Generate is : [options] [--] [<packages>]...

            // Generate the option synopsis.
            if (isShort && GetOptions().Length > 0)
            {
                synopsis.Append($"[{nameof(options)}]").Append(Str.Space);
            }
            else if (!isShort)
            {
                foreach (var option in GetOptions())
                {
                    var value = string.Empty;
                    if (option.IsValueAccept)
                    {
                        value = $" {(option.IsValueOptional ? "[" : string.Empty)}" +
                                $"{option.Name.ToUpper()}" +
                                $"{(option.IsValueOptional ? "]" : string.Empty)}";
                    }

                    var shortcut = string.Join("|", Arr.Map(option.GetShortcut(), (item) => "-" + item));
                    synopsis.Append($"[{shortcut}--{option.Name}{value}]").Append(Str.Space);
                }
            }

            // Generate the argument synopsis.
            if (GetArguments().Length > 0)
            {
                synopsis.Append("[--]").Append(Str.Space);
            }

            foreach (var argument in GetArguments())
            {
                if (!argument.IsRequired)
                {
                    synopsis.Append("[");
                }

                synopsis.Append($"<{argument.Name}>");

                if (!argument.IsRequired)
                {
                    synopsis.Append("]");
                }

                if (argument.IsArray)
                {
                    synopsis.Append("...");
                }

                synopsis.Append(Str.Space);
            }

            return synopsis.ToString().TrimEnd();
        }

        /// <summary>
        /// Adds an <see cref="InputOption"/> object.
        /// </summary>
        /// <param name="option">The <see cref="InputOption"/> object.</param>
        private void AddOption(InputOption option)
        {
            var olderOption = Array.Find(options, (item) => item.Name == option.Name);
            if (olderOption != null && olderOption.Equals(option))
            {
                throw new ConsoleLogicException($"An option named \"{option.Name}\" already exists.");
            }

            // if shortcut exists then we check the options whether consistent.
            foreach (var shortcut in option.GetShortcut())
            {
                if (!shortcuts.TryGetValue(shortcut, out string olderOptionName))
                {
                    continue;
                }

                olderOption = Array.Find(options, (item) => item.Name == olderOptionName);
                if (olderOption != null && !olderOption.Equals(option))
                {
                    throw new ConsoleLogicException($"An option with shortcut \"{shortcut}\" already exists.");
                }
            }

            // overwrite existing shortcuts.
            Arr.Push(ref options, option);
            foreach (var shortcut in option.GetShortcut())
            {
                shortcuts[shortcut] = option.Name;
            }
        }

        /// <summary>
        /// Adds a <see cref="InputArgument"/> object.
        /// </summary>
        /// <param name="argument">The <see cref="InputArgument"/> object.</param>
        private void AddArgument(InputArgument argument)
        {
            if (Array.Exists(arguments, (arg) => arg.Name == argument.Name))
            {
                throw new ConsoleLogicException($"An argument with name \"{argument.Name}\" already exists.");
            }

            if (hasAnArrayArgument)
            {
                throw new ConsoleLogicException("Cannot add an argument after an array argument.");
            }

            if (argument.IsRequired && hasOptional)
            {
                throw new ConsoleLogicException("Cannot add a required argument after an optional one.");
            }

            if (argument.IsArray)
            {
                hasAnArrayArgument = true;
            }

            if (argument.IsRequired)
            {
                ++requiredCount;
            }
            else
            {
                hasOptional = true;
            }

            Arr.Push(ref arguments, argument);
        }

        /// <summary>
        /// Returns the <see cref="InputOption"/> name given a shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        /// <returns>The <see cref="InputOption"/> name.</returns>
        private string ShortcutToOptionName(string shortcut)
        {
            if (!shortcuts.TryGetValue(shortcut, out string optionName))
            {
                throw new InvalidArgumentException($"The \"{shortcut}\" option does not exist.");
            }

            return optionName;
        }
    }
}
