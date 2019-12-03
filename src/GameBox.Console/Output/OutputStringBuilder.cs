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
using System.Text;

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="OutputStringBuilder"/> can write messages to the <see cref="StringBuilder"/>.
    /// </summary>
    public class OutputStringBuilder : AbstractOutput
    {
        /// <summary>
        /// The string builder.
        /// </summary>
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStringBuilder"/> class.
        /// </summary>
        /// <param name="options">The output option.</param>
        /// <param name="formatter">Output formatter instance (null to use default <see cref="OutputFormatter"/>).</param>
        public OutputStringBuilder(
            OutputOptions options = OutputOptions.OutputNormal,
            IOutputFormatter formatter = null)
            : base(options, formatter)
        {
        }

        /// <summary>
        /// Empties string builder and returns its content.
        /// </summary>
        /// <returns>The written string.</returns>
        public string Fetch()
        {
            var content = builder.ToString();
            builder.Clear();
            return content;
        }

        /// <inheritdoc />
        protected override void Write(string message, bool newLine)
        {
            builder.Append(message);
            if (newLine)
            {
                builder.Append(Encoding.GetString(GetNewLine()));
            }
        }
    }
}
