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
using System.Text;

namespace GameBox.Console.Output
{
    /// <summary>
    /// Base class for output classes.
    /// </summary>
    /// <remarks>
    /// There are five levels of verbosity:
    /// - normal: no option passed (normal output).
    /// - verbose: -v (more output).
    /// - very verbose: -vv (highly extended output).
    /// - debug: -vvv (all debug output).
    /// - quiet: -q (no output).
    /// </remarks>
    public abstract class AbstractOutput : IOutput
    {
        /// <summary>
        /// Verbosities available enumeration.
        /// </summary>
        protected internal const OutputOptions Verbosities = OutputOptions.VerbosityDebug | OutputOptions.VerbosityQuiet |
                                                             OutputOptions.VerbosityVerbose | OutputOptions.VerbosityVeryVerbose |
                                                             OutputOptions.VerbosityNormal;

        /// <summary>
        /// The new line char.
        /// </summary>
        private byte[] newLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractOutput"/> class.
        /// </summary>
        /// <param name="options">The output option.</param>
        /// <param name="formatter">Output formatter instance (null to use default <see cref="OutputFormatter"/>).</param>
        protected AbstractOutput(OutputOptions options = OutputOptions.OutputNormal, IOutputFormatter formatter = null)
        {
            Options = options;
            Formatter = formatter ?? new OutputFormatter();
        }

        /// <inheritdoc />
        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <inheritdoc />
        public OutputOptions Options { get; private set; }

        /// <inheritdoc />
        public IOutputFormatter Formatter { get; private set; }

        /// <inheritdoc />
        public bool IsDecorated => Formatter?.Enable ?? false;

        /// <inheritdoc />
        public bool IsNormal => (OutputOptions.VerbosityNormal & Options) == OutputOptions.VerbosityNormal;

        /// <inheritdoc />
        public bool IsQuiet => (OutputOptions.VerbosityQuiet & Options) == OutputOptions.VerbosityQuiet;

        /// <inheritdoc />
        public bool IsVerbose => (OutputOptions.VerbosityVerbose & Options) == OutputOptions.VerbosityVerbose;

        /// <inheritdoc />
        public bool IsVeryVerbose => (OutputOptions.VerbosityVeryVerbose & Options) == OutputOptions.VerbosityVeryVerbose;

        /// <inheritdoc />
        public bool IsDebug => (OutputOptions.VerbosityDebug & Options) == OutputOptions.VerbosityDebug;

        /// <summary>
        /// Gets the verbosity level.
        /// </summary>
        protected OutputOptions Verbosity => (Verbosities & Options) > 0
            ? Verbosities & Options
            : OutputOptions.VerbosityNormal;

        /// <inheritdoc />
        public virtual void SetDecorated(bool decorated)
        {
            if (Formatter != null)
            {
                Formatter.Enable = decorated;
            }
        }

        /// <inheritdoc />
        public virtual void SetFormatter(IOutputFormatter formatter)
        {
            Formatter = formatter;
        }

        /// <inheritdoc />
        public virtual void SetOptions(OutputOptions options)
        {
            Options = options;
        }

        /// <inheritdoc />
        public virtual void SetVerbosity(OutputOptions verbosity)
        {
            Options = (Options | Verbosities) ^ Verbosities;
            Options = Options | (verbosity & Verbosities);
        }

        /// <inheritdoc />
        public void WriteLine(string message, OutputOptions options = OutputOptions.None)
        {
            Write(message, true, options);
        }

        /// <inheritdoc />
        public void Write(string message, bool newLine = false, OutputOptions options = OutputOptions.None)
        {
            const OutputOptions types = OutputOptions.OutputNormal | OutputOptions.OutputRaw |
                                        OutputOptions.OutputPlain;
            var type = (types & options) > 0 ? types & options : OutputOptions.OutputNormal;
            var verbosity = (Verbosities & options) > 0 ? Verbosities & options : OutputOptions.VerbosityNormal;

            if (verbosity > Verbosity)
            {
                return;
            }

            switch (type)
            {
                case OutputOptions.OutputNormal:
                    message = Formatter.Format(message);
                    break;
                case OutputOptions.OutputRaw:
                    break;
                case OutputOptions.OutputPlain:
                    message = Str.StripHtml(Formatter.Format(message));
                    break;
                default:
                    throw new ConsoleLogicException(
                        $"Only one of the three types is allowed. {nameof(OutputOptions.OutputNormal)},{nameof(OutputOptions.OutputPlain)},{nameof(OutputOptions.OutputRaw)}");
            }

            Write(message, newLine);
        }

        /// <inheritdoc cref="newLine"/>
        protected byte[] GetNewLine()
        {
            return newLine ?? (newLine = Encoding.GetBytes(Environment.NewLine));
        }

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">A message to write to the output.</param>
        /// <param name="newLine">Whether to add a newline.</param>
        protected abstract void Write(string message, bool newLine);
    }
}
