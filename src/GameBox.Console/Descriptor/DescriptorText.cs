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

using GameBox.Console.Input;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameBox.Console.Descriptor
{
    /// <summary>
    /// Text descriptor.
    /// </summary>
    // http://docopt.org/
    public sealed class DescriptorText : AbstractDescriptor
    {
        /// <summary>
        /// The total width option key.
        /// </summary>
        private const string KeyTotalWidth = "total_width";

        /// <inheritdoc />
        protected override void DescribeInputDefinition(InputDefinition definition, params Mixture[] options)
        {
            var totalWidth = CalculateTotalWidthForOptions(definition.GetOptions());
            foreach (var argument in definition.GetArguments())
            {
                totalWidth = Math.Max(totalWidth, Str.Width(argument.Name));
            }

            options = Arr.Merge(options, new[] { TotalWidth(totalWidth) });

            if (definition.GetArguments().Length > 0)
            {
                Write("<comment>Arguments:</comment>", options);
                Write(Environment.NewLine);
                foreach (var argument in definition.GetArguments())
                {
                    DescribeInputArgument(argument, options);
                    Write(Environment.NewLine);
                }
            }

            if (definition.GetArguments().Length > 0 && definition.GetOptions().Length > 0)
            {
                Write(Environment.NewLine);
            }

            if (definition.GetOptions().Length <= 0)
            {
                return;
            }

            var laterOptions = Array.Empty<InputOption>();
            Write("<comment>Options:</comment>", options);
            foreach (var option in definition.GetOptions())
            {
                if (option.GetShortcutString().Length > 1)
                {
                    Arr.Push(ref laterOptions, option);
                    continue;
                }

                Write(Environment.NewLine);
                DescribeInputOption(option, options);
            }

            foreach (var option in laterOptions)
            {
                Write(Environment.NewLine);
                DescribeInputOption(option, options);
            }
        }

        /// <inheritdoc />
        protected override void DescribeInputOption(InputOption inputOption, params Mixture[] options)
        {
            var @default = string.Empty;
            if (inputOption.IsValueAccept && !(inputOption.GetDefault() is null) && (!inputOption.IsArray || inputOption.GetDefault().Length > 0))
            {
                @default = $"<comment> [default: {FormatDefaultValue(inputOption.GetDefault())}]</comment>";
            }

            var value = string.Empty;
            if (inputOption.IsValueAccept)
            {
                value = $"={inputOption.Name.ToUpper()}";

                if (inputOption.IsValueOptional)
                {
                    value = $"[{value}]";
                }
            }

            var totalWidth = options.Get(KeyTotalWidth, CalculateTotalWidthForOptions(inputOption));

            // "-" + shortcut + ", --" + name
            var shorcut = !string.IsNullOrEmpty(inputOption.GetShortcutString())
                ? $"-{inputOption.GetShortcutString()}, "
                : "    ";
            var synopsis = $"{shorcut}--{inputOption.Name}{value}";

            var spacingWidth = totalWidth - Str.Width(synopsis);

            // totalWidth + 4 means: 2 spaces before <info>, 2 spaces after </info>
            // This step ensures that the format alignment after the description
            // of the line break is also correct.
            var description = Regex.Replace(inputOption.Description, @"\s*[\r\n]\s*",
                Environment.NewLine + Str.Pad(totalWidth + 4));
            var multiple = inputOption.IsArray ? "<comment> (multiple values allowed)</comment>" : string.Empty;

            Write($"  <info>{synopsis}</info>  {Str.Pad(spacingWidth)}{description}{@default}{multiple}", options);
        }

        /// <inheritdoc />
        protected override void DescribeInputArgument(InputArgument argument, params Mixture[] options)
        {
            var @default = string.Empty;
            if (!(argument.GetDefault() is null) &&
                (!argument.GetDefault().IsArray || argument.GetDefault().Length > 0))
            {
                @default = $"<comment> [default: {FormatDefaultValue(argument.GetDefault())}]</comment>";
            }

            int totalWidth = options.Get(KeyTotalWidth, Str.Width(argument.Name));
            var spacingWidth = totalWidth - argument.Name.Length;

            // totalWidth + 4 means: 2 spaces before <info>, 2 spaces after </info>
            // This step ensures that the format alignment after the description
            // of the line break is also correct.
            var description = Regex.Replace(argument.Description, @"\s*[\r\n]\s*",
                Environment.NewLine + Str.Pad(totalWidth + 4));

            Write($"  <info>{argument.Name}</info>  {Str.Pad(spacingWidth)}{description}{@default}", options);
        }

        /// <inheritdoc />
        protected override void DescribeCommand(Command.Command command, params Mixture[] options)
        {
            command.GetSynopsis();
            command.GetSynopsis(true);
            command.MergeApplicationDefinition(false);

            var description = command.Description;
            if (!string.IsNullOrEmpty(description))
            {
                var spacingDescription = Regex.Replace(command.Description, @"\s*[\r\n]\s*",
                    Environment.NewLine + Str.Pad(2));
                Write("<comment>Description:</comment>", options);
                Write(Environment.NewLine);
                Write($"  {spacingDescription}", options);
                Write($"{Environment.NewLine}{Environment.NewLine}");
            }

            Write("<comment>Usage:</comment>", options);

            var usages = Arr.Merge(new[] { command.GetSynopsis(true) }, command.GetAliases(), command.GetUsages());
            foreach (var usage in usages)
            {
                Write(Environment.NewLine);
                Write($"  {usage}", options);
            }

            Write(Environment.NewLine);

            var definition = command.GetOriginalDefinition();
            if (definition.GetArguments().Length > 0 || definition.GetOptions().Length > 0)
            {
                Write(Environment.NewLine);
                DescribeInputDefinition(definition, options);
                Write(Environment.NewLine);
            }

            var help = command.Help;
            if (string.IsNullOrEmpty(help) || help == description)
            {
                return;
            }

            Write(Environment.NewLine);
            Write("<comment>Help:</comment>", options);
            Write(Environment.NewLine);
            Write($"  {help.Replace(Environment.NewLine, $"{Environment.NewLine}  ")}", options);
            Write(Environment.NewLine);
        }

        /// <inheritdoc />
        protected override void DescribeApplication(Application application, params Mixture[] options)
        {
            var describedNamespace = options.Get(Namespace);
            var description = new DescriptionApplication(application, describedNamespace);
            var commandsExcludeAliases = description.GetCommandsExcludeAliases();

            if (options.Get(RawText))
            {
                var width = CalculateColumnWidth(commandsExcludeAliases);
                foreach (var command in commandsExcludeAliases)
                {
                    var spacingWidth = width - command.Name.Length;
                    var spacingDescription = Regex.Replace(command.Description, @"\s*[\r\n]\s*",
                        Environment.NewLine + Str.Pad(width));
                    Write($"{command.Name}{Str.Pad(spacingWidth)}{spacingDescription}", options);
                    Write(Environment.NewLine);
                }
            }
            else
            {
                var help = application.GetHelp();
                if (!string.IsNullOrEmpty(help))
                {
                    Write($"{help}{Environment.NewLine}{Environment.NewLine}", options);
                }

                Write($"<comment>Usage:</comment>{Environment.NewLine}", options);
                Write($"  command [options] [arguments]{Environment.NewLine}{Environment.NewLine}", options);

                DescribeInputDefinition(
                    new InputDefinition(Arr.Map(
                        application.GetDefinition().GetOptions(),
                        (option) => (IInputDefinition)option)), options);

                Write($"{Environment.NewLine}{Environment.NewLine}");

                var namespaces = description.GetNamespaces();
                var width = CalculateColumnWidth(commandsExcludeAliases);

                Write(
                    !string.IsNullOrEmpty(describedNamespace)
                    ? $"<comment>Available commands for the {describedNamespace} namespace:</comment>"
                    : "<comment>Available commands:</comment>", options);

                foreach (var @namespace in namespaces)
                {
                    var namespaceCommandsExcludeAliases = Arr.Filter(@namespace.commandNames, (commandName) =>
                    {
                        return Array.Exists(commandsExcludeAliases, (item) => item.Name == commandName);
                    });

                    if (namespaceCommandsExcludeAliases.Length <= 0)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(describedNamespace) &&
                        @namespace.@namespace != DescriptionApplication.GlobalNamespace)
                    {
                        Write(Environment.NewLine);
                        Write($" <comment>{@namespace.@namespace}</comment>", options);
                    }

                    foreach (var commandName in namespaceCommandsExcludeAliases)
                    {
                        Write(Environment.NewLine);
                        var spacingWidth = width - Str.Width(commandName);
                        var commandInstance = description.GetCommand(commandName);
                        var commandAliases = GetCommandAliasesText(commandInstance);

                        var aliseAndDescription = Regex.Replace(commandAliases + commandInstance.Description, @"\s*[\r\n]\s*",
                            Environment.NewLine + Str.Pad(width + 2));

                        Write($"  <info>{commandName}</info>{Str.Pad(spacingWidth)}{aliseAndDescription}", options);
                    }
                }

                Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Sets the total width option.
        /// </summary>
        /// <param name="width">The total width.</param>
        /// <returns>The option.</returns>
        private static Mixture TotalWidth(int width)
        {
            return new Mixture(width) { Name = KeyTotalWidth };
        }

        /// <summary>
        /// Gets the command aliases text.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <returns>The command aliases text.</returns>
        private static string GetCommandAliasesText(Command.Command command)
        {
            var text = string.Empty;
            var aliases = command.GetAliases();
            if (aliases.Length > 0)
            {
                text = $"[{string.Join("|", aliases)}]";
            }

            return text;
        }

        /// <summary>
        /// Calculate the total buffer width for options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The total buffer width.</returns>
        private static int CalculateTotalWidthForOptions(params InputOption[] options)
        {
            var totalWidth = 0;
            foreach (var option in options)
            {
                // "-" + shortcut + ", --" + name
                var nameLength = 1 + Math.Max(Str.Width(option.GetShortcutString()), 1) + 4 +
                                 Str.Width(option.Name);

                if (option.IsValueAccept)
                {
                    var valueLength = 1 + Str.Width(option.Name); // + value
                    valueLength += option.IsValueOptional ? 2 : 0; // [ + ]
                    nameLength += valueLength;
                }

                totalWidth = Math.Max(totalWidth, nameLength);
            }

            return totalWidth;
        }

        /// <summary>
        /// Calculate the command column width.
        /// </summary>
        /// <param name="commands">The command array.</param>
        /// <returns>The column width.</returns>
        private static int CalculateColumnWidth(IEnumerable<Command.Command> commands)
        {
            var width = 0;
            foreach (var command in commands)
            {
                width = Math.Max(Str.Width(command.Name), width);
                foreach (var alias in command.GetAliases())
                {
                    width = Math.Max(Str.Width(alias), width);
                }
            }

            return width > 0 ? width + 2 : 0;
        }

        /// <summary>
        /// Formats input option/argument default value.
        /// </summary>
        /// <param name="value">The default value.</param>
        /// <returns>The formatted string.</returns>
        private string FormatDefaultValue(Mixture value)
        {
            // todo:
            return value;
        }

        /// <summary>
        /// Output the message with options.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="options">The message options.</param>
        private void Write(string message, Mixture[] options)
        {
            if (options.Get(RawText))
            {
                message = Str.StripHtml(message);
            }

            Write(message, !options.Get(RawOutput));
        }
    }
}
