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

using System.Collections.Generic;

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="IOutput"/> extension function.
    /// </summary>
    public static class ExtendOutput
    {
        /// <summary>
        /// Writes an message array to the output and adds a newline at the end.
        /// </summary>
        /// <param name="output">The <see cref="IOutput"/> instance.</param>
        /// <param name="messages">The message as an iterable of strings or a single string.</param>
        /// <param name="options">A bitmask of options. <see cref="OutputOptions.None"/> is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.</param>
        public static void WriteLine(this IOutput output, IEnumerable<string> messages, OutputOptions options = OutputOptions.None)
        {
            foreach (var message in messages)
            {
                output.WriteLine(message, options);
            }
        }
    }
}
