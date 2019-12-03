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
using GameBox.Console.Helper;
using GameBox.Console.Output;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBox.Console.Style
{
    /// <summary>
    /// Decorates output to add console style guide helpers.
    /// </summary>
    public abstract class AbstractStyleOutput : IOutput, IStyle
    {
        /// <summary>
        /// The std output instance.
        /// </summary>
        private readonly IOutput output;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStyleOutput"/> class.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        protected AbstractStyleOutput(IOutput output)
        {
            this.output = output;
        }

        /// <inheritdoc />
        public OutputOptions Options => output.Options;

        /// <inheritdoc />
        public IOutputFormatter Formatter => output.Formatter;

        /// <inheritdoc />
        public bool IsDecorated => output.IsDecorated;

        /// <inheritdoc />
        public bool IsQuiet => output.IsQuiet;

        /// <inheritdoc />
        public bool IsNormal => output.IsNormal;

        /// <inheritdoc />
        public bool IsVerbose => output.IsVerbose;

        /// <inheritdoc />
        public bool IsVeryVerbose => output.IsVeryVerbose;

        /// <inheritdoc />
        public bool IsDebug => output.IsDebug;

        /// <inheritdoc />
        public Encoding Encoding
        {
            get => output.Encoding;
            set => output.Encoding = value;
        }

        /// <inheritdoc />
        public abstract void Title(string title);

        /// <inheritdoc />
        public abstract void Section(string section);

        /// <inheritdoc />
        public abstract void Comment(string comment);

        /// <inheritdoc />
        public abstract void Listing(string[] elements);

        /// <inheritdoc />
        public abstract void Text(string text);

        /// <inheritdoc />
        public abstract void Success(string text);

        /// <inheritdoc />
        public abstract void Error(string text);

        /// <inheritdoc />
        public abstract void Warning(string text);

        /// <inheritdoc />
        public abstract void Note(string text);

        /// <inheritdoc />
        public abstract void Caution(string text);

        /// <inheritdoc />
        public abstract Mixture Ask(string question, Mixture defaultValue = null, Func<Mixture, Mixture> validator = null, int maxAttempts = 0);

        /// <inheritdoc />
        public abstract Mixture AskPassword(string question, Func<Mixture, Mixture> validator = null, int maxAttempts = 0);

        /// <inheritdoc />
        public abstract bool AskConfirm(string question, bool defaultValue = true);

        /// <inheritdoc />
        public abstract string AskChoice(string question, KeyValuePair<string, string>[] choices, Mixture defaultValue = null, int maxAttempts = 0);

        /// <inheritdoc />
        public void NewLine(int count = 1)
        {
            Write(Str.Repeat(Environment.NewLine, count));
        }

        /// <inheritdoc />
        public abstract void ProgressBegin(int maxStep = 0);

        /// <inheritdoc />
        public abstract void ProcessAdvance(int steps = 1);

        /// <inheritdoc />
        public abstract void ProgressEnd();

        /// <inheritdoc />
        public virtual void Write(string message, bool newLine = false, OutputOptions options = OutputOptions.None)
        {
            output.Write(message, newLine, options);
        }

        /// <inheritdoc />
        public virtual void WriteLine(string message, OutputOptions options = OutputOptions.None)
        {
            output.WriteLine(message, options);
        }

        /// <inheritdoc />
        public virtual void SetVerbosity(OutputOptions verbosity)
        {
            output.SetVerbosity(verbosity);
        }

        /// <inheritdoc />
        public virtual void SetDecorated(bool decorated)
        {
            output.SetDecorated(decorated);
        }

        /// <inheritdoc />
        public virtual void SetOptions(OutputOptions options)
        {
            output.SetOptions(options);
        }

        /// <summary>
        /// Set the message formatter.
        /// </summary>
        /// <param name="formatter">The foramtter.</param>
        public virtual void SetFormatter(IOutputFormatter formatter)
        {
            output.SetFormatter(formatter);
        }

        /// <summary>
        /// Create a new <see cref="ProgressBar"/> instance.
        /// </summary>
        /// <param name="maxSteps">The max steps.(0 if unknown).</param>
        /// <returns>The <see cref="ProgressBar"/> instance.</returns>
        public virtual ProgressBar CreateProgressBar(int maxSteps = 0)
        {
            return new ProgressBar(output, maxSteps);
        }

        /// <summary>
        /// Gets the error output.
        /// </summary>
        /// <returns>The error output.</returns>
        protected IOutput GetErrorOutput()
        {
            return output is OutputConsole console ? console.GetErrorOutput() : output;
        }
    }
}
