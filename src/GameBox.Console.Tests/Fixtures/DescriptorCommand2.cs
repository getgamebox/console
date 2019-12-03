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

namespace GameBox.Console.Tests.Fixtures
{
    public class DescriptorCommand2 : Command.Command
    {
        public DescriptorCommand2()
        {
            Initialize();
        }

        protected override void Configure()
        {
            SetName("descriptor/command2");
            SetAlias("alias1", "alias2");
            SetDescription("command 2 description");
            SetHelp("command 2 help");
            AddUsage("-o|--option_name <argument_name>");
            AddUsage("<argument_name>");
            AddArgument("argument_name", Console.Input.InputArgumentModes.Required);
            AddOption("option_name", "o", Console.Input.InputOptionModes.ValueNone);
        }
    }
}
