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

namespace GameBox.Console.Policy.Configure
{
    /// <summary>
    /// The console interactive configure.
    /// </summary>
    internal sealed class ConfigureInteractive : IConfigurePolicy
    {
        /// <inheritdoc />
        public void Execute(IInput input, IOutput output)
        {
            if (input.HasRawOption("--no-interaction", true)
                || input.HasRawOption("-n", true))
            {
                input.SetInteractive(false);
            }
        }
    }
}
