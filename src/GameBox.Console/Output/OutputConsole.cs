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
using System.Text;

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="OutputConsole"/> is the default class for all CLI output. It uses STDOUT and STDERR.
    /// </summary>
    public class OutputConsole : OutputStream, IOutputConsole
    {
        /// <summary>
        /// The standard error stream.
        /// </summary>
        private IOutput stderr;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputConsole"/> class.
        /// </summary>
        /// <param name="options">The output options.</param>
        /// <param name="formatter">Output formatter instance (null to use default <see cref="OutputFormatter"/>).</param>
        public OutputConsole(
            OutputOptions options = OutputOptions.OutputNormal,
            IOutputFormatter formatter = null)
            : base(Terminal.GetStandardOutput(), options, formatter)
        {
            stderr = new OutputStream(Terminal.GetStandardError(), options, formatter);
        }

        /// <inheritdoc />
        public override Encoding Encoding
        {
            get => base.Encoding;
            set
            {
                Terminal.OutputEncoding = value;
                base.Encoding = value;
            }
        }

        /// <summary>
        /// Gets the stderr output interface.
        /// </summary>
        /// <returns>The stderr output value.</returns>
        public IOutput GetErrorOutput()
        {
            return stderr;
        }

        /// <summary>
        /// Sets the stderr output in current console output.
        /// </summary>
        /// <param name="stderr">The stderr output value.</param>
        public void SetErrorOutput(IOutput stderr)
        {
            this.stderr = stderr;
        }

        /// <inheritdoc />
        public override void SetVerbosity(OutputOptions verbosity)
        {
            base.SetVerbosity(verbosity);
            stderr.SetVerbosity(verbosity);
        }

        /// <inheritdoc />
        public override void SetFormatter(IOutputFormatter formatter)
        {
            base.SetFormatter(formatter);
            stderr?.SetFormatter(formatter);
        }

        /// <inheritdoc />
        public override void SetDecorated(bool decorated)
        {
            base.SetDecorated(decorated);
            stderr?.SetDecorated(decorated);
        }

        /// <inheritdoc />
        public override void SetOptions(OutputOptions options)
        {
            base.SetOptions(options);
            stderr?.SetOptions(options);
        }
    }
}
