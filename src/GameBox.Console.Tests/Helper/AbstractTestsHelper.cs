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
using Moq;
using System.IO;
using System.Text;

namespace GameBox.Console.Tests.Helper
{
#pragma warning disable S1118
    public abstract class AbstractTestsHelper
#pragma warning restore S1118
    {
        protected static IInput CreateInput(string interactiveInput, bool interactive = true)
        {
            var input = new Mock<IInputStreamable>();
            if (!string.IsNullOrEmpty(interactiveInput))
            {
                interactiveInput = interactiveInput.Replace("\n", System.Environment.NewLine.ToString(), System.StringComparison.CurrentCulture);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(interactiveInput));
                input.Setup((foo) => foo.GetInputStream()).Returns(() => stream);
            }

            input.Setup((foo) => foo.IsInteractive).Returns(() => interactive);
            input.Setup((foo) => foo.Encoding).Returns(() => Encoding.UTF8);
            return input.Object;
        }

        protected static OutputStringBuilder CreateOutputStringBuilder()
        {
            return new OutputStringBuilder();
        }
    }
}
