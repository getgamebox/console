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
using GameBox.Console.Output;

namespace GameBox.Console.Tests.Fixtures
{
    public class CommandFooWithoutAlias : Command.Command
    {
        public static new string Name { get; } = $"foo";

        public IInput Input { get; private set; }

        public IOutput Output { get; private set; }

        protected override void Configure()
        {
            SetName(Name)
                .SetDescription($"The {Name} command");
        }

        protected override void Interact(IInput input, IOutput output)
        {
            output.WriteLine($"{nameof(Interact)} called");
        }

        protected override int Execute(IInput input, IOutput output)
        {
            Input = input;
            Output = output;

            output.WriteLine($"{nameof(Execute)} called");
            return 0;
        }
    }
}
