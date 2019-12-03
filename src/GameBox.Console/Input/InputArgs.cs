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
using System.Linq;
using System.Text.RegularExpressions;

namespace GameBox.Console.Input
{
    /// <summary>
    /// The input comes from the CLI arguments (args).
    /// </summary>
    /// <remarks>
    /// By default, the `<see cref="Environment.GetCommandLineArgs"/>` array is used for the input values.
    /// This can be overridden by explicitly passing the input values in the constructor:
    /// <code>var inputArgs = new InputArgs(args);</code>
    /// </remarks>
    // https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html
    // http://pubs.opengroup.org/onlinepubs/009695399/basedefs/xbd_chap12.html#tag_12_02
    public class InputArgs : AbstractInput
    {
        /// <summary>
        /// The raw args.
        /// </summary>
        private string[] args;

        /// <summary>
        /// The parsed args.
        /// </summary>
        private string[] parsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputArgs"/> class.
        /// </summary>
        /// <param name="args">An array of parameters from the CLI.</param>
        /// <param name="definition">A <see cref="InputDefinition"/> instance.</param>
        public InputArgs(string[] args = null, InputDefinition definition = null)
        {
            SetArgs(args ?? Arr.Slice(Terminal.GetCommandLineArgs(), 1));

            if (definition == null)
            {
                return;
            }

            Bind(definition);
            Validate();
        }

        /// <inheritdoc />
        public override bool HasRawOption(string name, bool ignoreAdditional = false)
        {
            // Only be very familiar with what this function does to be able to use!
            // The method does not implement all the functions of parsing.
            // Be careful!
            foreach (var token in args)
            {
                // #69
                if (ignoreAdditional && token == "--")
                {
                    return false;
                }

                // Options with values:
                // - For long options, test for '--option=' at beginning
                // - For short options, test for '-o' at beginning
                // Did not implement full parsing.
                var leading = token.IndexOf("--", StringComparison.Ordinal) == 0 ? name + "=" : name;
                if (token == name || token == leading || (!string.IsNullOrEmpty(leading) && token.IndexOf(leading, StringComparison.Ordinal) == 0))
                {
                    return true;
                }
            }

            return base.HasRawOption(name, ignoreAdditional);
        }

        /// <inheritdoc />
        public override Mixture GetRawOption(string name, Mixture defaultValue = null, bool ignoreAdditional = false)
        {
            // Only be very familiar with what this function does to be able to use!
            // The method does not implement all the functions of parsing.
            // Be careful!
            var tokens = args;

            while (tokens.Length > 0)
            {
                var token = Arr.Shift(ref tokens);

                if (ignoreAdditional && token == "--")
                {
                    return defaultValue;
                }

                // Options with values:
                //   For long options, test for '--option=' at beginning
                //   For short options, test for '-o' at beginning
                var leading = token.IndexOf("--", StringComparison.Ordinal) == 0 ? name + "=" : name;
                if (!string.IsNullOrEmpty(leading) && token.IndexOf(leading, StringComparison.Ordinal) == 0)
                {
                    return token.Substring(leading.Length);
                }
            }

            return base.GetRawOption(name, defaultValue, ignoreAdditional);
        }

        /// <inheritdoc />
        public override Mixture GetFirstArgument()
        {
            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(arg) && arg[0] == '-')
                {
                    continue;
                }

                return arg;
            }

