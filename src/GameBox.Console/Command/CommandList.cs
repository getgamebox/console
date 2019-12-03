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

using GameBox.Console.Helper;
using GameBox.Console.Input;
using GameBox.Console.Output;

namespace GameBox.Console.Command
{
    /// <summary>
    /// CommandList displays the list of all available commands for the application.
    /// </summary>
    public class CommandList : Command
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public static new string Name { get; } = "list";

        /// <inheritdoc />
        protected override void Configure()
        {
            SetName(Name)
                .SetDefinition(
                    new InputArgument("namespace", InputArgumentModes.Optional, "The namespace name."),
                    new InputOption("raw", null, InputOptionModes.ValueNone, "To output raw command list."),
                    new InputOption("format", "-f", InputOptionModes.ValueRequired, "The output format (txt, json)", "txt"))
                .SetDescription("Lists commands")
                .SetHelp(
@"The <info>{command.name}</info> command lists all commands:
  <info>{environment.executable_file} {command.name}</info>
You can also display the commands for a specific namespace:
  <info>{environment.executable_file} {command.name} namespace</info>
You can also output the information in other formats by using the <comment>--format</comment> option:
  <info>{environment.executable_file} {command.name} --format=json</info>
It's also possible to get raw list of commands (useful for embedding command runner):
  <info>{environment.executable_file} {command.name} --raw</info>");
        }

        /// <inheritdoc />
        protected override int Execute(IInput input, IOutput output)
        {
            var helper = new HelperDescriptor();
            helper.Describe(output, Application, new[]
            {
                HelperDescriptor.OptionFormat(input.GetOption("format")),
                HelperDescriptor.OptionRawText(input.GetOption("raw")),
                HelperDescriptor.Namespace(input.GetArgument("namespace")),
            });
            return 0;
        }
    }
}
