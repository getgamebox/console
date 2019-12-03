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
    /// Eases the testing of console commands.
    /// </summary>
    public class TesterCommand : AbstractTester
    {
        /// <summary>
        /// The test command.
        /// </summary>
        private readonly Command.Command command;

        /// <summary>
        /// An array of strings representing each input passed to the command input stream.
        /// </summary>
        private string[] inputs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TesterCommand"/> class.
        /// </summary>
        /// <param name="command">The test command.</param>
        public TesterCommand(Command.Command command)
        {
            this.command = command;
        }

        /// <summary>
        /// Sets an array of strings representing each input passed to the command input stream.
        /// </summary>
        /// <param name="inputs">The array of strings.</param>
        /// <returns>The current instance.</returns>
        public TesterCommand SetInputs(string[] inputs)
        {
            this.inputs = inputs;
            return this;
        }

        /// <inheritdoc cref="Execute(string, Mixture[])"/>
        public int Execute()
        {
            return Execute(string.Empty);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="instruction">The instruction argument and option.</param>
        /// <param name="options">The option.</param>
        /// <returns>The status code.</returns>
        public int Execute(string instruction, params Mixture[] options)
        {
            var input = new InputString(instruction);

            if (options.TryGet("interactive", out Mixture exists))
            {
                input.SetInteractive(exists);
            }

            if (inputs != null && inputs.Length > 0)
            {
                input.SetInputStream(CreateStream(inputs));
            }

            Initialize(options);
            command.Initialize();
            var statusCode = command.Run(input, Output);
            return statusCode;
        }
    }
}
