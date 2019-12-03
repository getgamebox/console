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
using GameBox.Console.Vendor.LitJson;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameBox.Console.Descriptor
{
    /// <summary>
    /// Json descriptor.
    /// </summary>
    public sealed class DescriptorJson : AbstractDescriptor
    {
        /// <inheritdoc />
        protected override void DescribeInputDefinition(InputDefinition definition, params Mixture[] options)
        {
            WriteData(GetInputDefinitionData(definition));
        }

        /// <inheritdoc />
        protected override void DescribeInputOption(InputOption inputOption, params Mixture[] options)
        {
            WriteData(GetInputOptionData(inputOption));
        }

        /// <inheritdoc />
        protected override void DescribeInputArgument(InputArgument argument, params Mixture[] options)
        {
            WriteData(GetInputArgumentData(argument));
        }

        /// <inheritdoc />
        protected override void DescribeCommand(Command.Command command, params Mixture[] options)
        {
            WriteData(GetCommandData(command));
        }

        /// <inheritdoc />
        protected override void DescribeApplication(Application application, params Mixture[] options)
        {
            var data = new ApplicationData();
            var describedNamespace = options.Get(Namespace);
            var description = new DescriptionApplication(application, describedNamespace, true);
            var commands = new List<CommandData>();

            foreach (var command in description.GetCommandsExcludeAliases())
            {
                commands.Add(GetCommandData(command));
            }

            if (application.Name != Application.Unknow)
            {
                data.Name = application.Name;
                if (application.Version != Application.Unknow)
                {
                    data.Version = application.Version;
                }
            }

            data.Commands = commands.ToArray();
            data.Namespace = describedNamespace;
            var namspaces = description.GetNamespaces();
            data.Namespaces = namspaces.Length <= 0 ? null : Arr.Map(namspaces, (item) => item.@namespace);

            WriteData(data);
        }

        /// <summary>
        /// Gets the input argument serialize class.
        /// </summary>
        /// <param name="argument">The argument instance.</param>
        /// <returns>The serialize class.</returns>
        private static InputArgumentData GetInputArgumentData(InputArgument argument)
        {
            return new InputArgumentData
            {
                Name = argument.Name,
                IsRequired = argument.IsRequired,
                IsArray = argument.IsArray,
                Description = Regex.Replace(argument.Description, "\\s*[\r\n]\\s*", Str.Space),
                Default = argument.GetDefault()?.ToString(),
            };
        }

        /// <summary>
        /// Gets the input option serialize class.
        /// </summary>
        /// <param name="option">The option instance.</param>
        /// <returns>The serialize class.</returns>
        private static InputOptionData GetInputOptionData(InputOption option)
        {
            return new InputOptionData
            {
                Name = "--" + option.Name,
                Shortcut = !string.IsNullOrEmpty(option.GetShortcutString())
                    ? "-" + option.GetShortcutString().Replace("|", "|-") : null,
                IsValueAccept = option.IsValueAccept,
                IsValueRequired = option.IsValueRequired,
                IsArray = option.IsArray,
                Description = Regex.Replace(option.Description, "\\s*[\r\n]\\s*", Str.Space),
                Default = option.GetDefault()?.ToString(),
            };
        }

        /// <summary>
        /// Gets the intput definition serialize class.
        /// </summary>
        /// <param name="definition">The input definition.</param>
        /// <returns>The serialize class.</returns>
        private static InputDefinitionData GetInputDefinitionData(InputDefinition definition)
        {
            var arguments = new List<InputArgumentData>(definition.GetArguments().Length);
            foreach (var argument in definition.GetArguments())
            {
                arguments.Add(GetInputArgumentData(argument));
            }

            var options = new List<InputOptionData>(definition.GetOptions().Length);
            foreach (var option in definition.GetOptions())
            {
                options.Add(GetInputOptionData(option));
            }

            return new InputDefinitionData
            {
                Arguments = arguments.ToArray(),
                Options = options.ToArray(),
            };
        }

        /// <summary>
        /// Gets the command serialize class.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <returns>The command serialize class.</returns>
        private static CommandData GetCommandData(Command.Command command)
        {
            command.GetSynopsis();
            command.MergeApplicationDefinition(false);

            return new CommandData
            {
                Name = command.Name,
                Usage = Arr.Merge(new[] { command.GetSynopsis() }, command.GetUsages(), command.GetAliases()),
                Description = command.Description,
                Help = command.Help,
                Definition = GetInputDefinitionData(command.GetDefinition()),
                IsHidden = command.IsHidden,
            };
        }

        /// <summary>
        /// Write the serialize class in output.
        /// </summary>
        /// <param name="data">The serialize class.</param>
        private void WriteData(object data)
        {
            var jsonWriter = new JsonWriter
            {
                IndentValue = 4,
                LowerCaseProperties = true,
                PrettyPrint = true,
            };
            JsonMapper.ToJson(data, jsonWriter);
            Write(jsonWriter.ToString());
        }

        /// <summary>
        /// <see cref="InputArgument"/> serialize class.
        /// </summary>
        private class InputArgumentData
        {
            /// <summary>
            /// Gets or sets the argument name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the argument is required.
            /// </summary>
            public bool IsRequired { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the argument is array.
            /// </summary>
            public bool IsArray { get; set; }

            /// <summary>
            /// Gets or sets the argument description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the argument default value.
            /// </summary>
            public string Default { get; set; }
        }

        /// <summary>
        /// <see cref="InputOption" /> serialize class.
        /// </summary>
        private class InputOptionData
        {
            /// <summary>
            /// Gets or sets the option name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the option shortcut.
            /// </summary>
            public string Shortcut { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the value is accept.
            /// </summary>
            public bool IsValueAccept { get; set; }

            /// <summary>
            ///  Gets or sets a value indicating whether the value is required.
            /// </summary>
            public bool IsValueRequired { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the option is array.
            /// </summary>
            public bool IsArray { get; set; }

            /// <summary>
            /// Gets or sets the option description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the argument default value.
            /// </summary>
            public string Default { get; set; }
        }

        /// <summary>
        /// <see cref="InputDefinition" /> serialize class.
        /// </summary>
        private class InputDefinitionData
        {
            /// <summary>
            /// Gets or sets an array of the arguments.
            /// </summary>
            public InputArgumentData[] Arguments { get; set; }

            /// <summary>
            /// Gets or sets an array of the options.
            /// </summary>
            public InputOptionData[] Options { get; set; }
        }

        /// <summary>
        /// <see cref="Command.Command" /> serialize class.
        /// </summary>
        private class CommandData
        {
            /// <summary>
            /// Gets or sets the command name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the command usage.
            /// </summary>
            public string[] Usage { get; set; }

            /// <summary>
            /// Gets or sets the command processed help.
            /// </summary>
            public string Help { get; set; }

            /// <summary>
            /// Gets or sets the command description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the command definition.
            /// </summary>
            public InputDefinitionData Definition { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the command is hidden.
            /// </summary>
            public bool IsHidden { get; set; }
        }

        /// <summary>
        /// <see cref="Application"/> serialize class.
        /// </summary>
        private class ApplicationData
        {
            /// <summary>
            /// Gets or sets the application name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the application version.
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// Gets or sets the application commands.
            /// </summary>
            public CommandData[] Commands { get; set; }

            /// <summary>
            /// Gets or sets the application current namespaces.
            /// </summary>
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets the application sub namespaces(including itself).
            /// </summary>
            public string[] Namespaces { get; set; }
        }
    }
}
