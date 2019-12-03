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
using System;

namespace GameBox.Console.Events
{
    /// <summary>
    /// Allows to inspect input and output of a command.
    /// </summary>
    public class ConsoleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command to trigger an event.</param>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        public ConsoleEventArgs(Command.Command command, IInput input, IOutput output)
        {
            Command = command;
            Input = input;
            Output = output;
        }

        /// <summary>
        /// Gets the command to trigger an event.
        /// </summary>
        public Command.Command Command { get; }

        /// <summary>
        /// Gets the std input instance.
        /// </summary>
        public IInput Input { get; }

        /// <summary>
        /// Gets the std output instance.
        /// </summary>
        public IOutput Output { get; }
    }
}
