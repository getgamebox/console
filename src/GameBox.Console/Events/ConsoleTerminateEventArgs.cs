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
    /// Allows to manipulate the exit code of a command after its execution.
    /// </summary>
    public class ConsoleTerminateEventArgs : ConsoleEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTerminateEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instnace.</param>
        /// <param name="exitCode">The exit code.</param>
        public ConsoleTerminateEventArgs(Command.Command command, IInput input, IOutput output, int exitCode)
            : base(command, input, output)
        {
            ExitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code.
        /// </summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// Sets the exception exit code.
        /// </summary>
        /// <param name="code">The exit code.</param>
        public void SetExitCode(int code)
        {
            ExitCode = code;
        }
    }
}
