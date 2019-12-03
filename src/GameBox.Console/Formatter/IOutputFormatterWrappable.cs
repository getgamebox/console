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

namespace GameBox.Console.Formatter
{
    /// <summary>
    /// Formatter interface for console output that supports word wrapping.
    /// </summary>
    public interface IOutputFormatterWrappable : IOutputFormatter
    {
        /// <summary>
        /// Formats a message according to the given styles, wrapping at <paramref name="width"/>.
        /// </summary>
        /// <param name="message">The message to style.</param>
        /// <param name="width">Word wrapping at <paramref name="width"/>, 0 means no wrapping.</param>
        /// <returns>The styled message.</returns>
        string FormatAndWrap(string message, int width = 0);
    }
}
