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
    public class CommandFooSameCaseUppercase : Command.Command
    {
        protected override void Configure()
        {
            SetName("foo/BAR")
                .SetDescription("foo/BAR command");
        }
    }
}
