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
using GameBox.Console.Output;
using GameBox.Console.Util;
using System;
using System.IO;
using System.Text;

namespace GameBox.Console.Tester
{
    /// <summary>
    /// Base class for tester classes.
    /// </summary>
    public abstract class AbstractTester
    {
        /// <summary>
        /// Whether output of stdout and stderr separately available.
        /// </summary>
        private bool captureStderrSeparately;

        /// <summary>
        /// Gets the std output instnace.
        /// </summary>
        public OutputStream Output { get; private set; }

        /// <summary>
        /// Gets the string encoding.
        /// </summary>
        public Encoding Encoding { get; } = Encoding.UTF8;

        /// <summary>
        /// Make output of stdout and stderr separately available.
        /// </summary>
        /// <param name="enable">Whether separately available.</param>
        /// <returns>The option.</returns>
        public static Mixture OptionStdErrorSeparately(bool enable)
        {
            return new Mixture(enable) { Name = "capture_stderr_separately" };
        }

        /// <summary>
        /// Sets verbosity option.
        /// </summary>
        /// <param name="verbosity">The verbosity version.</param>
        /// <returns>The verbosity option.</returns>
        public static Mixture OptionVerbosity(OutputOptions verbosity)
        {
            var option = AbstractOutput.Verbosities & verbosity;
            return new Mixture((int)option) { Name = "verbosity" };
        }

        /// <summary>
        /// Sets decorated option.
        /// </summary>
        /// <param name="decorated">Whether is decorated the message.</param>
        /// <returns>The decorated option.</returns>
        public static Mixture OptionDecorated(bool decorated)
        {
            return new Mixture(decorated) { Name = "decorated" };
        }

        /// <summary>
        /// Sets interactive option.
        /// </summary>
        /// <param name="interactive">Whether is interactive.</param>
        /// <returns>The interactive option.</returns>
        public static Mixture Interactive(bool interactive)
        {
            return new Mixture(interactive) { Name = "interactive" };
        }

        /// <summary>
        /// Gets the std output string.
        /// </summary>
        /// <param name="encoding">The string encoding.</param>
        /// <returns>The std output string.</returns>
        public string GetDisplay(Encoding encoding = null)
        {
            Output.Stream.Position = 0;
            return Output.Stream.ToText(encoding ?? Encoding, false);
        }

        /// <summary>
        /// Gets the std error string.
        /// </summary>
        /// <param name="encoding">The string encoding.</param>
        /// <returns>The std output string.</returns>
        public string GetDisplayError(Encoding encoding = null)
        {
            encoding = encoding ?? Encoding;
            if (!captureStderrSeparately)
            {
                throw new ConsoleLogicException(
                    $"The error output is not available when the tester is run without {nameof(OptionStdErrorSeparately)} option set.");
            }

            var consoleOutput = (OutputConsole)Output;
            var stderr = consoleOutput.GetErrorOutput();
            var stderrStream = ((OutputStream)stderr).Stream;

            stderrStream.Position = 0;
            return stderrStream.ToText(encoding, false);
        }

        /// <summary>
        /// Create an new input stream.
        /// </summary>
        /// <param name="inputs">An array of strings representing each input passed to the command input stream.</param>
        /// <param name="encoding">The string encoding.</param>
        /// <returns>The input stream.</returns>
        protected static Stream CreateStream(string[] inputs, Encoding encoding)
        {
            var stream = new MemoryStream();

            if (inputs == null || inputs.Length <= 0)
            {
                return stream;
            }

            foreach (var input in inputs)
            {
                var data = encoding.GetBytes($"{input}{Environment.NewLine}");
                stream.Write(data, 0, data.Length);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Create an new input stream.
        /// </summary>
        /// <param name="inputs">An array of strings representing each input passed to the command input stream.</param>
        /// <returns>The input stream.</returns>
        protected Stream CreateStream(string[] inputs)
        {
            return CreateStream(inputs, Encoding);
        }

        /// <summary>
        /// Initialize the output.
        /// </summary>
        /// <param name="options">The options array.</param>
        protected void Initialize(Mixture[] options)
        {
            var outputOptions = OutputOptions.OutputNormal;
            var verbosity = options.Get("verbosity", (int)OutputOptions.VerbosityNormal);
            if (!(verbosity is null))
            {
                outputOptions = outputOptions | (OutputOptions)(int)verbosity;
            }

            captureStderrSeparately = options.Get("capture_stderr_separately", false);
            if (!captureStderrSeparately)
            {
                Output = new OutputStream(new MemoryStream(), outputOptions);
                goto set_decorated;
            }

            var output = new OutputConsole(outputOptions);
            var stderr = new OutputStream(new MemoryStream(), output.Options, output.Formatter);
            output.SetErrorOutput(stderr);
            output.SetStream(new MemoryStream());
            Output = output;

        set_decorated:
            Output.SetDecorated(options.Get("decorated", false));
        }
    }
}
