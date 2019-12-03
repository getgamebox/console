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

using GameBox.Console.Formatter;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// The Formatter class provides helpers to format messages.
    /// </summary>
    public class HelperFormatter : AbstractHelper
    {
        /// <summary>
        /// The message cache list.
        /// </summary>
        private readonly List<string> lines = new List<string>();

        /// <inheritdoc />
        public override string Name { get; } = "format";

#pragma warning disable CA1822 // Mark members as static

        /// <summary>
        /// Formats a message within a section.
        /// </summary>
        /// <param name="section">The section name.</param>
        /// <param name="message">The message.</param>
        /// <param name="style">The style to apply to the section.</param>
        /// <returns>The formatted string.</returns>
        public string FormatSection(string section, string message, string style = "info")
        {
            return $"<{style}>[{section}]</{style}> {message}";
        }

        /// <summary>
        /// Formats a message within a text.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="style">The style to apply to the section.</param>
        /// <returns>The formatted string.</returns>
        public string FormatText(string message, string style = "info")
        {
            return $"<{style}>{message}</{style}>";
        }

#pragma warning restore CA1822 // Mark members as static

        /// <inheritdoc cref="FormatBlock(string[], string, bool)"/>
        /// <param name="message">The message to write in the block.</param>
        public string FormatBlock(string message, string style, bool large = false)
        {
            return FormatBlock(new[] { message }, style, large);
        }

        /// <summary>
        /// Formats a message as a block of text.
        /// </summary>
        /// <param name="messages">An message array write in the block.</param>
        /// <param name="style">The style to apply to the whole block.</param>
        /// <param name="large">Whether to return a large block.(Add blank lines before and after the block).</param>
        /// <returns>The formatted string.</returns>
        public string FormatBlock(string[] messages, string style, bool large = false)
        {
            lines.Clear();
            var maxLength = 0;
            var result = new string[messages.Length];

            foreach (var message in messages)
            {
                var line = OutputFormatter.Escape(message);
                lines.Add(large ? $"  {line}  " : $" {line} ");
                maxLength = Math.Max(Str.Width(line) + (large ? 4 : 2), maxLength);
            }

            for (var i = 0; i < messages.Length; i++)
            {
                result[i] = lines[i] + Str.Pad(maxLength - Str.Width(lines[i]));
            }

            if (large)
            {
                var pad = Str.Pad(maxLength);
                Arr.Unshift(ref result, pad);
                Arr.Push(ref result, pad);
            }

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = $"<{style}>{result[i]}</{style}>";
            }

            return string.Join(Environment.NewLine, result);
        }
    }
}