            return null;
        }

        /// <summary>
        /// Returns a stringified representation of the args passed to the command.
        /// </summary>
        /// <returns>The args passed to the command.</returns>
        public override string ToString()
        {
            var tokens = Arr.Map(args, (token) =>
            {
                var match = Regex.Match(token, "^(-[^=]+=)(.+)");
                if (match.Success)
                {
                    return match.Groups[1].Value + EscapesToken(match.Groups[2].Value);
                }

                if (token.Length > 0 && token[0] != '-')
                {
                    return EscapesToken(token);
                }

                return token;
            });
            return string.Join(Str.Space, tokens);
        }

        /// <summary>
        /// Sets the raw args.
        /// </summary>
        /// <param name="args">The raw args.</param>
        protected void SetArgs(IEnumerable<string> args)
        {
            this.args = args.ToArray();
        }

        /// <inheritdoc />
        protected override void Parse()
        {
            parsed = args;
            var parseOptions = true;

            while (parsed.Length > 0)
            {
                var token = Arr.Shift(ref parsed) ?? string.Empty;
                if (parseOptions && string.IsNullOrEmpty(token))
                {
                    ParseArgument(token);
                }
                else if (parseOptions && token == "--")
                {
                    // Subsequent content will be passed to the script.
                    // Use the Argument array to receive the following data.
                    parseOptions = false;
                }
                else if (parseOptions && token.IndexOf("--", StringComparison.Ordinal) == 0)
                {
                    ParseLongOption(token);
                }
                else if (parseOptions && token[0] == '-' && token != "-")
                {
                    ParseShortOption(token);
                }
                else
                {
                    ParseArgument(token);
                }
            }
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <param name="token">The current token.</param>
        private void ParseArgument(string token)
        {
            var index = GetArguments().Length;

            // if input is expecting argument, add it
            var argument = Definition.GetArgument(index);
            if (argument != null)
            {
                if (argument.IsArray)
                {
                    SetArgument(argument.Name, new[] { token });
                    return;
                }

                SetArgument(argument.Name, token);
                return;
            }

            // if last argument is array, append token to last argument
            argument = Definition.GetArgument(index - 1);
            if (argument != null && argument.IsArray)
            {
                string[] lastToken = GetArgument(argument.Name);
                Arr.Push(ref lastToken, token);
                SetArgument(argument.Name, lastToken);
                return;
            }

            var allArguments = Definition.GetArguments();
            if (allArguments.Length > 0)
            {
                throw new ParseException(
                    $"Too many arguments, expected arguments {string.Join(Str.Space, Arr.Map(allArguments, (arg) => arg.Name))}.");
            }

            throw new ParseException($"No arguments expected, got \"{token}\".");
        }

        /// <summary>
        /// Parses a long option.
        /// </summary>
        /// <param name="token">The current token.</param>
        private void ParseLongOption(string token)
        {
            var name = token.Substring(2);
            var assignmentPosition = name.IndexOf('=');
            if (assignmentPosition >= 0)
            {
                var value = name.Substring(assignmentPosition + 1);
                if (value.Length <= 0)
                {
                    Arr.Unshift(ref parsed, value);
                }

                AddLongOption(name.Substring(0, assignmentPosition), value);
            }
            else
            {
                AddLongOption(name, null);
            }
        }

        /// <summary>
        /// Adds a long option value.
        /// </summary>
        /// <param name="name">The long option name.</param>
        /// <param name="value">The long option value.</param>
        private void AddLongOption(string name, string value)
        {
            var option = Definition.GetOption(name);
            if (option == null)
            {
                throw new ParseException($"The \"--{name}\" option does not exist.");
            }

            if (value != null && !option.IsValueAccept)
            {
                throw new ParseException($"The \"--{name}\" option does not accept a value.");
            }

            if (string.IsNullOrEmpty(value)
                && option.IsValueAccept && parsed.Length > 0)
            {
                var next = parsed[0];
                if (string.IsNullOrEmpty(next) || (next.Length > 0 && next[0] != '-'))
                {
                    value = Arr.Shift(ref parsed);
                }
            }

            if (value == null)
            {
                if (option.IsValueRequired)
                {
                    throw new ParseException($"The \"--{name}\" option requires a value.");
                }

                if (!option.IsArray && !option.IsValueOptional)
                {
                    value = bool.TrueString;
                }
            }

            if (!option.IsArray)
            {
                SetOption(name, value);
                return;
            }

            string[] lastOption = Array.Find(GetOptions(), opt => opt.Name == name);

            if (lastOption == null)
            {
                SetOption(name, new[] { value });
            }
            else
            {
                Arr.Push(ref lastOption, value);
                SetOption(name, lastOption);
            }
        }

        /// <summary>
        /// Parses a short option.
        /// </summary>
        /// <param name="token">The current token.</param>
        private void ParseShortOption(string token)
        {
            var name = token.Substring(1);
            if (name.Length > 1)
            {
                if (Definition.HasShortcut(name[0]) && Definition.GetOptionForShortcut(name[0]).IsValueAccept)
                {
                    AddShortOption(name[0], name.Substring(1));
                }
                else
                {
                    ParseShortOptionSet(name);
                }
            }
            else
            {
                AddShortOption(name, null);
            }
        }

        /// <summary>
        /// Adds a short option value.
        /// </summary>
        /// <param name="shortcut">The short option key.</param>
        /// <param name="value">The short option value.</param>
        private void AddShortOption(char shortcut, string value)
        {
            AddShortOption(shortcut.ToString(), value);
        }

        /// <summary>
        /// Adds a short option value.
        /// </summary>
        /// <param name="shortcut">The short option key.</param>
        /// <param name="value">The short option value.</param>
        private void AddShortOption(string shortcut, string value)
        {
            if (!Definition.HasShortcut(shortcut))
            {
                throw new ParseException($"The \"-{shortcut}\" option does not exist.");
            }

            AddLongOption(Definition.GetOptionForShortcut(shortcut).Name, value);
        }

        /// <summary>
        /// Parses a short option set.
        /// </summary>
        /// <param name="name">The short option name.</param>
        private void ParseShortOptionSet(string name)
        {
            for (var index = 0; index < name.Length; index++)
            {
                if (!Definition.HasShortcut(name[index]))
                {
                    throw new ParseException($"The \"-{name[index]}\" option does not exist.");
                }

                var option = Definition.GetOptionForShortcut(name[index]);
                if (option.IsValueAccept)
                {
                    AddLongOption(option.Name, index == name.Length - 1 ? null : name.Substring(index + 1));
                    break;
                }

                AddLongOption(option.Name, null);
            }
        }
    }
}
