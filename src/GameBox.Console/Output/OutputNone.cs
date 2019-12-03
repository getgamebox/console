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

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="OutputNone"/> suppresses all output.
    /// </summary>
    public sealed class OutputNone : AbstractOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputNone"/> class.
        /// </summary>
        public OutputNone()
            : base(OutputOptions.None)
        {
        }

        /// <inheritdoc />
        protected override void Write(string message, bool newLine)
        {
            // do nothing.
        }
    }
}
