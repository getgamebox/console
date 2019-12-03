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

using GameBox.Console.Exception;
using GameBox.Console.Formatter;
using GameBox.Console.Util;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="OutputStream"/> writes the output to a given <see cref="Stream"/>.
    /// </summary>
    public class OutputStream : AbstractOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStream"/> class.
        /// </summary>
        /// <param name="stream">The given stream resources.</param>
        /// <param name="options">The output option.</param>
        /// <param name="formatter">Output formatter instance (null to use default <see cref="OutputFormatter"/>).</param>
        public OutputStream(Stream stream, OutputOptions options = OutputOptions.OutputNormal,
            IOutputFormatter formatter = null)
            : base(options, formatter)
        {
            Guard.Requires<ArgumentNullException>(stream != null);

            if (!stream.CanWrite)
            {
                throw new InvalidArgumentException($"The {nameof(stream)} can not writeable.");
            }

            Stream = stream;
            Formatter.Enable = HasDecoratedSupport();
        }

        /// <summary>
        /// Gets the stream attached to this <see cref="OutputStream"/> instance.
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Sets the stream attached to this <see cref="OutputStream"/> instance.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SetStream(Stream stream)
        {
            Stream = stream;
        }

        /// <summary>
        /// Returns true if the stream supports colorization.
        /// </summary>
        /// <returns>Whether the cli is support decorated.</returns>
        protected static bool HasDecoratedSupport()
        {
            // https://hyper.is/
            if (Terminal.GetEnvironmentVariable("TERM_PROGRAM") == "Hyper")
            {
                return true;
            }

            // https://cmder.net/
            if (Terminal.GetEnvironmentVariable("CMDER_SHELL")?.Length > 1)
            {
                return true;
            }

            // https://github.com/adoxa/ansicon
            // https://conemu.github.io/en/ConEmuEnvironment.html
            // https://invisible-island.net/xterm/
            if (Terminal.GetEnvironmentVariable("ANSICON")
                || Terminal.GetEnvironmentVariable("ConEmuANSI") == "ON"
                || Terminal.GetEnvironmentVariable("TERM") == "xterm")
            {
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            try
            {
                return !Terminal.IsInputRedirected;
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return false;
            }
        }

        /// <inheritdoc />
        protected override void Write(string message, bool newLine)
        {
            var data = Encoding.GetBytes(message);
            Stream.Write(data, 0, data.Length);
            if (newLine)
            {
                Stream.Write(GetNewLine(), 0, GetNewLine().Length);
            }

            Stream.Flush();
        }
    }
}
