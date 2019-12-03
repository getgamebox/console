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

namespace GameBox.Console
{
    /// <summary>
    /// The environment variables with console.
    /// </summary>
    public static class EnvironmentVariables
    {
        /// <summary>
        /// True or False indicates whether to interact with the user.
        /// </summary>
        public const string ConsoleShellInteractive = "CONSOLE_SHELL_INTERACTIVE";

        /// <summary>
        /// -1 to 3 indicates the level of detail of the output.
        /// </summary>
        /// <remarks>
        /// -1 means silent(quiet) output.
        /// 0 means normal output.
        /// 1 means detail output.
        /// 2 means very detail output.
        /// 3 means debug output.
        /// </remarks>
        public const string ConsoleShellVerbosity = "CONSOLE_SHELL_VERBOSITY";

        /// <summary>
        /// Console screen bufferd width.
        /// </summary>
        public const string ConsoleBufferWidth = "CONSOLE_BUFFER_WIDTH";

        /// <summary>
        /// Console screen bufferd height.
        /// </summary>
        public const string ConsoleBufferHeight = "CONSOLE_BUFFER_HEIGHT";
    }
}
