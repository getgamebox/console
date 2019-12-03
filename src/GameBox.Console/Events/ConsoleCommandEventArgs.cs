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

namespace GameBox.Console.Events
{
    /// <summary>
    /// Allows to do things before the command is executed, like skipping the command or changing the input.
    /// </summary>
    public class ConsoleCommandEventArgs : ConsoleEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        public ConsoleCommandEventArgs(Command.Command command, IInput input, IOutput output)
            : base(command, input, output)
        {
        }

        /// <summary>
        /// Gets a value indicating whether true if skipped commands.
        /// </summary>
        public bool SkipCommand { get; private set; } = false;

        /// <summary>
        /// Sets whether skip execute command.
        /// </summary>
        /// <param name="skipped">Whether is skip execute the command.</param>
        public void SetSkipCommand(bool skipped)
        {
            SkipCommand = skipped;
        }
    }
}
