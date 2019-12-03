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

using GameBox.Console.Exception;
using GameBox.Console.Input;
using GameBox.Console.Output;

namespace GameBox.Console.Events
{
    /// <summary>
    /// Allows to handle exception thrown while running a command.
    /// </summary>
    public class ConsoleErrorEventArgs : ConsoleEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleErrorEventArgs"/> class.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <param name="exception">The exception instance.</param>
        /// <param name="command">The command instance.</param>
        public ConsoleErrorEventArgs(IInput input, IOutput output, System.Exception exception, Command.Command command = null)
            : base(command, input, output)
        {
            SetException(exception);
        }

        /// <summary>
        /// Gets the handle exception.
        /// </summary>
        public System.Exception Exception { get; private set; } = null;

        /// <summary>
        /// Gets the exit code.
        /// </summary>
        public int ExitCode { get; private set; } = ExitCodes.GeneralException;

        /// <summary>
        /// Sets the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void SetException(System.Exception ex)
        {
            if (ex is IException exception)
            {
                ExitCode = exception.ExitCode;
            }

            Exception = ex;
        }

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
