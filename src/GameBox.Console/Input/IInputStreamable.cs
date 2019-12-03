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

using System.IO;

namespace GameBox.Console.Input
{
    /// <summary>
    /// <see cref="IInputStreamable"/> is the interface implemented by all input classes
    /// that have an input stream. This is mainly useful for testing purpose.
    /// </summary>
    public interface IInputStreamable : IInput
    {
        /// <summary>
        /// Sets the input stream to read from when interacting with the user.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        void SetInputStream(Stream stream);

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        Stream GetInputStream();
    }
}
