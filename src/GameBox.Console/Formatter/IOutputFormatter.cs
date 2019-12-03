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

using System;

namespace GameBox.Console.Formatter
{
    /// <summary>
    /// Formatter interface for console output.
    /// </summary>
    public interface IOutputFormatter : ICloneable
    {
        /// <summary>
        /// Gets or sets a value indicating whether set the formatter enable.
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// Formats a message according to the given styles.
        /// </summary>
        /// <param name="message">The message to style.</param>
        /// <returns>The styled message.</returns>
        string Format(string message);

        /// <summary>
        /// Sets a new output style.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <param name="style">The style instance.</param>
        void SetStyle(string name, IOutputFormatterStyle style);

        /// <summary>
        /// Checks if output formatter has style with specified name.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <returns>true if has specific name style, false otherwise.</returns>
        bool HasStyle(string name);

        /// <summary>
        /// Gets style options from style with specified name.
        /// </summary>
        /// <param name="name">The style name.</param>
        /// <exception cref="GameBox.Console.Exception.InvalidArgumentException">When style isn't defined.</exception>
        IOutputFormatterStyle GetStyle(string name);
    }
}
