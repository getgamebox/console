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
    public class DescriptorCommand1 : Command.Command
    {
        public DescriptorCommand1()
        {
            Initialize();
        }

        protected override void Configure()
        {
            SetName("descriptor/command1");
            SetAlias("alias1", "alias2");
            SetDescription("command 1 description");
            SetHelp("command 1 help");
        }
    }
}
