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
using GameBox.Console.Output;
using GameBox.Console.Util;

namespace GameBox.Console.Policy.Configure
{
    /// <summary>
    /// The console log verbosity configure.
    /// </summary>
    internal sealed class ConfigureVerbosity : IConfigurePolicy
    {
        /// <inheritdoc />
        public void Execute(IInput input, IOutput output)
        {
            var shellVerbosity = 0;
            if (input.HasRawOption("--quiet", true) || input.HasRawOption("-q", true))
            {
                output.SetVerbosity(OutputOptions.VerbosityQuiet);
                shellVerbosity = -1;
            }
            else
            {
                if (input.HasRawOption("-vvv", true)
                    || input.HasRawOption("--verbose=3", true)
                    || input.HasRawOption("--verbose=debug", true))
                {
                    output.SetVerbosity(OutputOptions.VerbosityDebug);
                    shellVerbosity = 3;
                }
                else if (input.HasRawOption("-vv", true)
                         || input.HasRawOption("--verbose=2")
                         || input.HasRawOption("--verbose=veryverbose", true))
                {
                    output.SetVerbosity(OutputOptions.VerbosityVeryVerbose);
                    shellVerbosity = 2;
                }
                else if (input.HasRawOption("-v", true)
                         || input.HasRawOption("--verbose=1")
                         || input.HasRawOption("--verbose=verbose", true)
                         || input.HasRawOption("--verbose", true))
                {
                    output.SetVerbosity(OutputOptions.VerbosityVerbose);
                    shellVerbosity = 1;
                }
            }

            if (shellVerbosity == -1)
            {
                input.SetInteractive(false);
            }

            Terminal.SetEnvironmentVariable(EnvironmentVariables.ConsoleShellVerbosity, shellVerbosity);
        }
    }
}
