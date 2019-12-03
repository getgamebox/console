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
    /// <see cref="CommandHelp"/> displays the help for a given command.
    /// </summary>
    public class CommandHelp : Command
    {
        /// <summary>
        /// Command that needs help.
        /// </summary>
        private Command command;

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public static new string Name { get; } = "help";

        /// <inheritdoc cref="command"/>
        /// <param name="command">The command needs help.</param>
        public void SetCommand(Command command)
        {
            this.command = command;
        }

        /// <inheritdoc />
        protected override void Configure()
        {
            SetName(Name)
                .SetDefinition(
                    new InputArgument("command_name", InputArgumentModes.Optional, "The command name.", Name),
                    new InputOption("format", "-f", InputOptionModes.ValueRequired,
                        "The output format (txt, json).", "txt"),
                    new InputOption("raw", null, InputOptionModes.ValueNone, "To output raw command help."))
                .SetDescription("Displays help for a command")
                .SetHelp(
@"The <info>{command.name}</info> command displays help for a given command:
  <info>{environment.executable_file} {command.name} list</info>
You can also output the help in other formats by using the <comment>--format</comment> option:
  <info>{environment.executable_file} {command.name} list --format=json</info>
To display the list of available commands, please use the <info>list</info> command.");
        }

        /// <inheritdoc />
        protected override int Execute(IInput input, IOutput output)
        {
            if (command == null)
            {
                command = Application.Find(input.GetArgument("command_name"));
            }

            var helper = new HelperDescriptor();
            helper.Describe(output, command, new[]
            {
                HelperDescriptor.OptionFormat(input.GetOption("format")),
                HelperDescriptor.OptionRawText(input.GetOption("raw")),
            });

            command = null;
            return 0;
        }
    }
}
