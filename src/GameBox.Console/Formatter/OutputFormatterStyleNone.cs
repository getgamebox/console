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
    /// <see cref="OutputFormatterStyleNone"/> does not have any features.
    /// maybe in test class is very useful.
    /// </summary>
    public class OutputFormatterStyleNone : IOutputFormatterStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatterStyleNone"/> class.
        /// </summary>
        public OutputFormatterStyleNone()
        {
        }

        /// <inheritdoc />
        public void SetForeground(string color = null)
        {
            // ignore.
        }

        /// <inheritdoc />
        public void SetBackground(string color = null)
        {
            // ignore.
        }

        /// <inheritdoc />
        public void SetOption(string effect)
        {
            // ignore.
        }

        /// <inheritdoc />
        public void UnsetOption(string effect)
        {
            // ignore.
        }

        /// <inheritdoc />
        public string Format(string text)
        {
            return text;
        }
    }
}
