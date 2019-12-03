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

using GameBox.Console.Events;

namespace GameBox.Console
{
    /// <summary>
    /// Represents an application event.
    /// </summary>
    public static class ApplicationEvents
    {
        /// <summary>
        /// An <see cref="ConsoleErrorEventArgs"/> Indicates that the current console program has an error.
        /// </summary>
        public const string ConsoleError = "console-error";

        /// <summary>
        /// An <see cref="ConsoleCommandEventArgs"/> Indicates that the current command to be executed.
        /// </summary>
        public const string ConsoleCommand = "console-command";

        /// <summary>
        /// An <see cref="ConsoleTerminateEventArgs"/> Indicates that the console will terminate.
        /// </summary>
        public const string ConsoleTerminate = "console-terminate";
    }
}
