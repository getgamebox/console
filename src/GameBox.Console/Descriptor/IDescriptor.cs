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

using GameBox.Console.Output;
using GameBox.Console.Util;

namespace GameBox.Console.Descriptor
{
    /// <summary>
    /// Descriptor interface for console output.
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// Describes an object if supported.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="content">The described object.</param>
        /// <param name="options">The described option.</param>
        void Describe(IOutput output, object content, params Mixture[] options);
    }
}
