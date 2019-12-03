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
    /// <see cref="IOutput"/> is the interface implemented by all Standard Output classes.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Gets the output option.
        /// </summary>
        OutputOptions Options { get; }

        /// <summary>
        /// Gets the formatter.
        /// </summary>
        IOutputFormatter Formatter { get; }

        /// <summary>
        /// Gets or sets the encoding for output message.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Gets a value indicating whether is decorated.
        /// </summary>
        bool IsDecorated { get; }

        /// <summary>
        /// Gets a value indicating whether the verbosity is set to <see cref="OutputOptions.VerbosityNormal"/>.
        /// </summary>
        bool IsNormal { get; }

        /// <summary>
        /// Gets a value indicating whether the verbosity is set to <see cref="OutputOptions.VerbosityQuiet"/>.
        /// </summary>
        bool IsQuiet { get; }

        /// <summary>
        /// Gets a value indicating whether the verbosity is set to <see cref="OutputOptions.VerbosityVerbose"/>.
        /// </summary>
        bool IsVerbose { get; }

        /// <summary>
        /// Gets a value indicating whether the verbosity is set to <see cref="OutputOptions.VerbosityVeryVerbose"/>.
        /// </summary>
        bool IsVeryVerbose { get; }

        /// <summary>
        /// Gets a value indicating whether the verbosity is set to <see cref="OutputOptions.VerbosityDebug"/>.
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">The message as an iterable of strings or a single string.</param>
        /// <param name="newLine">Whether to add a newline.</param>
        /// <param name="options">A bitmask of options. <see cref="OutputOptions.None"/> is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.</param>
        void Write(string message, bool newLine = false, OutputOptions options = OutputOptions.None);

        /// <summary>
        /// Writes a message to the output and adds a newline at the end.
        /// </summary>
        /// <param name="message">The message as an iterable of strings or a single string.</param>
        /// <param name="options">A bitmask of options. <see cref="OutputOptions.None"/> is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.</param>
        void WriteLine(string message, OutputOptions options = OutputOptions.None);

        /// <summary>
        /// Sets the option of the output.
        /// </summary>
        /// <param name="options">The options.</param>
        void SetOptions(OutputOptions options);

        /// <summary>
        /// Sets the verbosity level.
        /// </summary>
        /// <param name="verbosity">The verbosity.</param>
        void SetVerbosity(OutputOptions verbosity);

        /// <summary>
        /// Sets the decorate enable flag.
        /// </summary>
        /// <param name="decorated">Whether to decorated the messages.</param>
        void SetDecorated(bool decorated);

        /// <summary>
        /// Set the message formatter.
        /// </summary>
        /// <param name="formatter">The foramtter.</param>
        void SetFormatter(IOutputFormatter formatter);
    }
}
