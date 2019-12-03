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
    /// <see cref="IConfigurePolicy"/> is the interface implemented by all configure policy classes.
    /// </summary>
    public interface IConfigurePolicy
    {
        /// <summary>
        /// Execute the configure policy.
        /// </summary>
        /// <param name="input">The standard input instance.</param>
        /// <param name="output">The standard output instance.</param>
        void Execute(IInput input, IOutput output);
    }
}
