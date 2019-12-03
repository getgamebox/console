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

using GameBox.Console.Input;
using GameBox.Console.Util;
using System;

namespace GameBox.Console.Tester
{
    /// <summary>
    /// Eases the testing of console applications.
    /// </summary>
    /// <remarks>
    /// This is not a unit test file, so there is no need to
    /// place it in the test case library.
    /// </remarks>
    public class TesterApplication : AbstractTester
    {
        /// <summary>
        /// An array of strings representing each input passed to the command input stream.
        /// </summary>
        private string[] inputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TesterApplication"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public TesterApplication(Application application)
        {
            Application = application;
        }

        /// <summary>
        /// Gets the application instance.
        /// </summary>
        protected Application Application { get; }

        /// <summary>
        /// Sets an array of strings representing each input passed to the command input stream.
        /// </summary>
        /// <param name="inputs">The array of strings.</param>
        /// <returns>The current instance.</returns>
        public TesterApplication SetInputs(string[] inputs)
        {
            this.inputs = inputs;
            return this;
        }

        /// <inheritdoc cref="Run(string, Mixture[])"/>
        public int Run()
        {
            return Run(string.Empty);
        }

        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="instruction">The instruction argument and option.</param>
        /// <param name="options">The option with <see cref="AbstractTester"/>.</param>
        /// <returns>The status code.</returns>
        public int Run(string instruction, params Mixture[] options)
        {
            var input = new InputString(instruction);

            if (options.TryGet("interactive", out Mixture exists))
            {
                input.SetInteractive(exists);
            }

            var shellInteractive = Terminal.GetEnvironmentVariable(EnvironmentVariables.ConsoleShellInteractive);
            if (inputs != null && inputs.Length > 0)
            {
                input.SetInputStream(CreateStream(inputs));
                Terminal.SetEnvironmentVariable(EnvironmentVariables.ConsoleShellInteractive, true);
            }

            Initialize(options);
            var statusCode = Application.Run(input, Output);
            Terminal.SetEnvironmentVariable(EnvironmentVariables.ConsoleShellInteractive, shellInteractive);
            return statusCode;
        }
    }
}
