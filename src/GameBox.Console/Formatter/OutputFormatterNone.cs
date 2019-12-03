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
    /// <see cref="OutputFormatterNone"/> does not have any features.
    /// maybe in test class is very useful.
    /// </summary>
    public sealed class OutputFormatterNone : IOutputFormatter
    {
        /// <inheritdoc />
        public bool Enable { get; set; } = false;

        /// <inheritdoc />
        public string Format(string message)
        {
            return message;
        }

        /// <inheritdoc />
        public IOutputFormatterStyle GetStyle(string name)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool HasStyle(string name)
        {
            return false;
        }

        /// <inheritdoc />
        public void SetStyle(string name, IOutputFormatterStyle style)
        {
            // do nothing.
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
