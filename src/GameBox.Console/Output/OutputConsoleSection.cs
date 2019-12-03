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

#pragma warning disable CA1801
#pragma warning disable CA1822
#pragma warning disable SA1600

using GameBox.Console.Formatter;
using System;
using System.IO;

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="OutputConsoleSection"/> can limit the output to the section, after which the contents of the section can be deleted.
    /// </summary>
    public class OutputConsoleSection : OutputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputConsoleSection"/> class.
        /// </summary>
        /// <param name="stdout">The given stream resources.</param>
        /// <param name="options">The output options.</param>
        /// <param name="formatter">Output formatter instance (null to use default <see cref="OutputFormatter"/>).</param>
        public OutputConsoleSection(Stream stdout, OutputOptions options = OutputOptions.OutputNormal,
            IOutputFormatter formatter = null)
            : base(stdout, options, formatter)
        {
        }

        /// <summary>
        /// Clear the section.
        /// </summary>
        /// <param name="line">How much line will cleaned.</param>
        public void Clear(int line)
        {
            // ignore.
        }

        /// <summary>
        /// Add message in section.
        /// </summary>
        /// <param name="message">The content message.</param>
        protected internal void AddContent(string message)
        {
            // ignore.
        }

        /// <inheritdoc />
        protected override void Write(string message, bool newLine)
        {
            if (!IsDecorated)
            {
                base.Write(message, newLine);
            }

            throw new NotImplementedException();
        }
    }
}
